using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Debuff_Burn", menuName = "Debuffs/Debuff_Burn")]
public class Debuff_Burn : ScriptableBuffDebuff
{

    public int burnDamage = 0;

    public override bool OnCharacterTurnStart(GameObject target)
    {

        //+1 damage on each stack
        BuffDebuffClass buffDebuff = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(target, this.nameID);

        int extraBurn = buffDebuff.tempVariable;

        //deal damage each turn
        Combat.Instance.AdjustTargetHealth(null, target, burnDamage + extraBurn, false, SystemManager.AdjustNumberModes.ATTACK);

        return base.OnCharacterTurnStart(target);

    }

}
