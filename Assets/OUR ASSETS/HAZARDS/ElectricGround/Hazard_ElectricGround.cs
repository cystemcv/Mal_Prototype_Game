using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Hazard_ElectricGround", menuName = "Hazard/Hazard_ElectricGround")]
public class Hazard_ElectricGround : ScriptableHazard
{
    public ScriptableBuffDebuff wet;
    public int damage = 5;

    public override IEnumerator OnTurnStart(CombatPosition combatPosition)
    {


        if (combatPosition.entityOccupiedPos != null)
        {

            PlayEffects(combatPosition);

            //explode
            MonoBehaviour runner = CombatCardHandler.Instance;

            yield return runner.StartCoroutine(base.OnTurnStart(combatPosition));

            //yield return runner.StartCoroutine(Combat.Instance.RemoveHazardIE(this, combatPosition));

            int finalDmg = damage;
            //check if its wet
            //get the buff
            BuffDebuffClass buffDebuffClass = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(combatPosition.entityOccupiedPos, wet.nameID);
            if (!SystemManager.Instance.CheckNullMonobehavior(buffDebuffClass))
            {
                finalDmg = damage * 2;
                //decrease by 1
                BuffSystemManager.Instance.DecreaseValueTargetBuffDebuff(combatPosition.entityOccupiedPos, wet.nameID, -1);
            }

            yield return runner.StartCoroutine(Combat.Instance.AttackSingleTargetEnemy(null, finalDmg, 1, null, combatPosition.entityOccupiedPos, 1, 1));

            //do explode effect
            yield return null;

        }

    }
}
