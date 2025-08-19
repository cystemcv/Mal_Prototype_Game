using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Artifacts_SwiftBoots", menuName = "Item/Artifacts/Artifacts_SwiftBoots")]
public class Artifacts_SwiftBoots : ScriptableItem
{
    [Title("UNIQUE ITEM ABILITY")]
    public int extraMana = 1;

    public override void Expired(ClassItemData classItem, CardScriptData cardScriptData, GameObject target)
    {
        CombatCardHandler.Instance.moveExtraMana -= extraMana;

        if (CombatCardHandler.Instance.moveExtraMana <= 0)
        {
            CombatCardHandler.Instance.moveExtraMana = 0;
        }
    }


    public override void Initialiaze(ClassItemData classItem, CardScriptData cardScriptData, GameObject target)
    {
        CombatCardHandler.Instance.moveExtraMana += extraMana;
        ItemManager.Instance.AddItemOnActivateOrder(this, "Recharged", false);
    }




}
