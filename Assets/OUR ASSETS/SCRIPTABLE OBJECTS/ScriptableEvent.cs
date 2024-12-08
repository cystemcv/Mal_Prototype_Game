using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "ScriptableEvent", menuName = "Events/ScriptableEvent")]
public class ScriptableEvent : ScriptableObject
{

    public Sprite icon;
    public string title;
    public string description;


    [Serializable]
    public class EventButtonClass
    {

        public string buttonName;
        public ScriptableButtonEvent scriptableButtonEvent;

    }


}
