using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// 角色Wons·Retniw的攻击/特殊攻击/死亡
/// </summary>
public class WonsController : MonoBehaviour, IPlayer{   
    public static Queue<GameObject> enermyNumber = new Queue<GameObject>();    //存放阻挡数内的敌人
    private List<GameObject> enermyInRange = new List<GameObject>();
    public Button bigAttackButton;
    private bool specialBegin = false;
    public Action WonsSpecialAttackDone;

    void Start(){
        enermyNumber.Clear();
        GetComponent<PlayerController>().die+=WonsDie;
        EnermyMove.catchEnemyEvent+=catchEnemy;
        GetComponent<PlayerController>().bigAttackReady+=BigAttack;
    }

    void Update(){
        enermyInRange = this.GetComponent<PlayerController>().enermyInRange;
        if(enermyInRange.Count!=0 && !specialBegin)
            GetComponent<Animator>().Play(Animator.StringToHash("normalHit1"));
    }

    /// <summary>
    /// 捕获到被阻挡的敌方/阻挡响应
    /// </summary>
    public static void catchEnemy(GameObject o){
        enermyNumber.Enqueue(o);
    }

    public void Attack(){
        enermyInRange[0].GetComponent<EnemyInfo>().DropHP(50);
    }

    public void BigAttack(){
        bigAttackButton.gameObject.SetActive(true);
        Vector3 worldPos = Camera.main.WorldToScreenPoint(transform.position); //玩家屏幕坐标
        bigAttackButton.GetComponent<RectTransform>().position = new Vector3(worldPos.x,worldPos.y+300,worldPos.z);
    }

    /// <summary>
    /// 手动触发的点击事件
    /// </summary>
    public void BigAttackOnClick(){
        GetComponent<Animator>().Play(Animator.StringToHash("specialHit1"));
        specialBegin = true;
        WonsSpecialAttackDone?.Invoke();
        StartCoroutine(WonsAttack());
    }

    /// <summary>
    /// 角色死亡效果
    /// </summary>
    public void WonsDie(){
        foreach(GameObject o in enermyNumber){
            o.GetComponent<EnermyMove>().enabled = true;
            o.GetComponent<Animator>().Play(Animator.StringToHash("New State"));
        }
        PlayerMgr.GetInstance().PushPlayer("Wons",12,PlayerType.LowGround,this.gameObject);
        PlayerMgr.GetInstance().playerList.Remove(this.transform);
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// 攻击范围内，持续五秒，每秒敌人扣除100hp
    /// </summary>
    IEnumerator WonsAttack(){
        for(int i = 0;i<5;i++){
            foreach(GameObject o in enermyInRange){
                o.GetComponent<EnermyMove>().enabled = false;
                o.GetComponent<EnemyInfo>().DropHP(100);
            }
            yield return new WaitForSeconds(1);
        }
        foreach(GameObject o in enermyInRange){
            o.GetComponent<EnermyMove>().enabled = true;
        }
        specialBegin = false;
        bigAttackButton.gameObject.SetActive(false);
        yield return null;
    }
}
