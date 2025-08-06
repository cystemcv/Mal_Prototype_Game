using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Angel_Card_RighteousShield", menuName = "Card/Angel/Angel_Card_RighteousShield")]
public class Angel_Card_RighteousShield : ScriptableCard
{

    public int shieldAmount = 0;
    public ScriptableBuffDebuff righteous;

    private GameObject realTarget;

    private int shieldAmountScaling = 0;

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        BuffDebuffClass buffDebuffClass = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(entityUsedCard, righteous.nameID);

        int shield = 0;

        //scaling
        shieldAmountScaling = shieldAmount + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        if (buffDebuffClass != null)
        {
            shield = buffDebuffClass.tempValue + shieldAmountScaling;
        }
        else
        {
            shield = shieldAmountScaling;
        }


        int calculatedShield = (Combat.Instance == null) ? shield : Combat.Instance.CalculateEntityShield(shield, entityUsedCard, realTarget);
        customDesc += "Add " + DeckManager.Instance.GetCalculatedValueString(shield, calculatedShield) + " shield to target";
        customDesc += "\nShield Increase from " + BuffSystemManager.Instance.GetBuffDebuffColor(righteous) + " stacks";


        return customDesc;
    }

    public override void OnPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnPlayCard(cardScriptData, entityUsedCard);
        realTarget = CombatCardHandler.Instance.targetClicked;
        Activate(cardScriptData, entityUsedCard);
    }

    public override void OnAiPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayCard(cardScriptData, entityUsedCard);
        realTarget = entityUsedCard.GetComponent<AIBrain>().targetForCard;
        Activate(cardScriptData, entityUsedCard);
    }

    public void Activate(CardScriptData cardScriptData, GameObject entityUsedCard)
    {

        MonoBehaviour runner = CombatCardHandler.Instance;


        BuffDebuffClass buffDebuffClass = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(entityUsedCard, righteous.nameID);

        int shield = 0;

        //scaling
        shieldAmountScaling = shieldAmount + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        if (buffDebuffClass != null)
        {
            shield = buffDebuffClass.tempValue + shieldAmountScaling;
        }
        else
        {
            shield = shieldAmountScaling;
        }

        int calculatedShield = (Combat.Instance == null) ? shield : Combat.Instance.CalculateEntityShield(shield, entityUsedCard, realTarget);
        runner.StartCoroutine(Combat.Instance.AdjustTargetHealth(entityUsedCard, realTarget, calculatedShield, false, SystemManager.AdjustNumberModes.SHIELD));
    }

    public override void OnAiPlayTarget(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayTarget(cardScriptData, entityUsedCard);

        realTarget = AIManager.Instance.GetRandomAlly(entityUsedCard);

        entityUsedCard.GetComponent<AIBrain>().targetForCard = realTarget;

    }


}
