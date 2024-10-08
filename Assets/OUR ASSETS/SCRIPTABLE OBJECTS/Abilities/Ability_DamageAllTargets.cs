using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Ability_DamageAllTargets", menuName = "CardAbility/Ability_DamageAllTargets")]
public class Ability_DamageAllTargets : ScriptableCardAbility
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
        int calculatedDmg = Combat.Instance.CalculateEntityDmg(DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList), entity, null);
        int multiHits = DeckManager.Instance.GetIntValueFromList(1, cardAbilityClass.abilityIntValueList);
        string multiHitString = (multiHits > 0) ? multiHits + "x" : "";

        string keyword = base.AbilityDescription(cardScript, cardAbilityClass, entity);
        string description = "Deal " + multiHitString + cardDmg + DeckManager.Instance.GetBonusAttackAsDescription(cardDmg, calculatedDmg) + " to all enemies";
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



        multiHits = DeckManager.Instance.GetIntValueFromList(1, cardAbilityClass.abilityIntValueList);

        //then loop
        if (multiHits <= 0)
        {
            multiHits = 1;
        }

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

                    //spawn prefab
                    base.SpawnEffectPrefab(targetFound, cardAbilityClass);

                    int calculatedDmg = Combat.Instance.CalculateEntityDmg(DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList), entityUsedCard, targetFound);
                    Combat.Instance.AdjustTargetHealth(targetFound, calculatedDmg, false, SystemManager.AdjustNumberModes.ATTACK);
                }
            }

            // Wait between hits
            yield return new WaitForSeconds(cardAbilityClass.waitForAbility / multiHits);
        }
    }





}
