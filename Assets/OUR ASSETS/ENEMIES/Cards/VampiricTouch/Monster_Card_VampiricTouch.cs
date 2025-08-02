using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster_Card_VampiricTouch", menuName = "Card/Monster/Monster_Card_VampiricTouch")]
public class Monster_Card_VampiricTouch : ScriptableCard
{

    public int damageAmount = 0;
    public int multiHits = 0;

    private GameObject realTarget;
    private GameObject entityUsedCardGlobal;

    private int finalDmg = 0;

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        finalDmg = AIManager.Instance.ReturnAIDmg(damageAmount, entityUsedCard);

        int calculatedDamage = (Combat.Instance == null) ? finalDmg : Combat.Instance.CalculateEntityDmg(finalDmg, entityUsedCard, realTarget);
        customDesc += "Deal " + DeckManager.Instance.GetCalculatedValueString(finalDmg, calculatedDamage) + " to an enemy";
        customDesc += "\nHeal " + finalDmg;

        return customDesc;
    }

    public override void OnPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        //base.OnPlayCard(cardScriptData, entityUsedCard);

        realTarget = CombatCardHandler.Instance.targetClicked;
        entityUsedCardGlobal = entityUsedCard;

        ExecuteCard();

    }

    public override void OnAiPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        //base.OnAiPlayCard(cardScriptData, entityUsedCard);

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
        runner.StartCoroutine(Combat.Instance.AttackSingleTargetEnemy(this, finalDmg, 1, entityUsedCardGlobal, realTarget, multiHits, 1));
        runner.StartCoroutine(Combat.Instance.AdjustTargetHealth(entityUsedCardGlobal, entityUsedCardGlobal, finalDmg, false,SystemManager.AdjustNumberModes.HEAL));
    }




}
