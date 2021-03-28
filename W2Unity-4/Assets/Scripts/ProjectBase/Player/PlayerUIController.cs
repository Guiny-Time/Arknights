using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

/// <summary>
/// 与玩家有关的UI组件(显示角色图片的按钮/角色信息面板)的控制器
/// 这个类确实写的有点猥琐：
/// 1.获取鼠标位置以便在对应的地方实例化角色
/// 2.放置成功之后检查费用并扣费
/// 3.拖拽的时候显示角色面板同时显示可放置区域的颜色，松开的时候消失
/// 4.正确放置的角色被吸附到格子中心
/// 5.通过按钮获取角色信息
/// 6.eventTrigger相关
/// </summary>

public class PlayerUIController : MonoBehaviour
{
    Color putInGreen; Color putInYellow;            //放置显色
    public Material high; public Material low;      //用于拖拽时可放置区域的显示
    public Animator cameraAnim;                     //相机旋转动画
    List<PlayerInfo> players = new List<PlayerInfo>();
    Dictionary<string,GameObject> playerPrafebs = new Dictionary<string, GameObject>();
    List<Button> buttons = new List<Button>();
    Vector3 mouseposition; Vector3 mousPositionAfterRotation;   //鼠标的世界坐标与旋转偏移坐标
    public GameObject block;                                    //射线的基准点,,,随便抓了个cube
    public Text text; int costNumber = 0;                       //cost点数
    static int index = -1;                                      //角色的编号

    void Start(){
        //初始化
        PlayerMgr.GetInstance().DefaultList();
        players = PlayerMgr.GetInstance().playerInfomation;
        buttons = PlayerMgr.GetInstance().buttonList;
        PlayerMgr.GetInstance().InitPlayerAndButton();
        //颜色设置
        ColorUtility.TryParseHtmlString("#6FDD5B", out putInGreen);
        ColorUtility.TryParseHtmlString("#FFFFFF", out putInYellow);
        high.color = putInYellow;
        low.color = putInYellow;
        //获取player预制件
        for(int i = 0;i<players.Count;i++){
            playerPrafebs.Add(players[i].name,GameObject.Find(players[i].name+" Variant"));
            playerPrafebs[players[i].name].gameObject.SetActive(false);
        }
    }
    
    void Update(){
        CostCheck();
        MouseCheck();
    }

    /// <summary>
    /// 获取鼠标屏幕坐标与读取信息面板时偏移的坐标
    /// </summary>
    public void MouseCheck(){
        mouseposition = Input.mousePosition;
        mouseposition.z = 0;
        mouseposition = Camera.main.ScreenToWorldPoint(mouseposition);
        mouseposition.z = -1;
        mousPositionAfterRotation = mouseposition-new Vector3(-1,0,0);
    }

    /// <summary>
    /// 费用与角色的联系/在按钮的子Text上显示费用
    /// </summary>
    void CostCheck(){
        costNumber = int.Parse(text.text);
        for(int i = 0;i<players.Count;i++){
            if(costNumber<players[i].cost){
                buttons[i].GetComponent<EventTrigger>().enabled = false;
                buttons[i].GetComponentInChildren<Image>().color = Color.gray;
            }else{
                buttons[i].GetComponent<EventTrigger>().enabled = true;
                buttons[i].GetComponentInChildren<Image>().color = Color.white;
            }
        }
    }

    /// <summary>
    /// 拖拽角色，用于EventTrigger
    /// </summary>
    public void OnPointerDown(GameObject sender){
        PlayerType type = GetPlayerInformation(sender).type;
        string playerName = GetPlayerInformation(sender).name;
        ShowInfomationPanel(index);             //第几个角色的信息面板
        index = -1;
        cameraAnim.Play("Lerp");
        ShowColorForCube(type,playerName);//显示可放置区域
    }
    
