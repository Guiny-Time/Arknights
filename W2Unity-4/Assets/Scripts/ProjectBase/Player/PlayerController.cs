using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// 玩家控制器，管理玩家放置/数值事件/攻击范围/范围内成员
/// </summary>
public class PlayerController : MonoBehaviour{
    [Header("Player Infomation")]
    public Slider HPBar;
    public Slider MPBar;
    public Big_Attack_Type type = Big_Attack_Type.Auto;         //技能类型，默认自动触发
    public int hp;
    public float maxMp;
    float currentMp = 0;
    public float speed;                                         //回复mp的速度
    public Action die;

    [Header("Attack Infomation")]
    public Vector2[] attackRange;                               //攻击范围(以自身为原点)
    public Vector2[] attackRangeForBloack;                      //攻击范围所对应的真实的的格子坐标
    public List<GameObject> enermyInRange = new List<GameObject>(); //位于攻击范围内的敌人
    public List<GameObject> playerInRange = new List<GameObject>(); //位于攻击范围内的友方
    Direction currentOrientation;                                   //朝向
    Vector2[] left; Vector2[] up; private Vector2[] down;   //数据结构的问题
    float xMin = 0; float xMax = 0; float yMin = 0; float yMax = 0;         //攻击范围边界参数
    public LineRenderer line;
    public Action bigAttackReady;

    void Start(){
        SetHealth();
        if(GetComponent<WonsController>()){
            GetComponent<WonsController>().WonsSpecialAttackDone+=SetMagic;
        }
        if(GetComponent<OrionController>()){
            GetComponent<OrionController>().OrionSpecialAttackDone+=SetMagic;
        }
        SetDefaultDirectionArray();
        line = gameObject.GetComponent<LineRenderer>();
        line.positionCount = 5;
        line.startWidth = 0.09f;
        line.endWidth = 0.09f;
    }

    void Update()
    {
        MpPP(maxMp,speed,type);
        currentOrientation = this.GetComponent<PutDownController>().orientation;
        HealthMgr.GetInstance().setHealth(MPBar,currentMp);
        if(HealthMgr.GetInstance().getHealth(HPBar) == 0){
            die?.Invoke();
            HealthMgr.GetInstance().setHealth(HPBar,hp);
            HealthMgr.GetInstance().setHealth(MPBar,0);
            currentMp = 0;
        }
        SetDirection();
        criticalPoint(attackRangeForBloack);
        creatLineCast(xMin, xMax, yMin, yMax);
        PlayerEnemyPisitionListAdder(PlayerMgr.GetInstance().enemyList, enermyInRange);//获得在范围内的敌人列表
        PlayerEnemyPisitionListAdder(PlayerMgr.GetInstance().playerList, playerInRange);//获得在范围内的玩家列表
    }

    /// <summary>
    /// 初始化表示不同朝向的攻击范围的数组
    /// </summary>
    public void SetDefaultDirectionArray(){
        attackRangeForBloack = (Vector2[])attackRange.Clone();
        left = (Vector2[])attackRange.Clone();
        up = (Vector2[])attackRange.Clone();
        down = (Vector2[])attackRange.Clone();
        for(int i = 0;i<attackRangeForBloack.Length;i++){
            left[i] = new Vector2(-left[i].x,left[i].y);
            up[i] = new Vector2(-up[i].y,up[i].x);
            down[i] = new Vector2(down[i].y,-down[i].x);
        }
    }

