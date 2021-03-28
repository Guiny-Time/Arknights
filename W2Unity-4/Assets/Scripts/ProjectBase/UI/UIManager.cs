using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// UI层级
/// </summary>
public enum E_UI_Layer
{
    Bot,Mid,Top,System,
}

/// <summary>
/// UI管理器
/// 1.管理所有显示的面板
/// 2.提供给外部 显示和隐藏等等接口
/// </summary>
public class UIManager : BaseManager<UIManager>
{
    //记录我们UI的Canvas父对象 方便以后外部可能会使用它
    public RectTransform canvas;

    public UIManager(){
        //创建Canvas 让其过场景的时候 不被移除
        GameObject obj = GameObject.Find("UI_Canvas Variant");
        canvas = obj.transform as RectTransform;
        GameObject.DontDestroyOnLoad(obj);
    }

    /// <summary>
    /// 显示面板
    /// </summary>
    public void ShowPanel(string panelName){
        GameObject.Find(panelName).SetActive(true);
    }

    /// <summary>
    /// 隐藏面板
    /// </summary>
    /// <param name="panelName"></param>
    public void HidePanel(string panelName){
        GameObject.Find(panelName).SetActive(false);
    }

}
