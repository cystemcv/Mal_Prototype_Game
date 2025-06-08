using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public class ScriptableButtonEvent : ScriptableObject
{


    public string eventButtonDescription = "";
    public virtual void OnButtonCreate(GameObject eventButton)
    {

    }

    public virtual void OnButtonClick(GameObject eventButton)
    {

    }


}
