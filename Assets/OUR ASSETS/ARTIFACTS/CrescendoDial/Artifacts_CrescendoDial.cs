using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Artifacts_CrescendoDial", menuName = "Item/Artifacts/Artifacts_CrescendoDial")]
public class Artifacts_CrescendoDial : ScriptableItem
{
    [Title("UNIQUE ITEM ABILITY")]
    public ScriptableBuffDebuff strengthAbility;
    public ScriptableBuffDebuff defenceAbility;

    public int strengthValue = 1;
    public int defenceValue = 1;
    public int healValue = 3;

    public int combo = 0;
    public int maxCombo = 2;

    public override void Activate(ClassItemData classItem, CardScript cardScript)
    {

        MonoBehaviour runner = CombatCardHandler.Instance;

        //if the cost of the card is not the same as the combo value then reset the combo back and stop this
        if (StaticData.artifact_CardScript.scriptableCard.primaryManaCost != combo)
        {

            if (combo > 0)
            {
                ItemManager.Instance.AddItemOnActivateOrder(this, this.itemName + " Failed Combo!", true);
            }

            //reset combo
            combo = 0;
            return;
        }

        if (StaticData.artifact_CardScript.scriptableCard.primaryManaCost == maxCombo)
        {


            ItemManager.Instance.AddItemOnActivateOrder(this, this.itemName + " Activated!",false);

            GameObject target = GameObject.FindGameObjectWithTag("Player");

            //increase strength
            EntityClass entityClass = BuffSystemManager.Instance.AddBuffDebuff(target, strengthAbility, strengthValue);
            entityClass.attack += strengthValue;

            //increase defence
            entityClass = BuffSystemManager.Instance.AddBuffDebuff(target, defenceAbility, defenceValue);
            entityClass.defence += defenceValue;

            //increase hp
            runner.StartCoroutine(Combat.Instance.AdjustTargetHealth(null, target, healValue, false, SystemManager.AdjustNumberModes.HEAL));

            //reset combo
            combo = 0;
        }
        else
        {
            //increase combo
            combo += 1;
        }





    }

    public override void Initialiaze(ClassItemData classItem, CardScript cardScript)
    {
        combo = 0;
    }


    public override void Expired(ClassItemData classItem, CardScript cardScript)
    {
        combo = 0;
    }




}
