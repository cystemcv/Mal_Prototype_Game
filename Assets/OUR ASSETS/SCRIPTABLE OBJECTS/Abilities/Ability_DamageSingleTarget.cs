using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Ability_DamageSingleTarget", menuName = "CardAbility/Ability_DamageSingleTarget")]
public class Ability_DamageSingleTarget : ScriptableCardAbility
{

    [Header("UNIQUE")]
    public int empty;

    private GameObject realTarget;

    public override string AbilityDescription(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity)
    {

            int cardDmg = DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList);
            int multiHits = DeckManager.Instance.GetIntValueFromList(1, cardAbilityClass.abilityIntValueList);
            string multiHitString = (multiHits > 0) ? multiHits + "x" : "";


            int calculatedDmg = Combat.Instance.CalculateEntityDmg(DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList), entity, null);


            string keyword = base.AbilityDescription(cardScript, cardAbilityClass, entity);
            string description = "Deal " + multiHitString + cardDmg + DeckManager.Instance.GetBonusAttackAsDescription(cardDmg, calculatedDmg) + " to an enemy";
            
            string final = keyword + description;

            return final;

    }



    public override void OnPlayCard(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity, GameObject target)
    {
        //assign target 
        if (target != null)
        {
            realTarget = target;
        }
        else
        {
            if (CombatCardHandler.Instance.targetClicked == null)
            {
                return;
            }
            else
            {
                realTarget = CombatCardHandler.Instance.targetClicked;
            }
        }

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
        base.OnPlayCard(cardScript, cardAbilityClass, entity, realTarget);
        ProceedToAbility(cardScript, cardAbilityClass, entity);

    }

    private void ProceedToAbility(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity)
    {
        //spawn prefab
        base.SpawnEffectPrefab(realTarget, cardAbilityClass);

        int calculatedDmg = Combat.Instance.CalculateEntityDmg(DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList), entity, realTarget);
        Combat.Instance.AdjustTargetHealth(realTarget, calculatedDmg, false, SystemManager.AdjustNumberModes.ATTACK);

    }


}
