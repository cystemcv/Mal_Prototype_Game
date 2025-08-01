using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff_TemporaryStrength", menuName = "Buffs/Buff_TemporaryStrength")]
public class Buff_TemporaryStrength : ScriptableBuffDebuff
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

        target.GetComponent<EntityClass>().attack -= buffDebuffClass.tempValue;


    }

    public override void OnApplyBuff(GameObject target, int value)
    {
        target.GetComponent<EntityClass>().attack += value;
    }
}
