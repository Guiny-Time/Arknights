﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//这个类看着真的挺诱人的
//但是用不清楚(昏迷了

/// <summary>
/// 面板基类 
/// 帮助我们通过代码快速的找到所有的子控件
/// 方便我们在子类中处理逻辑 
/// 节约找控件的工作量
/// </summary>
public class BasePanel : MonoBehaviour
{
    //通过里式转换原则 来存储所有的控件
    private Dictionary<string, List<UIBehaviour>> controlDic = new Dictionary<string, List<UIBehaviour>>();

	// Use this for initialization
	protected virtual void Awake () {
        FindChildrenControl<Button>();
        FindChildrenControl<Image>();
        FindChildrenControl<Text>();
        FindChildrenControl<Toggle>();
        FindChildrenControl<Slider>();
        FindChildrenControl<ScrollRect>();
        FindChildrenControl<InputField>();
        FindChildrenControl<RawImage>();
    }

    /// <summary>
    /// 显示自己
    /// </summary>
    public virtual void ShowMe(string name)
    {
        GameObject.Find(name).SetActive(true);
    }

    /// <summary>
    /// 隐藏自己
    /// </summary>
    public virtual void HideMe(string name)
    {
        GameObject.Find(name).SetActive(false);
    }

    /// <summary>
    /// 得到对应名字的对应控件脚本
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="controlName"></param>
    /// <returns></returns>
    protected T GetControl<T>(string controlName) where T : UIBehaviour
    {
        if(controlDic.ContainsKey(controlName))
        {
            for( int i = 0; i <controlDic[controlName].Count; ++i )
            {
                if (controlDic[controlName][i] is T)
                    return controlDic[controlName][i] as T;
            }
        }
        return null;
    }

    /// <summary>
    /// 找到子对象的对应控件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private void FindChildrenControl<T>() where T:UIBehaviour
    {
        T[] controls = this.GetComponentsInChildren<T>();
        for (int i = 0; i < controls.Length; ++i)
        {
            string objName = controls[i].gameObject.name;
            if (controlDic.ContainsKey(objName))
                controlDic[objName].Add(controls[i]);
            else
                controlDic.Add(objName, new List<UIBehaviour>() { controls[i] });
        }
    }
    public Dictionary<string, List<UIBehaviour>> GetUIDictionary(){
        return controlDic;
    }
}
