using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster_Card_DarkBlade", menuName = "Card/Monster/Monster_Card_DarkBlade")]
public class Monster_Card_DarkBlade : ScriptableCard
{
    private GameObject realTarget;
    public ScriptableBuffDebuff darkBlade;

    public override string OnCardDescription(CardScript cardScript, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScript, entityUsedCard);

        //BuffDebuffClass buffDebuffClass = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(entityUsedCard, darkBlade.nameID);

        customDesc += "Add " + BuffSystemManager.Instance.GetBuffDebuffColor(darkBlade) + " to target";
        customDesc += "\nCannot be used on summons";
        return customDesc;
    }

    public override void OnPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        realTarget = CombatCardHandler.Instance.targetClicked;

        base.OnPlayCard(cardScript, entityUsedCard);

        BuffSystemManager.Instance.AddBuffDebuff(realTarget, darkBlade, 1);


    }

    public override void OnAiPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {


        base.OnPlayCard(cardScript, entityUsedCard);
        realTarget = entityUsedCard.GetComponent<AIBrain>().targetForCard;
        BuffSystemManager.Instance.AddBuffDebuff(entityUsedCard, darkBlade, 1);

    }

    public override void OnAiPlayTarget(CardScript cardScript, GameObject entityUsedCard)
    {
        base.OnAiPlayTarget(cardScript, entityUsedCard);
        realTarget = AIManager.Instance.GetRandomAlly(entityUsedCard);
        entityUsedCard.GetComponent<AIBrain>().targetForCard = realTarget;
    }

}
