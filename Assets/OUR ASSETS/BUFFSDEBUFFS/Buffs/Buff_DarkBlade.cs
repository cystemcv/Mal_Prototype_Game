using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff_DarkBlade", menuName = "Buffs/Buff_DarkBlade")]
public class Buff_DarkBlade : ScriptableBuffDebuff
{

    public ScriptableBuffDebuff strength;
    public int buffValue = 2;

    public override bool OnCharacterTurnStart(GameObject target)
    {
        Activate(target);

        return true;
    }

    public override bool OnEnemyTurnStart(GameObject target)
    {
        Activate(target);

        return true;
    }

    public void Activate(GameObject target)
    {

        BuffDebuffClass buffDebuff = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(target, this.nameID);

        int extra = buffDebuff.tempValue - 1;

        BuffSystemManager.Instance.AddBuffDebuff(target, strength, buffValue + extra);

    }

 

}
