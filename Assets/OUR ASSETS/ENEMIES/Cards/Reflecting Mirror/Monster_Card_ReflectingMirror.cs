using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster_Card_ReflectingMirror", menuName = "Card/Monster/Monster_Card_ReflectingMirror")]
public class Monster_Card_ReflectingMirror : ScriptableCard
{
    private GameObject realTarget;
    public ScriptableBuffDebuff counter;
    public int counterAmount = 3;

    public override string OnCardDescription(CardScript cardScript, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScript, entityUsedCard);

        customDesc += "Add " + counterAmount + " " + BuffSystemManager.Instance.GetBuffDebuffColor(counter) + " to target";

        return customDesc;
    }

    public override void OnPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        base.OnPlayCard(cardScript, entityUsedCard);

        MonoBehaviour runner = CombatCardHandler.Instance;

        realTarget = CombatCardHandler.Instance.targetClicked;

        BuffSystemManager.Instance.AddBuffDebuff(realTarget, counter, counterAmount);

    }

    public override void OnAiPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        base.OnAiPlayCard(cardScript, entityUsedCard);

        MonoBehaviour runner = CombatCardHandler.Instance;

        realTarget = entityUsedCard.GetComponent<AIBrain>().targetForCard;

        BuffSystemManager.Instance.AddBuffDebuff(realTarget, counter, counterAmount);
    }

    public override void OnAiPlayTarget(CardScript cardScript, GameObject entityUsedCard)
    {
        base.OnAiPlayTarget(cardScript, entityUsedCard);
        realTarget = AIManager.Instance.GetRandomAlly(entityUsedCard);
        entityUsedCard.GetComponent<AIBrain>().targetForCard = realTarget;
    }

}
