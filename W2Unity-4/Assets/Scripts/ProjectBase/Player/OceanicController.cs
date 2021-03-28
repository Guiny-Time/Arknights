using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanicController : MonoBehaviour, IPlayer{
    public List<GameObject> playerInRange = new List<GameObject>();//用于范围内友方
    private GameObject poorestPlayer;       //最惨的友方，某种意义上掉血最多的人

    void Start(){
        GetComponent<PlayerController>().die+=OceanicDie;
    }

    void Update(){
        playerInRange = GetComponent<PlayerController>().playerInRange;
        if(playerInRange[0]!=null && playerInRange[0].GetComponent<PlayerController>().GetHealth()!=playerInRange[0].GetComponent<PlayerController>().hp)
            GetComponent<Animator>().Play(Animator.StringToHash("normalHit3"));
    }

     public void Attack(){
        float[] different = new float[playerInRange.Count];
        float maxHP = 0;
        float currentHP = 0;
        float max = 0;
        int index = 0;
        
        for(int i = 0;i<playerInRange.Count;i++){
            maxHP = playerInRange[i].GetComponent<PlayerController>().hp;
            currentHP = playerInRange[i].GetComponent<PlayerController>().GetHealth();
            different[i] = maxHP - currentHP;
        }
        for(int i = 0;i<different.Length;i++){
            if(different[i]>max){
                max = different[i];
                index = i;
            }
        }
        poorestPlayer = playerInRange[index];
        playerInRange[index].GetComponent<PlayerController>().Heal(30);
    }

    public void BigAttack(){
        GetComponent<Animator>().Play(Animator.StringToHash("specialHit3"));
    }

    /// <summary>
    /// 特殊攻击动画事件
    /// </summary>
    public void BigAttackOnAnimate(){
        if(poorestPlayer!=null){//如果没有友方也没关系，就当成他在跳舞
            poorestPlayer.GetComponent<PlayerController>().Heal(10000);//好耶，远洋yyds
            StartCoroutine(OceanicBigAttack());
        }
    }

    /// <summary>
    /// 角色死亡事件
    /// </summary>
    public void OceanicDie(){
        PlayerMgr.GetInstance().PushPlayer("Oceanic",17,PlayerType.HightGround,this.gameObject);
        PlayerMgr.GetInstance().playerList.Remove(this.transform);
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// 特殊攻击，攻击范围内的友方每秒回复30hp，持续5s
    /// </summary>
    IEnumerator OceanicBigAttack(){
        for(int i = 0;i<5;i++){
            foreach(GameObject o in playerInRange){
                o.GetComponent<PlayerController>().Heal(30);
            }
            yield return new WaitForSeconds(1);
        }
        yield return null;
    }
}
