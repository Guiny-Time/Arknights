using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class OrionController : MonoBehaviour, IPlayer
{
    private List<GameObject> enermyInRange = new List<GameObject>();
    private List<GameObject> playerInRange = new List<GameObject>();
    public Button bigAttackButton;
    private bool specialBegin = false;
    public Action OrionSpecialAttackDone;

    void Start()
    {
        GetComponent<PlayerController>().bigAttackReady+=BigAttack;
    }

    void Update()
    {
        enermyInRange = this.GetComponent<PlayerController>().enermyInRange;
        playerInRange = this.GetComponent<PlayerController>().playerInRange;
        if(enermyInRange.Count!=0 && !specialBegin)
            GetComponent<Animator>().Play(Animator.StringToHash("normalHit"));
    }

    //普通攻击，对所有进入攻击范围的敌人造成80点伤害
    public void Attack(){
        foreach(GameObject o in enermyInRange){
            o.GetComponent<EnemyInfo>().DropHP(80);
        }
    }

    //特殊攻击，范围内所有敌人和友方死亡
    public void BigAttack(){
        bigAttackButton.gameObject.SetActive(true);
        Vector3 worldPos = Camera.main.WorldToScreenPoint(transform.position); //玩家屏幕坐标
        bigAttackButton.GetComponent<RectTransform>().position = new Vector3(worldPos.x,worldPos.y+350,worldPos.z);
    }

    //点击按钮播放动画
    public void BigAttackOnClick(){
        bigAttackButton.gameObject.SetActive(false);
        GetComponent<Animator>().Play(Animator.StringToHash("specialHit"));
        specialBegin = true;
        OrionSpecialAttackDone?.Invoke();
    }

    //动画自带事件
    public void BigAttackOnAnimate(){
        foreach(GameObject o in enermyInRange){
            o.GetComponent<EnermyMove>().count = 0;
            PoolMgr.GetInstance().PushObj(o.name,o);
        }
        foreach(GameObject o in playerInRange){
            o.GetComponent<PlayerController>().SetNewHealth(0);
        }
        specialBegin = false;
    }
}
