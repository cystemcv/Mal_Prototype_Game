using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Angel_Card_RighteousFlame", menuName = "Card/Angel/Angel_Card_RighteousFlame")]
public class Angel_Card_RighteousFlame : ScriptableCard
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
        customDesc += multiHits + "X" +  " Deal " + DeckManager.Instance.GetCalculatedValueString(scalingDmg, calculatedDamage) + " to all enemies";

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

        realTarget = AIManager.Instance.GetRandomTarget(entityUsedCard);
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

        List<GameObject> targets = AIManager.Instance.GetAllTargets(entityUsedCardGlobal);

        //scaling
        scalingDmg = damageAmount + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        // Start the coroutine for each hit
        runner.StartCoroutine(Combat.Instance.AttackAllEnemy(this, scalingDmg, entityUsedCardGlobal, targets, multiHits, 2));

    }




}
