using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Artifacts_RelentlessFire", menuName = "Item/Artifacts/Artifacts_RelentlessFire")]
public class Artifacts_RelentlessFire : ScriptableItem
{
    [Title("UNIQUE ITEM ABILITY")]
    public ScriptableBuffDebuff strengthAbility;
    public ScriptableBuffDebuff defenceAbility;

    private int relentless = 0;
    public int maxRelentless = 2;

    public override void Activate(ClassItemData classItem, CardScript cardScript)
    {

        relentless += 1;

        if (relentless > maxRelentless)
        {
            relentless = maxRelentless;
        }

    }

    public override void Initialiaze(ClassItemData classItem, CardScript cardScript)
    {
        if (relentless == 0)
        {
            return;
        }

        if (relentless > maxRelentless)
        {
            relentless = maxRelentless;
        }

        ItemManager.Instance.AddItemOnActivateOrder(this, this.itemName + " Activated! Relentless : " + relentless , false);

        GameObject target = GameObject.FindGameObjectWithTag("Player");

        //increase strength
        EntityClass entityClass = BuffSystemManager.Instance.AddBuffDebuff(target, strengthAbility, relentless, 0);
        entityClass.attack += relentless;

        //increase defence
        entityClass = BuffSystemManager.Instance.AddBuffDebuff(target, defenceAbility, relentless, 0);
        entityClass.defence += relentless;

    }


    public override void Expired(ClassItemData classItem, CardScript cardScript)
    {
        if (relentless > 0)
        {
            relentless = 0;
            ItemManager.Instance.AddItemOnActivateOrder(this, this.itemName + " All stacks lost! Relentless : " + relentless, true);
        }
   

    }




}
