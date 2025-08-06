using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Debuff_Wet", menuName = "Debuffs/Debuff_Wet")]
public class Debuff_Wet : ScriptableBuffDebuff
{

    public ScriptableBuffDebuff burn;

    public override bool OnCharacterTurnStart(GameObject target)
    {
        Activate(target);
        return base.OnCharacterTurnStart(target);
    }

    public override bool OnEnemyTurnStart(GameObject target)
    {
        Activate(target);
        return base.OnEnemyTurnStart(target);
    }

    public void Activate(GameObject target)
    {
        MonoBehaviour runner = CombatCardHandler.Instance;
        //+1 damage on each stack
        BuffDebuffClass buffDebuffBurn = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(target, burn.nameID);

        //burn found
        if (buffDebuffBurn == null)
        {
            return;
        }

        BuffDebuffClass buffDebuffWet = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(target, this.nameID);

        //remove wet for burn
        if (buffDebuffWet == null)
        {
            return;
        }

        int burnValue = buffDebuffBurn.tempValue;
        int wetValue = buffDebuffWet.tempValue;
        int removeValue = 0;
        
        if(burnValue > wetValue)
        {
            removeValue = wetValue;
        }
        else if (burnValue < wetValue)
        {
            removeValue = burnValue;
        }
        else
        {
            removeValue = wetValue;
        }

        BuffSystemManager.Instance.DecreaseValueTargetBuffDebuff(target, burn.nameID, -1 * removeValue);
        BuffSystemManager.Instance.DecreaseValueTargetBuffDebuff(target, this.nameID, -1 * removeValue);
    }

}
