using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Common_Card_SeaMine", menuName = "Card/Common/Common_Card_SeaMine")]
public class Common_Card_SeaMine : ScriptableCard
{

    public int damageAmount = 4;
    public int multiHits = 0;
    public int multiplyAmountGlobal = 4;

    private GameObject realTarget;
    private GameObject entityUsedCardGlobal;

    private int scalingDmg = 0;

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        //scaling
        scalingDmg = damageAmount + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        int calculatedDamage = (Combat.Instance == null) ? scalingDmg : Combat.Instance.CalculateEntityDmg(scalingDmg, entityUsedCard, realTarget);

        int finalDamage = calculatedDamage * multiplyAmountGlobal;

        if (Combat.Instance != null && Combat.Instance.battleGroundType == SystemManager.BattleGroundType.WATER)
        {
            customDesc += "Deal " + DeckManager.Instance.GetCalculatedValueString(damageAmount, calculatedDamage) + " to an enemy." + "<br>" +
                "<color=blue>(x" + multiplyAmountGlobal + " on Water Battleground)</color>";
        }
        else
        {
            customDesc += "Deal " + DeckManager.Instance.GetCalculatedValueString(damageAmount, calculatedDamage) + " to an enemy." + "<br>" +
    "(x" + multiplyAmountGlobal + " on Water Battleground)";
        }



        return customDesc;
    }

    public override void OnPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnPlayCard(cardScriptData, entityUsedCard);

        realTarget = CombatCardHandler.Instance.targetClicked;
        entityUsedCardGlobal = entityUsedCard;

        ExecuteCard(cardScriptData);

    }

    public override void OnAiPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayCard(cardScriptData, entityUsedCard);

        realTarget = entityUsedCard.GetComponent<AIBrain>().targetForCard;
        entityUsedCardGlobal = entityUsedCard;

        ExecuteCard(cardScriptData);

    }

    public override void OnAiPlayTarget(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayTarget(cardScriptData, entityUsedCard);
        realTarget = AIManager.Instance.GetRandomTarget(entityUsedCard);
        entityUsedCard.GetComponent<AIBrain>().targetForCard = realTarget;
    }

    public void ExecuteCard(CardScriptData cardScriptData)
    {

        //then loop
        if (multiHits <= 0)
        {
            multiHits = 1;
        }

        MonoBehaviour runner = CombatCardHandler.Instance;


        int multiplyDamage = 1;
        if (Combat.Instance.battleGroundType == SystemManager.BattleGroundType.WATER)
        {
            multiplyDamage = multiplyAmountGlobal;
        }

        //scaling
        scalingDmg = damageAmount + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        // Start the coroutine for each hit
        runner.StartCoroutine(Combat.Instance.AttackSingleTargetEnemy(this, scalingDmg, multiplyDamage , entityUsedCardGlobal, realTarget, multiHits, 1));

    }

}
