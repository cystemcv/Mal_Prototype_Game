using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Artifacts_SummoningStaff", menuName = "Item/Artifacts/Artifacts_SummoningStaff")]
public class Artifacts_SummoningStaff : ScriptableItem
{
    [Title("UNIQUE ITEM ABILITY")]
    public ScriptableBuffDebuff scriptableBuffDebuff;
    public int amount = 1;

    public override void Activate(ClassItemData classItem, CardScriptData cardScriptData, GameObject target)
    {

        if (target == null)
        {
            return;
        }

        if (target.tag != "PlayerSummon")
        {
            return;
        }

        BuffSystemManager.Instance.AddBuffDebuff(target, scriptableBuffDebuff, amount);
        ItemManager.Instance.AddItemOnActivateOrder(this, "Added " + amount + " <color=yellow>" + scriptableBuffDebuff.nameID + "</color> to summon!", false);
    }

}
