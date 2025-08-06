using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Debuff_Burn", menuName = "Debuffs/Debuff_Burn")]
public class Debuff_Burn : ScriptableBuffDebuff
{

    public int burnDamage = 0;

    public override bool OnCharacterTurnEnd(GameObject target)
    {
        Activate(target);
        return base.OnCharacterTurnEnd(target);
    }

    public override bool OnEnemyTurnEnd(GameObject target)
    {
        Activate(target);
        return base.OnEnemyTurnEnd(target);
    }

    public void Activate(GameObject target)
    {
        MonoBehaviour runner = CombatCardHandler.Instance;
        //+1 damage on each stack
        BuffDebuffClass buffDebuff = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(target, this.nameID);

        int extraBurn = buffDebuff.tempValue;

        //deal damage each turn
        runner.StartCoroutine(Combat.Instance.AdjustTargetHealth(null, target, burnDamage + extraBurn, false, SystemManager.AdjustNumberModes.COUNTER));
    }

}
