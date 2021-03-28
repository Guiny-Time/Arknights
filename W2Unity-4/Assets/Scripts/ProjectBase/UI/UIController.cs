using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// UI控制器
/// </summary>
public class UIController : MonoBehaviour{
    public GameObject pause; public GameObject start;                       //开始暂停
    public GameObject timeOne; public GameObject timeTwo;                   //倍速切换
    public GameObject MessionFile; public GameObject MessionCompleted;      //任务完成/失败
    public GameObject playMusic; public GameObject stopMusic;               //音乐控制
    public Slider audioSlider;
    public GameObject panel;
    Slider slider; Text costNumber; public static int costNumberInteger = 0;//用于回费
    int enermyCount = 0; int enermyKilledNumber; public Text enermyShowText;//用于检测敌方进攻点数
    public static int saveCount = 3; public Text saveShowText;              //用于我方防御点数
    int playerCount = 0; public Text playerShowText;                        //剩余可放置角色数量
    
    void Start(){
        audioSlider.value = 1;
        EnemyInfo.enermyDie+=KillEnemy;
        Enermy.messionCompleted+=GameWin;
        enermyCount = GameObject.Find("Main Camera").GetComponent<Enermy>().GetEnermyNumber();
    }

    //一些UI数值
    void Update(){
        playerCount = PlayerMgr.GetInstance().playerInfomation.Count;
        slider = GameObject.Find("CostNumberSlider").GetComponent<Slider>();
        costNumber = GameObject.Find("CostNumber").GetComponent<Text>();
        slider.value += Time.deltaTime;
        if(slider.value>=1){
            slider.value = 0;
            costNumberInteger+=1;
            costNumber.text = costNumberInteger.ToString();
        }
        enermyShowText.text = enermyKilledNumber.ToString()+" / "+enermyCount.ToString();
        saveShowText.text = saveCount.ToString();
        playerShowText.text = "剩余可放置角色: "+playerCount.ToString();
        AudioControll(audioSlider);
        if(saveCount == 0)
            GameFail();
    }

    /// <summary>
    /// 游戏失败/防御点为0
    /// </summary>
    void GameFail(){
        MessionFile.SetActive(true);
        Invoke("ToTitle",2f);
    }

    /// <summary>
    /// 游戏胜利/在结束的时候防御点不为0
    /// </summary>
    void GameWin(){
        MessionCompleted.SetActive(true);
        Invoke("ToTitle",2f);
    }

    /// <summary>
    /// 击杀敌人
    /// </summary>
    void KillEnemy(){
        enermyKilledNumber+=1;
    }

    /// <summary>
    /// 返回标题并清空静态变量和委托
    /// </summary>
    void ToTitle(){
        SceneManager.LoadScene("Title");
        Time.timeScale = 1;
        costNumberInteger = 0;
        saveCount = 3;
        PoolMgr.GetInstance().Clear();
        EnermyMove.catchEnemyEvent -= WonsController.catchEnemy;
        Enermy.messionCompleted -= GameWin;
    }

    //下面基本都是OnClick
    //暂停
    public void Pause(){
        Time.timeScale = 0;
        pause.GetComponent<Button>().enabled = false;
        start.GetComponent<Button>().enabled = true;
        pause.gameObject.SetActive(false);
        start.gameObject.SetActive(true);
    }

    //开始
    public void ReStart(){
        Time.timeScale = 1;
        pause.GetComponent<Button>().enabled = true;
        start.GetComponent<Button>().enabled = false;
        pause.gameObject.SetActive(true);
        start.gameObject.SetActive(false);
    }

    //二倍速
    public void TimeDouble(){
        Time.timeScale = 2;
        timeOne.GetComponent<Button>().enabled = false;
        timeTwo.GetComponent<Button>().enabled = true;
        timeOne.gameObject.SetActive(false);
        timeTwo.gameObject.SetActive(true);
    }

    //一倍速
    public void TimeOne(){
        Time.timeScale = 1;
        timeTwo.GetComponent<Button>().enabled = false;
        timeOne.GetComponent<Button>().enabled = true;
        timeTwo.gameObject.SetActive(false);
        timeOne.gameObject.SetActive(true);
    }

    //退出
    public void Exit(){
        Application.Quit();
    }

    //设置
    public void Setting(){
        Pause();
        panel.SetActive(true);
    }

    public void LeaveSetting(){
        ReStart();
        panel.SetActive(false);
    }

    //音量控制
    public void AudioControll(Slider slider){
        float value = slider.value;
        MusicMgr.bkMusic = GetComponent<AudioSource>();
        MusicMgr.GetInstance().ChangeBKValue(value);
    }

    //静音
    public void AudioStopper(){
        MusicMgr.GetInstance().PauseBKMusic();
        stopMusic.GetComponent<Button>().enabled = false;
        playMusic.GetComponent<Button>().enabled = true;
        stopMusic.gameObject.SetActive(false);
        playMusic.gameObject.SetActive(true);
    }

    //重新播放音效
    public void AudioBeginner(){
        MusicMgr.GetInstance().PlaySound();
        playMusic.GetComponent<Button>().enabled = false;
        stopMusic.GetComponent<Button>().enabled = true;
        playMusic.gameObject.SetActive(false);
        stopMusic.gameObject.SetActive(true);
    }
}
