using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster_Card_ReflectingMirror", menuName = "Card/Monster/Monster_Card_ReflectingMirror")]
public class Monster_Card_ReflectingMirror : ScriptableCard
{
    private GameObject realTarget;
    public ScriptableBuffDebuff counter;
    public int counterAmount = 3;

    private int scalingBuff = 0;

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        //scaling
        scalingBuff = counterAmount + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        customDesc += "Add " + scalingBuff + " " + BuffSystemManager.Instance.GetBuffDebuffColor(counter) + " to target";

        return customDesc;
    }

    public override void OnPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnPlayCard(cardScriptData, entityUsedCard);

        MonoBehaviour runner = CombatCardHandler.Instance;

        realTarget = CombatCardHandler.Instance.targetClicked;

        //scaling
        scalingBuff = counterAmount + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        BuffSystemManager.Instance.AddBuffDebuff(realTarget, counter, scalingBuff);

    }

    public override void OnAiPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayCard(cardScriptData, entityUsedCard);

        MonoBehaviour runner = CombatCardHandler.Instance;

        realTarget = entityUsedCard.GetComponent<AIBrain>().targetForCard;

        //scaling
        scalingBuff = counterAmount + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        BuffSystemManager.Instance.AddBuffDebuff(realTarget, counter, scalingBuff);
    }

    public override void OnAiPlayTarget(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayTarget(cardScriptData, entityUsedCard);
        realTarget = AIManager.Instance.GetRandomAlly(entityUsedCard);
        entityUsedCard.GetComponent<AIBrain>().targetForCard = realTarget;
    }

}
