using UnityEngine;
using System;

/// <summary>
/// 敌方信息
/// </summary>
public class EnemyInfo : MonoBehaviour{
    public int HP = 550;            //敌方生命值
    public static Action enermyDie; //敌方死亡事件

    void Update(){
        if(HP <= 0)
            Die();
    }

    /// <summary>
    /// 敌方死亡
    /// </summary>
    void Die(){
        this.gameObject.GetComponent<EnermyMove>().enabled = true;
        this.GetComponent<EnermyMove>().BackDafult();
        HP = 550;
        enermyDie?.Invoke();
        if(WonsController.enermyNumber.Peek().name == this.name)//释放阻挡数
            WonsController.enermyNumber.Dequeue();
    }

    /// <summary>
    /// 敌方被攻击掉血
    /// </summary>
    public void DropHP(int value){
        HP -= value;
    }
}
