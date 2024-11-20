using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Ability_DamagePierceSingleTarget", menuName = "CardAbility/Ability_DamagePierceSingleTarget")]
public class Ability_DamagePierceSingleTarget : ScriptableCardAbility
{

    [Header("UNIQUE")]
    public int empty;

    private GameObject realTarget;

    public override string AbilityDescription(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity)
    {
        int cardDmg = DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList);

        int calculatedDmg = (entity == null) ? DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList) : Combat.Instance.CalculateEntityDmg(DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList), entity, null);

        string keyword = base.AbilityDescription(cardScript, cardAbilityClass, entity);
        string description = "Deal " + DeckManager.Instance.GetCalculatedValueString(cardDmg, calculatedDmg) + " to an enemy (Ignore Block)";
        string final = keyword + " : " + description;

        return final;
    }



    public override void OnPlayCard(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity, SystemManager.ControlBy controlBy)
    {

        //assign target 

            realTarget = CombatCardHandler.Instance.targetClicked;
      
        base.OnPlayCard(cardScript, cardAbilityClass, entity, controlBy);



            ProceedToAbility(cardScript, cardAbilityClass, entity);


    }


    private void ProceedToAbility(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity)
    {
        int calculatedDmg = Combat.Instance.CalculateEntityDmg(DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList), entity, realTarget);
        Combat.Instance.AdjustTargetHealth(entity,realTarget, calculatedDmg, true, SystemManager.AdjustNumberModes.ATTACK);

    }


}
