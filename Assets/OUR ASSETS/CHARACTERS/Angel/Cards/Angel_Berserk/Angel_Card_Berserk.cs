using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Angel_Card_Berserk", menuName = "Card/Angel/Angel_Card_Berserk")]
public class Angel_Card_Berserk : ScriptableCard
{

    public int berserkTurns = 0;
    public int frailTurn = 0;

    private GameObject realTarget;
    private GameObject entityUsedCardGlobal;

    public ScriptableBuffDebuff berserk;
    public ScriptableBuffDebuff frail;

    private int cardScalingBerserk = 0;
    private int cardScalingFrail = 0;

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        //scaling
        cardScalingBerserk = berserkTurns + (scalingLevelCardValue * cardScriptData.scalingLevelValue);
        cardScalingFrail = berserkTurns + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        customDesc += "Add " + cardScalingBerserk + " " + BuffSystemManager.Instance.GetBuffDebuffColor(berserk);
        customDesc += "\nAdd " + cardScalingFrail + " " + BuffSystemManager.Instance.GetBuffDebuffColor(frail);
        return customDesc;
    }

    public override void OnPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        realTarget = CombatCardHandler.Instance.targetClicked;

        base.OnPlayCard(cardScriptData, entityUsedCard);

        //scaling
        cardScalingBerserk = berserkTurns + (scalingLevelCardValue * cardScriptData.scalingLevelValue);
        cardScalingFrail = berserkTurns + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        BuffSystemManager.Instance.AddBuffDebuff(realTarget, berserk, cardScalingBerserk);

        //temp 
        BuffSystemManager.Instance.AddBuffDebuff(realTarget, frail, cardScalingFrail);

    }

    public override void OnAiPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {


        base.OnPlayCard(cardScriptData, entityUsedCard);

        realTarget = entityUsedCard.GetComponent<AIBrain>().targetForCard;

        if (realTarget == null)
        {
            return;
        }

        //scaling
        cardScalingBerserk = berserkTurns + (scalingLevelCardValue * cardScriptData.scalingLevelValue);
        cardScalingFrail = berserkTurns + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        BuffSystemManager.Instance.AddBuffDebuff(realTarget, berserk, cardScalingBerserk);

        //temp 
        BuffSystemManager.Instance.AddBuffDebuff(realTarget, frail, cardScalingFrail);

    }

    public override void OnAiPlayTarget(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayTarget(cardScriptData, entityUsedCard);

        realTarget = AIManager.Instance.GetRandomAlly(entityUsedCard);

        entityUsedCard.GetComponent<AIBrain>().targetForCard = realTarget;



    }




}
