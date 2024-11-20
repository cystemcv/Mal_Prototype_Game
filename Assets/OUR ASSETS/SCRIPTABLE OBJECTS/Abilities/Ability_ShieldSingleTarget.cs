using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Ability_ShieldSingleTarget", menuName = "CardAbility/Ability_ShieldSingleTarget")]
public class Ability_ShieldSingleTarget : ScriptableCardAbility
{
    [Header("UNIQUE")]
    public int empty;

    private GameObject realTarget;
    private GameObject entityUsedCard;


    public override string AbilityDescription(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity)
    {

        int cardShield = DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList);
        int calculatedShield = (entity == null) ? DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList) : Combat.Instance.CalculateEntityShield(DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList), entity, null);

        string keyword = base.AbilityDescription(cardScript, cardAbilityClass, entity);
        string description = "Add " + DeckManager.Instance.GetCalculatedValueString(cardShield, calculatedShield) + " shield to character";
        string final = keyword + description;

        return final;
    }



    public override void OnPlayCard(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity, SystemManager.ControlBy controlBy)
    {
        //assign target 
        if (SystemManager.ControlBy.PLAYER == controlBy)
        {
            realTarget = CombatCardHandler.Instance.targetClicked;
        }
        else //AI
        {
            List<GameObject> list = SystemManager.Instance.GetObjectsWithTagsFromGameobjectSameSide(entityUsedCard);

            int indexForTargetForCard = Random.Range(0, list.Count);
            //assign a target for card
            realTarget = list[indexForTargetForCard];
        }

        entityUsedCard = entity;

        base.OnPlayCard(cardScript, cardAbilityClass, entityUsedCard, controlBy);

        //spawn prefab
        base.SpawnEffectPrefab(realTarget, cardAbilityClass);


        int calculatedShield = (entity == null) ? DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList) : Combat.Instance.CalculateEntityShield(DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList), entity, null);
        Combat.Instance.AdjustTargetHealth(entityUsedCard, realTarget, calculatedShield, false, SystemManager.AdjustNumberModes.SHIELD);


    }


}
