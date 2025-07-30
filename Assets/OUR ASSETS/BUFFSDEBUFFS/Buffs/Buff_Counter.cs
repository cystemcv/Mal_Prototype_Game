using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff_Counter", menuName = "Buffs/Buff_Counter")]
public class Buff_Counter : ScriptableBuffDebuff
{

    public override void OnGettingHit(GameObject caster, GameObject target, int value, int turnsValue)
    {
        base.OnGettingHit(caster, target, value, turnsValue);

        //get the value of the buff then hit the target
        BuffDebuffClass buffDebuffClass = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(target, this.nameID);
        MonoBehaviour runner = CombatCardHandler.Instance;

        Debug.Log("caster : " + caster.name + " | " + buffDebuffClass.tempValue);
        Debug.Log("target : " + target.name + " | " + buffDebuffClass.tempValue);

        runner.StartCoroutine(Combat.Instance.AdjustTargetHealth(target, caster, buffDebuffClass.tempValue, false,SystemManager.AdjustNumberModes.COUNTER));

    }


}
