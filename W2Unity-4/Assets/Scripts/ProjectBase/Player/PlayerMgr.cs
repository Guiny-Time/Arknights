using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 角色管理器
/// 包含在场敌人、我方角色、以及用于角色显示的按钮
/// </summary>
public class PlayerMgr : BaseManager<PlayerMgr>
{
    public List<PlayerInfo> playerInfomation = new List<PlayerInfo>();  //全体角色信息，主要用途是角色禁用复用/按钮图标控制
    public List<Button> buttonList = new List<Button>();
    public List<Transform> enemyList = new List<Transform>();   //在场敌人位置信息
    public List<Transform> playerList = new List<Transform>();  //在场我方位置信息

    /// <summary>
    /// 初始化列表；
    /// 舟游的初始化感觉是在选择角色的那个界面实现的，这里直接手动加了
    /// </summary>
    public void DefaultList(){
        Clear();
        playerInfomation.Add(new PlayerInfo("Wons", 12, PlayerType.LowGround)); 
        playerInfomation.Add(new PlayerInfo("Oceanic", 17, PlayerType.HightGround));
        playerInfomation.Add(new PlayerInfo("Orion", 24, PlayerType.HightGround));
        for(int i = 0;i<playerInfomation.Count;i++){
            buttonList.Add(GameObject.Find("playerButton"+(i+1)).GetComponent<Button>());
        }
    }
    void Clear(){
        playerList.Clear();
        enemyList.Clear();
        playerInfomation.Clear();
        buttonList.Clear();
    }

    /// <summary>
    /// 初始化按钮列表；角色和按钮图片相对
    /// </summary>
    public void InitPlayerAndButton(){
        for(int i = 0;i<playerInfomation.Count;i++){
            buttonList[i].GetComponentInChildren<Image>().sprite = ResMgr.GetInstance().Load<Sprite>("UI/"+playerInfomation[i].name);
            buttonList[i].GetComponentInChildren<Text>().text = "费用: "+playerInfomation[i].cost;
        }
    }

    /// <summary>
    /// 从玩家列表中取出角色
    /// </summary>
    /// <param name="playerName"></param>
    public void GetPlayer(string name){
        for(int i = 0;i<playerInfomation.Count;i++){
            if(name == playerInfomation[i].name && playerInfomation.Count > 1){
                playerInfomation.Remove(playerInfomation[i]);
                buttonList[playerInfomation.Count].gameObject.SetActive(false);
                InitPlayerAndButton();
            }
            if(name == playerInfomation[i].name && playerInfomation.Count == 1){
                playerInfomation.Remove(playerInfomation[0]); 
                buttonList[0].gameObject.SetActive(false);
                InitPlayerAndButton();
            }
        }
    }

    /// <summary>
    /// 将角色放回玩家列表
    /// </summary>
    public void PushPlayer(string name,int cost,PlayerType type,GameObject player){
        GameObject.Find(name+" Variant").SetActive(false);
        playerInfomation.Add(new PlayerInfo(name, cost, type));
        buttonList[playerInfomation.Count-1].enabled = true;
        buttonList[playerInfomation.Count-1].gameObject.SetActive(true);
        InitPlayerAndButton();
    }
}
