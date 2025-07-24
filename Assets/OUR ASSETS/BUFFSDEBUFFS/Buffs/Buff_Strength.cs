using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff_Strength", menuName = "Buffs/Buff_Strength")]
public class Buff_Strength : ScriptableBuffDebuff
{


    public override void OnApplyBuff(GameObject target, int value)
    {
       
    }

    public override StatModifiedValue? OnModifyStats(GameObject caster, GameObject target, ScriptableCard scriptableCard)
    {

        BuffDebuffClass buffDebuffClass = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(caster, "Strength");

        StatModifiedValue statModifiedValue = new StatModifiedValue();
        statModifiedValue.StatModifiedAttribute = SystemManager.StatModifiedAttribute.ATTACK;
        statModifiedValue.statModifiedType = SystemManager.StatModifiedType.NORMAL;
        statModifiedValue.statIncreaseInt = buffDebuffClass.tempVariable;
        statModifiedValue.statIncreaseFloat = 0;

        return statModifiedValue;
    }
}
