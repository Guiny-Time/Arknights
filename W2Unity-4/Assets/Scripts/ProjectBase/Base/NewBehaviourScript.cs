using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : SingletonAutoMono<NewBehaviourScript>
{
    //protected override void Awake()
    //{
    //    base.Awake();
    //}
    void Start()
    {
        Debug.Log(NewBehaviourScript.GetInstance().name);//打印对象的名字
    }
}
