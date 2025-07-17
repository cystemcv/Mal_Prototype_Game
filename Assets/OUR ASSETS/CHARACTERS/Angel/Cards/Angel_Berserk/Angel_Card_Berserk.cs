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

    public override string OnCardDescription(CardScript cardScript, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScript, entityUsedCard);

        customDesc += "Add <color=yellow>" + berserk.nameID + "</color> for " + berserkTurns + " turns";
        customDesc += "\nAdd <color=yellow>" + frail.nameID + "</color> for " + frailTurn + " turns";
        return customDesc;
    }

    public override void OnPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        realTarget = CombatCardHandler.Instance.targetClicked;

        base.OnPlayCard(cardScript, entityUsedCard);

       
        BuffSystemManager.Instance.AddBuffDebuff(realTarget, berserk, 0, berserkTurns);

        //temp 
        BuffSystemManager.Instance.AddBuffDebuff(realTarget, frail, 0, frailTurn);

    }

    public override void OnAiPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        realTarget = AIManager.Instance.GetRandomAlly(entityUsedCard); ;

        base.OnPlayCard(cardScript, entityUsedCard);

        BuffSystemManager.Instance.AddBuffDebuff(entityUsedCard, berserk, 0, berserkTurns);

        //temp 
        BuffSystemManager.Instance.AddBuffDebuff(entityUsedCard, frail, 0, frailTurn);

    }






}
