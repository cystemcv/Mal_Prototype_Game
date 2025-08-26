using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster_Card_MonsterCells", menuName = "Card/Monster/Monster_Card_MonsterCells")]
public class Monster_Card_MonsterCells : ScriptableCard
{
    public ScriptableBuffDebuff buff;
    public int buffTurns = 1;
    private GameObject realTarget;

    private int scalingBuff = 0;

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        //scaling
        scalingBuff = buffTurns + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        customDesc += "Add " + scalingBuff + " " + BuffSystemManager.Instance.GetBuffDebuffColor(buff) + " to target";

        return customDesc;
    }

    public override void OnPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        realTarget = CombatCardHandler.Instance.targetClicked;

        base.OnPlayCard(cardScriptData, entityUsedCard);

        //scaling
        scalingBuff = buffTurns + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        //strength
        BuffSystemManager.Instance.AddBuffDebuff(realTarget, buff, scalingBuff);
    }

    public override void OnAiPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        realTarget = entityUsedCard.GetComponent<AIBrain>().targetForCard;

        base.OnPlayCard(cardScriptData, entityUsedCard);

        //scaling
        scalingBuff = buffTurns + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        //strength
        BuffSystemManager.Instance.AddBuffDebuff(realTarget, buff, scalingBuff);
    }

    public override void OnAiPlayTarget(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayTarget(cardScriptData, entityUsedCard);
        realTarget = AIManager.Instance.GetRandomAlly(entityUsedCard);
        entityUsedCard.GetComponent<AIBrain>().targetForCard = realTarget;
    }

}
