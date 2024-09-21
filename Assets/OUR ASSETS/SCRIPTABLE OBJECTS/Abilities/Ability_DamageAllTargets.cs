using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Ability_DamageAllTargets", menuName = "CardAbility/Ability_DamageAllTargets")]
public class Ability_DamageAllTargets : ScriptableCardAbility
{

    [Header("UNIQUE")]
    public int empty;

    public override string AbilityDescription(CardScript cardScript, CardAbilityClass cardAbilityClass,GameObject entity)
    {

        int cardDmg = DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList);
        int calculatedDmg = Combat.Instance.CalculateEntityDmg(DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList), entity, null);
        int multiHits =  DeckManager.Instance.GetIntValueFromList(1, cardAbilityClass.abilityIntValueList);
        string multiHitString = (multiHits > 0) ? multiHits + "x" : "";

        string keyword = base.AbilityDescription(cardScript, cardAbilityClass, entity);
        string description = "Deal " + multiHitString + cardDmg + DeckManager.Instance.GetBonusAttackAsDescription(cardDmg, calculatedDmg) + " to all enemies";
        string final = keyword + description;

        return final;
    }



    public override void OnPlayCard(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity, GameObject target)
    {
     


        int multiHits = DeckManager.Instance.GetIntValueFromList(1, cardAbilityClass.abilityIntValueList);

        //then loop
        if (multiHits <= 0)
        {
            multiHits = 1;
        }

        MonoBehaviour runner = CombatCardHandler.Instance; // Ensure this is a valid MonoBehaviour in your scene
                                                           //hit at least one time if its 0

        // Start the coroutine for each hit
        runner.StartCoroutine(ExecuteMultiHits(cardScript, cardAbilityClass, entity, multiHits));



      


    }

    // Coroutine to handle multiple hits with delay
    private IEnumerator ExecuteMultiHits(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity, int multiHits)
    {
        for (int i = 0; i < multiHits; i++)
        {
            // Use ability on each hit
            UseAbility(cardScript, cardAbilityClass, entity);

            // Wait between hits
            yield return new WaitForSeconds(cardAbilityClass.waitForAbility / multiHits);
        }
    }


    public void UseAbility(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity)
    {
        base.OnPlayCard(cardScript, cardAbilityClass, entity, null);
        ProceedToAbility(cardScript, cardAbilityClass, entity);

    }

    private void ProceedToAbility(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity)
    {

        GameObject[] targetsFound;

        //get all targets
        if (entity.tag == "Player")
        {
            targetsFound = GameObject.FindGameObjectsWithTag("Enemy");
        }
        else
        {
            targetsFound = GameObject.FindGameObjectsWithTag("Player");
        }

        //then loop
        foreach (GameObject targetFound in targetsFound)
        {
            if (targetFound.GetComponent<EntityClass>().entityMode != SystemManager.EntityMode.DEAD)
            {

                //spawn prefab
                base.SpawnEffectPrefab(targetFound, cardAbilityClass);

                int calculatedDmg = Combat.Instance.CalculateEntityDmg(DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList), entity, targetFound);
                Combat.Instance.AdjustTargetHealth(targetFound, calculatedDmg, false, SystemManager.AdjustNumberModes.ATTACK);
            }
        }
    }

}
