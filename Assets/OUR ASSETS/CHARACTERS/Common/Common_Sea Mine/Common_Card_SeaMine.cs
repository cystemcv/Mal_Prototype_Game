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

    public override string OnCardDescription(CardScript cardScript, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScript, entityUsedCard);

        int calculatedDamage = (Combat.Instance == null) ? damageAmount : Combat.Instance.CalculateEntityDmg(damageAmount, entityUsedCard, realTarget);

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

    public override void OnPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        base.OnPlayCard(cardScript, entityUsedCard);

        realTarget = CombatCardHandler.Instance.targetClicked;
        entityUsedCardGlobal = entityUsedCard;

        ExecuteCard();

    }

    public override void OnAiPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        base.OnAiPlayCard(cardScript, entityUsedCard);

        realTarget = entityUsedCard.GetComponent<AIBrain>().targetForCard;
        entityUsedCardGlobal = entityUsedCard;

        ExecuteCard();

    }

    public override void OnAiPlayTarget(CardScript cardScript, GameObject entityUsedCard)
    {
        base.OnAiPlayTarget(cardScript, entityUsedCard);
        realTarget = AIManager.Instance.GetRandomTarget(entityUsedCard);
        entityUsedCard.GetComponent<AIBrain>().targetForCard = realTarget;
    }

    public void ExecuteCard()
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

        // Start the coroutine for each hit
        runner.StartCoroutine(Combat.Instance.AttackSingleTargetEnemy(this, damageAmount, multiplyDamage , entityUsedCardGlobal, realTarget, multiHits, 1));

    }

}
