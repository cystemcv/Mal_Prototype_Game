using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Ability_DamageSingleTarget", menuName = "CardAbility/Ability_DamageSingleTarget")]
public class Ability_DamageSingleTarget : ScriptableCardAbility
{

    private GameObject entityUsedCard;
    private GameObject realTarget;
    private int multiHits = 0;
    private CardScript cardScript;
    private CardAbilityClass cardAbilityClass;
    private SystemManager.ControlBy controlBy;



    public override string AbilityDescription(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity)
    {

        int cardDmg = DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList);
        int multiHits = DeckManager.Instance.GetIntValueFromList(1, cardAbilityClass.abilityIntValueList);
        string multiHitString = (multiHits > 0) ? multiHits + "x" : "";


        int calculatedDmg = (entity == null) ? DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList) : Combat.Instance.CalculateEntityDmg(DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList), entity, null);


        string keyword = base.AbilityDescription(cardScript, cardAbilityClass, entity);
        string description = "Deal " + multiHitString + DeckManager.Instance.GetCalculatedValueString(cardDmg, calculatedDmg) + " to an enemy";

        string final = keyword + description;

        return final;

    }



    public override void OnPlayCard(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entityUsedCard, SystemManager.ControlBy controlBy)
    {
        //assign them to have them globally
        this.cardScript = cardScript;
        this.cardAbilityClass = cardAbilityClass;
        this.entityUsedCard = entityUsedCard;
        this.controlBy = controlBy;

        //get the target 
        if (SystemManager.ControlBy.PLAYER == controlBy)
        {
            realTarget = CombatCardHandler.Instance.targetClicked;
        }
        else
        {
            AIBrain aIBrain = entityUsedCard.GetComponent<AIBrain>();

            //get target, if target dies then assign to new target
     

            realTarget = aIBrain.targetForCard;
        }

        //stop card if there is no target
        if(realTarget == null)
        {
            return;
        }

        //get how many multihits it will activate
        multiHits = DeckManager.Instance.GetIntValueFromList(1, cardAbilityClass.abilityIntValueList);

        //then loop
        if (multiHits <= 0)
        {
            multiHits = 1;
        }

        //allow to activate coroutine on scriptable object
        MonoBehaviour runner = CombatCardHandler.Instance; // Ensure this is a valid MonoBehaviour in your scene
                                                           //hit at least one time if its 0

        // Start the coroutine for each hit
        runner.StartCoroutine(ExecuteMultiHits());





    }

    // Coroutine to handle multiple hits with delay
    private IEnumerator ExecuteMultiHits()
    {
        for (int i = 0; i < multiHits; i++)
        {

            //if target dies during multi attack then stop
            if (realTarget == null)
            {
                break;
            }

            // Use ability on each hit
            base.OnPlayCard(cardScript, cardAbilityClass, entityUsedCard, controlBy);

            base.SpawnEffectPrefab(realTarget, cardAbilityClass);

            int calculatedDmg = Combat.Instance.CalculateEntityDmg(DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList), entityUsedCard, realTarget);
            Combat.Instance.AdjustTargetHealth(entityUsedCard, realTarget, calculatedDmg, false, SystemManager.AdjustNumberModes.ATTACK);

            // Wait between hits
            yield return new WaitForSeconds(cardAbilityClass.waitForAbility);
        }
    }






}