    /// <summary>
    /// 根据当前朝向设置新的攻击范围
    /// </summary>
    public void SetDirection(){
        if(GetComponent<PutDownController>().enabled){
            if(currentOrientation == Direction.Right){
                for(int i = 0;i<attackRange.Length;i++)
                    attackRangeForBloack[i] = new Vector2(transform.position.x+attackRange[i].x, transform.position.y+attackRange[i].y);
            }
            if(currentOrientation == Direction.Left){
                for(int i = 0;i<attackRange.Length;i++)
                    attackRangeForBloack[i] = new Vector2(transform.position.x+left[i].x,transform.position.y+left[i].y);
            }
            if(currentOrientation == Direction.Up){
                for(int i = 0;i<attackRange.Length;i++)
                    attackRangeForBloack[i] = new Vector2(transform.position.x+up[i].x,transform.position.y+up[i].y);
            }
            if(currentOrientation == Direction.Down){
                for(int i = 0;i<attackRange.Length;i++)
                    attackRangeForBloack[i] = new Vector2(transform.position.x+down[i].x,transform.position.y+down[i].y);
            }
        }
    }

    /// <summary>
    /// 初始化生命值
    /// </summary>
    public void SetHealth(){
        HealthMgr.GetInstance().setMaxHealth(HPBar,hp);
        HealthMgr.GetInstance().setHealth(HPBar,hp);
        HealthMgr.GetInstance().setMaxHealth(MPBar,maxMp);
        HealthMgr.GetInstance().setHealth(MPBar,0);
    }

    /// <summary>
    /// 设置魔力值为0
    /// </summary>
    public void SetMagic(){
        currentMp = 0;
    }

    /// <summary>
    /// MP自增；分为自动触发和手动触发两种
    /// </summary>
    public void MpPP(float maxNum, float speed,Big_Attack_Type type){
        currentMp += speed * Time.deltaTime;
        switch(type){
            case(Big_Attack_Type.Auto):
                if(currentMp>=maxMp){
                    GetComponent<Animator>().Play(Animator.StringToHash("specialHit3"));
                    currentMp = 0;
                }
                break;
            case(Big_Attack_Type.HandControl):
                if(currentMp>=maxMp){
                    bigAttackReady?.Invoke();
                }
                break;
        }
    }

    /// <summary>
    /// 获取当前攻击范围的边界
    /// </summary>
    public void criticalPoint(Vector2[] array){
        xMin = yMin = 999;
        xMax = yMax =-999;
        for(int i = 0; i < array.Length; i++){
            if(array[i].x < xMin)
                xMin = array[i].x-0.5f;
            if(array[i].x >= xMax)
                xMax = array[i].x+0.5f;
            if(array[i].y > yMax)
                yMax = array[i].y+0.5f;
            if(array[i].y < yMin)
                yMin = array[i].y-0.5f;
        }
    }

    /// <summary>
    /// 绘制攻击范围
    /// </summary>
    public void creatLineCast(float xMin, float xMax, float yMin, float yMax){
        line.SetPosition(0, new Vector3(xMin,yMin,-1));
        line.SetPosition(1, new Vector3(xMax,yMin,-1));
        line.SetPosition(2, new Vector3(xMax,yMax,-1));
        line.SetPosition(3, new Vector3(xMin,yMax,-1));
        line.SetPosition(4, new Vector3(xMin,yMin,-1));
    }

    /// <summary>
    /// 捕获范围内元素
    /// </summary>
    public void PlayerEnemyPisitionListAdder(List<Transform> list1,List<GameObject> list2){
        list2.Clear();
        foreach(Transform t in list1){
            if(t.position.x > xMin && t.position.x < xMax && t.position.y > yMin && t.position.y < yMax){
                if(!list2.Contains(t.gameObject))
                    list2.Add(t.gameObject);
            }else{
                list2.Remove(t.gameObject);
            }
        }
    }

    /// <summary>
    /// 获取玩家当前HP
    /// </summary>
    public float GetHealth(){
        return HealthMgr.GetInstance().getHealth(HPBar);
    }

    /// <summary>
    /// 给玩家回复一定的Hp；无法溢出原有的最大值
    /// </summary>
    public void Heal(float value){
        HealthMgr.GetInstance().AddHealth(HPBar,value,hp);
    }

    /// <summary>
    /// 暴力改值（
    /// </summary>
    public void SetNewHealth(float health){
        HPBar.value = health;
    }
}