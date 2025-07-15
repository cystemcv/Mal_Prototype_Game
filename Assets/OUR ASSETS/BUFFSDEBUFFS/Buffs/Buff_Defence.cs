using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff_Defence", menuName = "Buffs/Buff_Defence")]
public class Buff_Defence : ScriptableBuffDebuff
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

        target.GetComponent<EntityClass>().defence -= buffDebuffClass.tempVariable;


    }

    public override void OnApplyBuff(GameObject target, int value, int turnsValue)
    {
        target.GetComponent<EntityClass>().defence += value;
    }
}
