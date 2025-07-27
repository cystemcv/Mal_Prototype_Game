using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Hazard_Mine", menuName = "Hazard/Hazard_Mine")]
public class Hazard_Mine : ScriptableHazard
{
    public int mineDamage = 20;

    public override IEnumerator OnTurnStart(CombatPosition combatPosition)
    {
        if (combatPosition.entityOccupiedPos != null)
        {

            //explode
            MonoBehaviour runner = CombatCardHandler.Instance;

            yield return runner.StartCoroutine(base.OnTurnStart(combatPosition));

            yield return runner.StartCoroutine(Combat.Instance.RemoveHazardIE(this, combatPosition));

            // Start the coroutine for each hit
            yield return runner.StartCoroutine(Combat.Instance.AttackSingleTargetEnemy(null, mineDamage, 1, null, combatPosition.entityOccupiedPos, 1, 1));

            //do explode effect




            yield return null;

        }

    }
}
