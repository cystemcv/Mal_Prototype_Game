using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Debuff_Weaken", menuName = "Debuffs/Debuff_Weaken")]
public class Debuff_Weaken : ScriptableBuffDebuff
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
        statModifiedValue.statModifiedType = SystemManager.StatModifiedType.TARGETPERCENTAGE;
        statModifiedValue.statIncreaseInt = 0;
        statModifiedValue.statIncreaseFloat = 50;

        return statModifiedValue;
    }
}
