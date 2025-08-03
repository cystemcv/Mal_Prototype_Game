using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Angel_Card_BlindAttack", menuName = "Card/Angel/Angel_Card_BlindAttack")]
public class Angel_Card_BlindAttack : ScriptableCard
{

    public int damageAmount = 0;
    public int multiHits = 0;

    private GameObject realTarget;
    private GameObject entityUsedCardGlobal;

    private int scalingHits = 0;

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        //scaling
        scalingHits = multiHits + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        int calculatedDamage = (Combat.Instance == null) ? damageAmount : Combat.Instance.CalculateEntityDmg(damageAmount, entityUsedCard, realTarget);
        customDesc += scalingHits + "X" +  " Deal " + DeckManager.Instance.GetCalculatedValueString(damageAmount, calculatedDamage) + " to random enemy";

        return customDesc;
    }

    public override void OnPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        //base.OnPlayCard(cardScriptData, entityUsedCard);

        //realTarget = CombatCardHandler.Instance.targetClicked;
        entityUsedCardGlobal = entityUsedCard;

        ExecuteCard(cardScriptData);

    }

    public override void OnAiPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        //base.OnAiPlayCard(cardScriptData, entityUsedCard);

        entityUsedCardGlobal = entityUsedCard;

        ExecuteCard(cardScriptData);

    }

    public void ExecuteCard(CardScriptData cardScriptData)
    {

        //then loop
        if (multiHits <= 0)
        {
            multiHits = 1;
        }

        MonoBehaviour runner = CombatCardHandler.Instance;

        //scaling
        scalingHits = multiHits + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        // Start the coroutine for each hit
        runner.StartCoroutine(Combat.Instance.AttackBlindlyEnemy(this, damageAmount, entityUsedCardGlobal, realTarget, scalingHits, 2));

    }




}
