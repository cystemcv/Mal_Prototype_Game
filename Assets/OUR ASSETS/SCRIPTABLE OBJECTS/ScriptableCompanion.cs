using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using static BuffSystemManager;
using static ScriptableCard;


public class ScriptableCompanion : ScriptableObject
{

    [Title("MAIN")]
    public string companionName;
    [TextArea] public string companionDescription;
    public GameObject companionPrefab;
    public int companionAbilityCD = 0;
    public Sprite companionImage;
    public GameObject companionChoose;

    public List<ScriptableItem> companionItemList;


    public virtual void OnPlayCard()
    {

    }


    public virtual void OnabilityActivate()
    {



    }


    public ScriptableCompanion Clone()
    {
        ScriptableCompanion clone = ScriptableObject.Instantiate(this);
        //clone.abilities = new List<string>(this.abilities); // Deep copy of the list
        return clone;
    }



}
