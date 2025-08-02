using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Summoner_Card_DarkSlash", menuName = "Card/Summoner/Summoner_Card_DarkSlash")]
public class Summoner_Card_DarkSlash : ScriptableCard
{

    public int damageAmount = 0;
    public int multiHits = 0;

    private GameObject realTarget;
    private GameObject entityUsedCardGlobal;

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        int calculatedDamage = (Combat.Instance == null) ? damageAmount : Combat.Instance.CalculateEntityDmg(damageAmount, entityUsedCard, realTarget);
        customDesc += "Deal " + DeckManager.Instance.GetCalculatedValueString(damageAmount, calculatedDamage) + " to an enemy";

        return customDesc;
    }

    public override void OnPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnPlayCard(cardScriptData, entityUsedCard);

        realTarget = CombatCardHandler.Instance.targetClicked;
        entityUsedCardGlobal = entityUsedCard;

        ExecuteCard();

    }

    public override void OnAiPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayCard(cardScriptData, entityUsedCard);

        realTarget = entityUsedCard.GetComponent<AIBrain>().targetForCard;
        entityUsedCardGlobal = entityUsedCard;

        ExecuteCard();

    }

    public override void OnAiPlayTarget(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayTarget(cardScriptData, entityUsedCard);
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

        // Start the coroutine for each hit
        runner.StartCoroutine(Combat.Instance.AttackSingleTargetEnemy(this, damageAmount,1, entityUsedCardGlobal, realTarget, multiHits, 1));

    }




}
