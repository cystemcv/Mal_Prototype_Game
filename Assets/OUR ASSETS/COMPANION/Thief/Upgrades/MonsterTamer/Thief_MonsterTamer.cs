using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Thief_MonsterTamer", menuName = "Item/CompanionPassives/Thief_MonsterTamer")]
public class Thief_MonsterTamer : ScriptableItem
{
    [Title("UNIQUE ITEM ABILITY")]
    public int attackBoost = 2; 

    public override void Activate(ClassItemData classItem, CardScriptData cardScriptData, GameObject target)
    {
        Combat.Instance.tempMonsterAttackBoost = attackBoost * classItem.level;
    }

    public override void Expired(ClassItemData classItem, CardScriptData cardScriptData, GameObject target)
    {
        Combat.Instance.tempMonsterAttackBoost = 0;
    }


}
