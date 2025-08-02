using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Artifacts_PhoenixPotion", menuName = "Item/Artifacts/Artifacts_PhoenixPotion")]
public class Artifacts_PhoenixPotion : ScriptableItem
{
    [Title("UNIQUE ITEM ABILITY")]
    public int hpToHealPerc = 30;
    public int activateAtHP = 0;
    
    private bool triggered = false;

    public override void Activate(ClassItemData classItem, CardScriptData cardScriptData)
    {

        if (triggered)
        {
            return;
        }

        GameObject character = GameObject.FindGameObjectWithTag("Player");

        if ( character.GetComponent<EntityClass>().health > activateAtHP)
        {
            return;
        }

        MonoBehaviour runner = CombatCardHandler.Instance;

        triggered = true;

        ItemManager.Instance.AddItemOnActivateOrder(this, this.itemName + " Activated!", false);

        runner.StartCoroutine(Combat.Instance.AdjustTargetHealth(null, character, 
            (character.GetComponent<EntityClass>().maxHealth * hpToHealPerc) / 100
            , false, SystemManager.AdjustNumberModes.HEAL));


    }

    public override void Initialiaze(ClassItemData classItem, CardScriptData cardScriptData)
    {
    
    }

    public override void Expired(ClassItemData classItem, CardScriptData cardScriptData)
    {
 
    }

    public override void GameStart()
    {
        triggered = false;
    }


}
