using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Artifacts_ManaOrb", menuName = "Item/Artifacts/Artifacts_ManaOrb")]
public class Artifacts_ManaOrb : ScriptableItem
{
    [Title("UNIQUE ITEM ABILITY")]
    public int extraMana = 1;
    public int expireOnTurn = 2;

    public override void Activate(ClassItemData classItem, CardScriptData cardScriptData, GameObject target)
    {

        ItemManager.Instance.AddItemOnActivateOrder(this, "+" + extraMana + " mana", false);

        Combat.Instance.manaMaxAvailable += 1;
    }

    public override void Initialiaze(ClassItemData classItem, CardScriptData cardScriptData, GameObject target)
    {
   
    }

    public override void Expired(ClassItemData classItem, CardScriptData cardScriptData, GameObject target)
    {

        if (Combat.Instance.turns == expireOnTurn)
        {
            ItemManager.Instance.AddItemOnActivateOrder(this, "-" + extraMana + " mana", true);
            Combat.Instance.manaMaxAvailable -= 1;
            Combat.Instance.ManaAvailable = 0;
        }

    }




}
