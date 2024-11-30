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
    public Sprite companionImage;
    public GameObject companionChoose;
    [Title("ABILITY")]
    public string companionAbilityName = "";
    public int companionAbilityCD = 0;
    public int startingAbilityCd = 0;
    [Title("UPGRADES")]
    public List<ScriptableItem> companionItemList;


    public virtual void OnPlayCard()
    {

    }


    public virtual void OnabilityActivate()
    {
        GameObject companionBtn = GameObject.FindGameObjectWithTag("CompanionButton");

        if (companionBtn == null)
        {
            return;
        }

        companionBtn.GetComponent<UI_CombatButton>().activatedButton = false;

        if (companionBtn.GetComponent<UI_CombatButton>().cdButton > 0)
        {
            return;
        }

        companionBtn.GetComponent<UI_CombatButton>().activatedButton = true;

        //put the button on cd
        companionBtn.GetComponent<UI_CombatButton>().cdButton = this.companionAbilityCD - Combat.Instance.reduceCompanionAbilityCd;

        companionBtn.GetComponent<UI_CombatButton>().UpdateButton();


    }

    public virtual void InitializeButton()
    {
        GameObject companionBtn = GameObject.FindGameObjectWithTag("CompanionButton");
        companionBtn.GetComponent<UI_CombatButton>().textSaved = companionAbilityName;

        if (startingAbilityCd > 0)
        {
            //put the button on cd
            companionBtn.GetComponent<UI_CombatButton>().cdButton = startingAbilityCd;
        }

    }


    public ScriptableCompanion Clone()
    {
        ScriptableCompanion clone = ScriptableObject.Instantiate(this);
        //clone.abilities = new List<string>(this.abilities); // Deep copy of the list
        return clone;
    }



}
