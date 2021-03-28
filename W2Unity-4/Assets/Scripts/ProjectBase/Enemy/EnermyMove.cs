using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 控制敌人ai移动
/// </summary>
public class EnermyMove : MonoBehaviour{
    public int count = 0;
    Vector2 a; Vector2 b;                       //移动的起始点
    List<AStarNode> finalPathFromAStar;         //自动寻路结果
    public static Quaternion defaultRotation;   //朝向
    public float moveSpeed;                     //移动速度
    Enermy enermyWaveController;
    public static Action<GameObject> catchEnemyEvent;//阻挡捕获事件
    private GameObject hitPlayer;               //被击中的玩家

    void Start(){       
        enermyWaveController = GameObject.Find("Main Camera").GetComponent<Enermy>();
        AStarMgr.GetInstance().InitMapInfo("/Scenes/Level1/level1Map.txt", 11, 7);
        defaultRotation = new Quaternion(transform.rotation.x,transform.rotation.y,transform.rotation.z,transform.rotation.w);
    }

    void Update(){
        try{
            EnermyMoveing();
        }catch(ArgumentOutOfRangeException){
            count = 0;
        }
    }

    /// <summary>
    /// 移动过程中遇到的事情（碰到我方单位/碰到防御点
    /// </summary>
    void EnermyMoveing(){
        a = enermyWaveController.startPoint;
        b = enermyWaveController.endPoint;
        finalPathFromAStar = AStarMgr.GetInstance().FindPath(a, b);
        RaycastHit hit;
        if (Physics.Raycast(transform.Find("ray").position, transform.Find("ray").transform.forward, out hit,0.1f)){
            if(hit.collider.name.Contains("Safe")){
                SafeCubeCatcher();
            }else if(hit.collider.name.Contains("Wons") && hit.collider.gameObject.activeInHierarchy){
                PlayerCatcher(hit);
            }else{
                MoveGameObject();
            }
        }else{
            MoveGameObject();
        }
    }

    /// <summary>
    /// 敌人遇到防御点
    /// </summary>
    void SafeCubeCatcher()
    {
        BackDafult();
        UIController.saveCount-=1;
    }

    /// <summary>
    /// 敌人遇到我方单位
    /// </summary>
    void PlayerCatcher(RaycastHit hit)
    {
        hitPlayer = hit.collider.gameObject;
        if(WonsController.enermyNumber.Count<2 && hitPlayer.GetComponent<PlayerController>().enermyInRange.Contains(this.gameObject)){
            catchEnemyEvent?.Invoke(this.gameObject);       //令该物体入队
            this.gameObject.GetComponent<EnermyMove>().enabled = false;
            GetComponent<Animator>().Play(Animator.StringToHash("attack"));
        }else{
            MoveGameObject();
            GetComponent<Animator>().Play(Animator.StringToHash("New State"));
        }
    }

    /// <summary>
    /// 根据路径进行移动
    /// </summary>
    void MoveGameObject(){
        Vector3 NextPos = new Vector3(finalPathFromAStar[count].x - 5, -finalPathFromAStar[count].y, -0.8477783f);
        if(transform.position == NextPos){
            count++;
            if(count+1>finalPathFromAStar.Count){
                if(this.name.Contains("flash")){
                    gameObject.GetComponent<EnermyMove>().enabled = false;
                    PlayerMgr.GetInstance().enemyList.Remove(this.gameObject.transform);
                    Destroy(this.gameObject,0.5f);
                }
                count = 0; 
            }
        }else{
            if(this.name.Contains("enermy")){
                transform.rotation = Quaternion.LookRotation(NextPos-transform.position, Vector3.forward);//转向
            }            
            transform.position = Vector3.MoveTowards(transform.position, NextPos, moveSpeed * Time.deltaTime);            
        }
    }

    /// <summary>
    /// 敌方被阻挡时开始攻击玩家
    /// </summary>
    public void Attack(){
        Slider s = hitPlayer.GetComponent<PlayerController>().HPBar;
        float a = HealthMgr.GetInstance().getHealth(s);
        HealthMgr.GetInstance().setHealth(s,a-25);
    }

    /// <summary>
    /// 使敌方回到默认位置
    /// </summary>
    public void BackDafult(){
        transform.position = new Vector3(a.x-5,-a.y,-0.8477783f);
        transform.rotation = defaultRotation;
        PoolMgr.GetInstance().PushObj(this.name,this.gameObject);
        count = 0;
    }
}