    /// <summary>
    /// 放置角色，用于EventTrigger
    /// </summary>
    public void OnPointerUp(){
        string playerName = "";
        int cost = 0;
        PlayerType type = PlayerType.HightGround;   
        //读取实例化对象信息
        for(int i = 0;i<buttons.Count;i++){
            if(!buttons[i].enabled){
                playerName = players[i].name;
                buttons[i].enabled = true;
                cost = players[i].cost;
                type = players[i].type;
            }
        }

        cameraAnim.Play("Back");                        //镜头回转
        HideInfomationPanel();                          //面板收起

        if(playerPrafebs.ContainsKey(playerName)){
            playerPrafebs[playerName].SetActive(false); 
            try{
                playerPrafebs[playerName].GetComponent<PutDownController>().enabled = true;
                playerPrafebs[playerName].GetComponent<PlayerController>().line.enabled = true;
            }catch(Exception){
                playerPrafebs[playerName].AddComponent<PutDownController>();
            }           
        }

        PutDown(playerName,type,cost);
        if(playerPrafebs[playerName].activeInHierarchy){
            PlayerMgr.GetInstance().GetPlayer(playerName);
        }
    }

    /// <summary>
    /// 放置完成
    /// </summary>
    void PutDown(string playerName,PlayerType type,int cost){
        RaycastHit hit;
        
        float transformX = (float)Math.Round(mousPositionAfterRotation.x,0);
        float transformY = (float)Math.Round(mousPositionAfterRotation.y,0);
        Vector3 integerMousePos = new Vector3(transformX,transformY,-1.3f);//整数坐标

        Physics.Raycast(integerMousePos, block.transform.forward, out hit, 50f);
        if(type == PlayerType.HightGround && hit.collider.name.Contains("高台"))
            PutDownPlayerToCenter(playerName,cost,integerMousePos);
        if(type == PlayerType.LowGround && hit.collider.name.Contains("可站立Cube1"))
            PutDownPlayerToCenter(playerName,cost,integerMousePos);

        high.color = putInYellow;
        low.color = putInYellow;
    }

    /// <summary>
    /// 可放置区域的显色与拖拽的时候角色在鼠标上
    /// </summary>
    void ShowColorForCube(PlayerType type, string playerName){
        if(type == PlayerType.HightGround){
            high.color = putInGreen;
            low.color = putInYellow;
        }else{
            low.color = putInGreen;
            high.color = putInYellow;
        }
        if(playerPrafebs.ContainsKey(playerName)){
            playerPrafebs[playerName].SetActive(true);
            playerPrafebs[playerName].transform.position = mousPositionAfterRotation;
            
        }  
    }

    /// <summary>
    /// 放置玩家到格子中心并扣费
    /// </summary>
    void PutDownPlayerToCenter(string playerName, int cost, Vector3 transformPos){
        playerPrafebs[playerName].transform.position = transformPos;
        playerPrafebs[playerName].SetActive(true);
        UIController.costNumberInteger-=cost;
    }

    /// <summary>
    /// 显示角色信息面板
    /// </summary>
    public void ShowInfomationPanel(int index)//todo: 加上设置alpha值的参数以达到细微的不同
    {
        GameObject canvass = GameObject.Find("InfoPanel");
        for(int i=0; i<canvass.transform.childCount; i++){
            canvass.transform.GetChild(i).transform.gameObject.SetActive(false);
        }
        string name = players[index].name;
        GameObject infoPanel =  canvass.transform.Find(name+"InfoPanel").gameObject;       
        infoPanel.SetActive(true);
    }

    /// <summary>
    /// 隐藏角色信息面板
    /// </summary>
    public void HideInfomationPanel()
    {
        GameObject canvass = GameObject.Find("InfoPanel");
        for(int i=0; i<canvass.transform.childCount; i++){
            canvass.transform.GetChild(i).transform.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 通过按钮获取角色信息
    /// </summary>
    PlayerInfo GetPlayerInformation(GameObject sender){
        for(int i = 0;i<buttons.Count;i++){
            if(buttons[i].name == sender.name){
                buttons[i].enabled = false;
                index = i;
                return players[i];
            }
        }
        return null;
    }
}