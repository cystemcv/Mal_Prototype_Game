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

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        customDesc += "Add " + BuffSystemManager.Instance.GetBuffDebuffColor(berserk) + " for " + berserkTurns + " turns";
        customDesc += "\nAdd " + BuffSystemManager.Instance.GetBuffDebuffColor(frail) + " for " + frailTurn + " turns";
        return customDesc;
    }

    public override void OnPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        realTarget = CombatCardHandler.Instance.targetClicked;

        base.OnPlayCard(cardScriptData, entityUsedCard);

       
        BuffSystemManager.Instance.AddBuffDebuff(realTarget, berserk,  berserkTurns);

        //temp 
        BuffSystemManager.Instance.AddBuffDebuff(realTarget, frail,  frailTurn);

    }

    public override void OnAiPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {


        base.OnPlayCard(cardScriptData, entityUsedCard);

        realTarget = entityUsedCard.GetComponent<AIBrain>().targetForCard;

        if (realTarget == null)
        {
            return;
        }

        BuffSystemManager.Instance.AddBuffDebuff(entityUsedCard, berserk, berserkTurns);

        //temp 
        BuffSystemManager.Instance.AddBuffDebuff(entityUsedCard, frail,  frailTurn);

    }

    public override void OnAiPlayTarget(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayTarget(cardScriptData, entityUsedCard);

        realTarget = AIManager.Instance.GetRandomAlly(entityUsedCard);

        entityUsedCard.GetComponent<AIBrain>().targetForCard = realTarget;

    }




}
