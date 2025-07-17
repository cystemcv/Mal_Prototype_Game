using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff_Defence", menuName = "Buffs/Buff_Defence")]
public class Buff_Defence : ScriptableBuffDebuff
{

    public override StatModifiedValue? OnModifyStats(GameObject caster, GameObject target, ScriptableCard scriptableCard)
    {

        BuffDebuffClass buffDebuffClass = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(caster, "Defence");

        StatModifiedValue statModifiedValue = new StatModifiedValue();
        statModifiedValue.StatModifiedAttribute = SystemManager.StatModifiedAttribute.DEFENCE;
        statModifiedValue.statModifiedType = SystemManager.StatModifiedType.NORMAL;
        statModifiedValue.statIncreaseInt = buffDebuffClass.tempVariable;
        statModifiedValue.statIncreaseFloat = 0;

        return statModifiedValue;
    }
}
