using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff_TemporaryDefence", menuName = "Buffs/Buff_TemporaryDefence")]
public class Buff_TemporaryDefence : ScriptableBuffDebuff
{
    public override bool OnCharacterTurnStart(GameObject target)
    {

        //activated
        return true;
    }

    public override void OnExpireBuffDebuff(GameObject target)
    {
        //get the buff/debuff
        BuffDebuffClass buffDebuffClass = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(target, this.nameID);

        target.GetComponent<EntityClass>().defence -= buffDebuffClass.tempValue;


    }

    public override void OnApplyBuff(GameObject target, int value)
    {
        target.GetComponent<EntityClass>().defence += value;
    }
}
