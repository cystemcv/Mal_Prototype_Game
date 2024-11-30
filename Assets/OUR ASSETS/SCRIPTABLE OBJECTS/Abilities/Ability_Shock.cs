using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Ability_Shock", menuName = "CardAbility/Ability_Shock")]
public class Ability_Shock : ScriptableCardAbility
{

    private GameObject entityUsedCard;
    private GameObject realTarget;
    private int multiHits = 0;
    private CardScript cardScript;
    private CardAbilityClass cardAbilityClass;
    private SystemManager.ControlBy controlBy;

    public ScriptableBuffDebuff scriptableBuffDebuff;

    public override string AbilityDescription(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity)
    {

        int cardDmg = DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList);

        int calculatedDmg = (entity == null) ? DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList) : Combat.Instance.CalculateEntityDmg(DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList), entity, null);


        string keyword = base.AbilityDescription(cardScript, cardAbilityClass, entity);
        string description = "Deal " + DeckManager.Instance.GetCalculatedValueString(cardDmg, calculatedDmg) + " to an enemy <color=yellow>Shock</color>";

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
            if (aIBrain.targetForCard == null)
            {
                aIBrain.ReAssignTargetForCard();
            }

            realTarget = aIBrain.targetForCard;
        }

        //stop card if there is no target
        if (realTarget == null)
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

        bool hitWetTarget = false;


        //check if buff exist on target
        BuffDebuffClass buffDebuffClass = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(realTarget, scriptableBuffDebuff.nameID);

        if (buffDebuffClass != null)
        {
            hitWetTarget = true;
        }

        // Use ability on each hit
        base.OnPlayCard(cardScript, cardAbilityClass, entityUsedCard, controlBy);

        base.SpawnEffectPrefab(realTarget, cardAbilityClass);

        int calculatedDmg = Combat.Instance.CalculateEntityDmg(DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList), entityUsedCard, realTarget);
        Combat.Instance.AdjustTargetHealth(entityUsedCard, realTarget, calculatedDmg, false, SystemManager.AdjustNumberModes.ATTACK);

        // Wait between hits
        yield return new WaitForSeconds(cardAbilityClass.waitForAbility);


        if (hitWetTarget)
        {

            //then go throught all other enemies with debuff and strike them too

            // Use ability on each hit
            base.OnPlayCard(cardScript, cardAbilityClass, entityUsedCard, controlBy);
            List<GameObject> targetsFound = new List<GameObject>();

            //get all targets
            if (SystemManager.Instance.GetPlayerTagsList().Contains(this.entityUsedCard.tag))
            {
                targetsFound = SystemManager.Instance.FindGameObjectsWithTags(SystemManager.Instance.GetEnemyTagsList());
            }
            else
            {
                targetsFound = SystemManager.Instance.FindGameObjectsWithTags(SystemManager.Instance.GetPlayerTagsList());
            }

            //then loop
            foreach (GameObject targetFound in targetsFound)
            {
                if (targetFound == null)
                {
                    continue;
                }


                if (targetFound.GetComponent<EntityClass>().entityMode != SystemManager.EntityMode.DEAD)
                {


                    BuffDebuffClass buffDebuff = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(targetFound, scriptableBuffDebuff.nameID);

                    if (buffDebuff != null)
                    {

                        int count = buffDebuff.tempVariable;

                        while (count > 0 && targetFound != null)
                        {

                            //spawn prefab
                            base.SpawnEffectPrefab(targetFound, cardAbilityClass);


                            calculatedDmg = Combat.Instance.CalculateEntityDmg(DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList), entityUsedCard, targetFound);
                            Combat.Instance.AdjustTargetHealth(entityUsedCard, targetFound, calculatedDmg, false, SystemManager.AdjustNumberModes.ATTACK);

                            count = count - 1;
                            //remove the debuff
                            BuffSystemManager.Instance.DecreaseValueTargetBuffDebuff(targetFound, scriptableBuffDebuff.nameID, -1);

                            // Wait between hits
                            yield return new WaitForSeconds(cardAbilityClass.waitForAbility);

                        }


                    }


                }






            }



        }
    }






}
