using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff_Strength", menuName = "Buffs/Buff_Strength")]
public class Buff_Strength : ScriptableBuffDebuff
{
    //public override bool OnCharacterTurnStart(GameObject target)
    //{

    //    //activated
    //    return true;
    //}

    public override void OnExpireBuffDebuff(GameObject target)
    {
        //get the buff/debuff
        BuffDebuffClass buffDebuffClass = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(target, this.nameID);

        target.GetComponent<EntityClass>().attack -= buffDebuffClass.tempVariable;


    }

    public override void OnApplyBuff(GameObject target, int value, int turnsValue)
    {
        target.GetComponent<EntityClass>().attack += value;
    }
}
