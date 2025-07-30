using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Debuff_Frail", menuName = "Debuffs/Debuff_Frail")]
public class Debuff_Frail : ScriptableBuffDebuff
{
    public override bool OnCharacterTurnStart(GameObject target)
    {

        //activated
        return true;
    }

    public override StatModifiedValue? OnModifyStats(GameObject caster, GameObject target, ScriptableCard scriptableCard)
    {

        StatModifiedValue statModifiedValue = new StatModifiedValue();
        statModifiedValue.StatModifiedAttribute = SystemManager.StatModifiedAttribute.DEFENCE;
        statModifiedValue.statModifiedType = SystemManager.StatModifiedType.PERCENTAGE;
        statModifiedValue.statIncreaseInt = 0;
        statModifiedValue.statIncreaseFloat = -25;

        return statModifiedValue;
    }
}
