using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff_Berserk", menuName = "Buffs/Buff_Berserk")]
public class Buff_Berserk : ScriptableBuffDebuff
{
    public override bool OnCharacterTurnStart(GameObject target)
    {

        //activated
        return true;
    }

    public override StatModifiedValue? OnModifyStats(GameObject caster, GameObject target, ScriptableCard scriptableCard)
    {

        StatModifiedValue statModifiedValue = new StatModifiedValue();
        statModifiedValue.StatModifiedAttribute = SystemManager.StatModifiedAttribute.ATTACK;
        statModifiedValue.statModifiedType = SystemManager.StatModifiedType.PERCENTAGE;
        statModifiedValue.statIncreaseInt = 0;
        statModifiedValue.statIncreaseFloat = 200;

        return statModifiedValue;
    }
}
