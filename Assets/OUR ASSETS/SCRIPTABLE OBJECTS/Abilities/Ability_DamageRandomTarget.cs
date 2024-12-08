using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Ability_DamageRandomTarget", menuName = "CardAbility/Ability_DamageRandomTarget")]
public class Ability_DamageRandomTarget : ScriptableCardAbility
{

    [Header("UNIQUE")]
    private GameObject targetFound;
    private GameObject entityUsedCard;

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
        string description = "Deal " + multiHitString + DeckManager.Instance.GetCalculatedValueString(cardDmg, calculatedDmg) + " randomly";

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

            int randomTarget = UnityEngine.Random.Range(0, targetsFound.Count);

            //then loop

            targetFound = targetsFound[randomTarget];

            if (targetFound == null)
            {
                continue;
            }


            if (targetFound.GetComponent<EntityClass>().entityMode != SystemManager.EntityMode.DEAD)
            {

                //spawn prefab
                base.SpawnEffectPrefab(targetFound, cardAbilityClass);

                int calculatedDmg = Combat.Instance.CalculateEntityDmg(DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList), entityUsedCard, targetFound);
                Combat.Instance.AdjustTargetHealth(entityUsedCard, targetFound, calculatedDmg, false, SystemManager.AdjustNumberModes.ATTACK);
            }

            // Wait between hits
            yield return new WaitForSeconds(cardAbilityClass.waitForAbility / multiHits);
        }
    }
}
