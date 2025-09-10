using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster_Card_Shield", menuName = "Card/Monster/Monster_Card_Shield")]
public class Monster_Card_Shield : ScriptableCard
{

    public int shieldAmount = 0;

    private GameObject realTarget;

    private int shieldAmountScaling = 0;

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        //scaling
        shieldAmountScaling = shieldAmount + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        int calculatedValue = (Combat.Instance == null) ? shieldAmountScaling : Combat.Instance.CalculateEntityShield(shieldAmountScaling, entityUsedCard, realTarget);
        customDesc += "Shield " + DeckManager.Instance.GetCalculatedValueString(shieldAmountScaling, calculatedValue) + " to lowest health ally";

   
        return customDesc;
    }

    public override void OnPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnPlayCard(cardScriptData, entityUsedCard);
        ActivateCard(cardScriptData, entityUsedCard);
    }

    public override void OnAiPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayCard(cardScriptData, entityUsedCard);
        ActivateCard(cardScriptData, entityUsedCard);
    }

    public void ActivateCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        realTarget = AIManager.Instance.GetLowestHealthAlly(entityUsedCard);

        if (realTarget == null)
        {
            return;
        }

        MonoBehaviour runner = CombatCardHandler.Instance;

        //scaling
        shieldAmountScaling = shieldAmount + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        int calculatedValue = (Combat.Instance == null) ? shieldAmountScaling : Combat.Instance.CalculateEntityShield(shieldAmountScaling, entityUsedCard, realTarget);
        runner.StartCoroutine(Combat.Instance.AdjustTargetHealth(entityUsedCard, realTarget, calculatedValue, false, SystemManager.AdjustNumberModes.HEAL));
    }

    public override void OnAiPlayTarget(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        //base.OnAiPlayTarget(cardScriptData, entityUsedCard);

        //realTarget = AIManager.Instance.GetLowestHealthAlly(entityUsedCard);

        //entityUsedCard.GetComponent<AIBrain>().targetForCard = realTarget;

    }


}
