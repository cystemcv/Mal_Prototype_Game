using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Artifacts_CounterGloves", menuName = "Item/Artifacts/Artifacts_CounterGloves")]
public class Artifacts_CounterGloves : ScriptableItem
{
    [Title("UNIQUE ITEM ABILITY")]
    public int counterAmount = 1;
    public ScriptableBuffDebuff counter;

    public override void Activate(ClassItemData classItem, CardScript cardScript)
    {
        GameObject character = GameObject.FindGameObjectWithTag("Player");

        BuffSystemManager.Instance.AddBuffDebuff(character, counter, counterAmount);

        ItemManager.Instance.AddItemOnActivateOrder(this, this.itemName + " Activated!", false);
    }




}
