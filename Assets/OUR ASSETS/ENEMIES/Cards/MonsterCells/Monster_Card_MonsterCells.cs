using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster_Card_MonsterCells", menuName = "Card/Monster/Monster_Card_MonsterCells")]
public class Monster_Card_MonsterCells : ScriptableCard
{
    public ScriptableBuffDebuff buff;
    public int buffTurns = 1;
    private GameObject realTarget;

    public override string OnCardDescription(CardScript cardScript, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScript, entityUsedCard);

        customDesc += "Add " + buffTurns + " " + BuffSystemManager.Instance.GetBuffDebuffColor(buff) + " to target";

        return customDesc;
    }

    public override void OnPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        realTarget = CombatCardHandler.Instance.targetClicked;

        base.OnPlayCard(cardScript, entityUsedCard);

        //strength
        BuffSystemManager.Instance.AddBuffDebuff(realTarget, buff, buffTurns);
    }

    public override void OnAiPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        realTarget = entityUsedCard.GetComponent<AIBrain>().targetForCard;

        base.OnPlayCard(cardScript, entityUsedCard);

        //strength
        BuffSystemManager.Instance.AddBuffDebuff(realTarget, buff, buffTurns);
    }

    public override void OnAiPlayTarget(CardScript cardScript, GameObject entityUsedCard)
    {
        base.OnAiPlayTarget(cardScript, entityUsedCard);
        realTarget = AIManager.Instance.GetRandomAlly(entityUsedCard);
        entityUsedCard.GetComponent<AIBrain>().targetForCard = realTarget;
    }

}
