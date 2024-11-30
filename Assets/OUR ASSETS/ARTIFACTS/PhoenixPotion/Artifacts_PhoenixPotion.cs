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

    public override void Activate(ClassItem classItem)
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

        triggered = true;

        ItemManager.Instance.AddItemOnActivateOrder(this, this.itemName + " Activated!", false);

        Combat.Instance.AdjustTargetHealth(null, character, 
            (character.GetComponent<EntityClass>().maxHealth * hpToHealPerc) / 100
            , false, SystemManager.AdjustNumberModes.HEAL);


    }

    public override void Initialiaze(ClassItem classItem)
    {
    
    }

    public override void Expired(ClassItem classItem)
    {
 
    }

    public override void GameStart()
    {
        triggered = false;
    }


}
