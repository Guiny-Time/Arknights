using UnityEngine.UI;

/// <summary>
/// 生命值管理器
/// </summary>
public class HealthMgr : BaseManager<HealthMgr>{
    /// <summary>
    /// 设置生命值为最大值
    /// </summary>
    /// <param name="slider"></param>
    /// <param name="maxValue"></param>
    public void setMaxHealth(Slider slider,float max){
        slider.maxValue = max;
        slider.value = max;
    }

    /// <summary>
    /// 设置生命值
    /// </summary>
    /// <param name="slider"></param>
    /// <param name="currentValue"></param>
    public void setHealth(Slider slider,float health){
        slider.value = health;
    }

    /// <summary>
    /// 回血方法，用于医疗
    /// </summary>
    /// <param name="slider"></param>
    /// <param name="healHealth"></param>
    /// <param name="maxHealth"></param>
    public void AddHealth(Slider slider,float health,float maxHP){
        slider.value +=health;
        if(slider.value>maxHP)
            slider.value = maxHP;
    }

    /// <summary>
    /// 获取当前生命值
    /// </summary>
    /// <param name="slider"></param>
    public float getHealth(Slider slider){
        return slider.value;
    }
}
