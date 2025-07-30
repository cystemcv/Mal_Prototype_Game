using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff_SuperArmor", menuName = "Buffs/Buff_SuperArmor")]
public class Buff_SuperArmor : ScriptableBuffDebuff
{


    public override void OnApplyBuff(GameObject target, int value)
    {
       
    }

    public override StatModifiedValue? OnModifyStats_Target(GameObject caster, GameObject target, ScriptableCard scriptableCard)
    {

        StatModifiedValue statModifiedValue = new StatModifiedValue();
        statModifiedValue.StatModifiedAttribute = SystemManager.StatModifiedAttribute.ATTACK;
        statModifiedValue.statModifiedType = SystemManager.StatModifiedType.TARGETFIXEDAMOUNT;
        statModifiedValue.statIncreaseInt = 1;
        statModifiedValue.statIncreaseFloat = 0;

        return statModifiedValue;
    }
}
