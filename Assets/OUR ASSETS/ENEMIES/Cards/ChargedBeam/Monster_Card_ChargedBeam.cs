using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster_Card_ChargedBeam", menuName = "Card/Monster/Monster_Card_ChargedBeam")]
public class Monster_Card_ChargedBeam : ScriptableCard
{

    public int damageAmount = 0;
    public int multiHits = 0;

    private GameObject realTarget;
    private GameObject entityUsedCardGlobal;

    private int scalingDmg = 0;

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        //scaling
        scalingDmg = damageAmount + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        int calculatedDamage = (Combat.Instance == null) ? scalingDmg : Combat.Instance.CalculateEntityDmg(scalingDmg, entityUsedCard, realTarget);
        customDesc += "Deal " + DeckManager.Instance.GetCalculatedValueString(scalingDmg, calculatedDamage) + " to an enemy";
        customDesc += "\nCannot hit Summons";

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


        if (SystemManager.Instance.GetEnemyTagsList().Contains(entityUsedCard.tag))
        {
            realTarget = GameObject.FindGameObjectWithTag("Player");
        }
        else
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
           
            realTarget = enemies[Random.Range(0,enemies.Length)];
        }

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

        //scaling
        scalingDmg = damageAmount + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        // Start the coroutine for each hit
        runner.StartCoroutine(Combat.Instance.AttackSingleTargetEnemy(this, scalingDmg, 1, entityUsedCardGlobal, realTarget, multiHits, 1));

    }




}
