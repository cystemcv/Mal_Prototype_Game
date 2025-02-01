using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Curse_Card_HauntingSpirit", menuName = "Card/Curse/Curse_Card_HauntingSpirit")]
public class Curse_Card_HauntingSpirit : ScriptableCard
{

    public int damageAmount = 0;
    public int multiHits = 0;

    private GameObject realTarget;
    private GameObject entityUsedCardGlobal;

    public override string OnCardDescription(CardScript cardScript, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScript, entityUsedCard);


        customDesc += "When you draw this card deal " + damageAmount + " to adventurer!";

        return customDesc;
    }

    public override void OnPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {

    }

    public override void OnAiPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {

    }

    public override void OnDrawCard(CardScript cardScript)
    {
        ExecuteCard();
    }

    public void ExecuteCard()
    {

        GameObject target = GameObject.FindGameObjectWithTag("Player");

        MonoBehaviour runner = CombatCardHandler.Instance;

        // Start the coroutine for each hit
        runner.StartCoroutine(Combat.Instance.AttackSingleTargetEnemy(this, damageAmount,1, null, target, multiHits, 1, true));

    }




}
