using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Artifacts_QuickSnack", menuName = "Item/Artifacts/Artifacts_QuickSnack")]
public class Artifacts_QuickSnack : ScriptableItem
{
    [Title("UNIQUE ITEM ABILITY")]
    public int hpToHeal = 6;
    public int turnsToHeal = 1;
    
    private bool triggered = false;

    public override void Activate(ClassItem classItem, CardScript cardScript)
    {

        if (Combat.Instance.turns > 1 && !triggered)
        {
            return;
        }

        triggered = true;

        ItemManager.Instance.AddItemOnActivateOrder(this, this.itemName + " Activated!", false);

        GameObject character = GameObject.FindGameObjectWithTag("Player");

        MonoBehaviour runner = CombatCardHandler.Instance;

        runner.StartCoroutine(Combat.Instance.AdjustTargetHealth(null, character, hpToHeal, false, SystemManager.AdjustNumberModes.HEAL));


    }

    public override void Expired(ClassItem classItem, CardScript cardScript)
    {
        ItemManager.Instance.AddItemOnActivateOrder(this, this.itemName + " Expired!", true);
        triggered = false;
    }




}
