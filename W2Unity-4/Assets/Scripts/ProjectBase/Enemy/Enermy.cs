using System.Collections;
using UnityEngine.Events;
using UnityEngine;
using System;

/// <summary>
/// 敌方进攻波数生成器
/// </summary>
public class Enermy : MonoBehaviour{
    //设置波数
    [System.Serializable]
    public class Wave{
        public int count;           //具体波数
        public float rate;          //敌人生成间隔
        public Vector2 starPoint; public Vector2 endPoint;  //自定义起始点
    }
    [Header("进攻波数信息")]
    public GameObject enermy;
    public GameObject flash;
    public Vector2 startPoint;
    public Vector2 endPoint;
    public Wave[] waves;                //自定义进攻波数
    private int nextWaveCount = 0;      //下一波
    public float timeBetweenWaves = 5f; //波之间的时间间隔
    public float waveCount;             //波数计数
    private float countingTime = 1;     //搜索计时
    public SpawnState state = SpawnState.COUNTING;//当前状态
    public UnityAction<GameObject> getObjectCallBack;
    public static Action messionCompleted;
    
    void Start(){
        AStarMgr.GetInstance().InitMapInfo("/Scenes/Level1/level1Map.txt",11,7);
        waveCount = timeBetweenWaves;
        getObjectCallBack =(GameObject o) =>{PlayerMgr.GetInstance().enemyList.Add(o.transform);};
    }

    void Update()   {
        if(state == SpawnState.WAITING){
            if(!EnermyIsAlive()){
                WaveCompleted();//进攻完成
            }else{
                return;
            }
        }
        if(waveCount <=0){
            if(state != SpawnState.SPAWING){
                StartCoroutine(SpawnWave(waves[nextWaveCount]));//开始下一波
                startPoint = waves[nextWaveCount].starPoint;
                endPoint = waves[nextWaveCount].endPoint;
            }
        }else{
            waveCount -= Time.deltaTime;           
        }
    }

    /// <summary>
    /// 进攻完成
    /// </summary>
    void WaveCompleted(){
        state = SpawnState.COUNTING;
        waveCount = timeBetweenWaves;
        if(nextWaveCount + 1 > waves.Length - 1){
            messionCompleted?.Invoke();
            GetComponent<Enermy>().enabled = false;            
        }else{
            nextWaveCount++;
        }
    }

    /// <summary>
    /// 判断场内是否还存在敌方物体
    /// </summary>
    bool EnermyIsAlive(){
        countingTime -= Time.deltaTime;
        if(countingTime<=0){
            countingTime = 1f;
            if(GameObject.FindGameObjectWithTag("Enermy") == null){
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 波内敌人的生成和生成间隔
    /// </summary>
    IEnumerator SpawnWave(Wave _wave){
        state = SpawnState.SPAWING;
        for(int i = 0;i< _wave.count;i++){
            if(i == 0){
                SpawnEnermy(flash);
                yield return new WaitForSeconds(5f);
            }else{
                SpawnEnermy(enermy);
                yield return new WaitForSeconds(1f/_wave.rate);
            }
        }
        state = SpawnState.WAITING;
        yield break;
    }

    /// <summary>
    /// 生成敌人
    /// </summary>
    void SpawnEnermy(GameObject _enermy){
        PoolMgr.GetInstance().GetObj("prefeb/"+_enermy.name, new Vector3(startPoint.x-5,-startPoint.y,-0.8477783f), _enermy.transform.rotation, getObjectCallBack);       
    }

    /// <summary>
    /// 获取敌人总数
    /// </summary>
    public int GetEnermyNumber(){
        int num = 0;
        for(int i = 0;i<waves.Length;i++){
            num+=waves[i].count;
        }
        return num-waves.Length;
    }
}