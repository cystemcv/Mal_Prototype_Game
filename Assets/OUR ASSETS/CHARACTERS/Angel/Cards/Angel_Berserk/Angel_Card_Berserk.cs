using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Angel_Card_Berserk", menuName = "Card/Angel/Angel_Card_Berserk")]
public class Angel_Card_Berserk : ScriptableCard
{

    public int buffValueTemp1 = 0;
    public int buffTurnsTemp1 = 0;
    public int buffValueTemp2 = 0;
    public int buffTurnsTemp2 = 0;

    private GameObject realTarget;
    private GameObject entityUsedCardGlobal;

    public ScriptableBuffDebuff tempStrength;
    public ScriptableBuffDebuff tempDefence;

    public override string OnCardDescription(CardScript cardScript, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScript, entityUsedCard);

        customDesc += "Add " + buffValueTemp1 + " <color=yellow>" + tempStrength.nameID + "</color> for " + buffTurnsTemp1 + " turns";
        customDesc += "\nRemove " + Mathf.Abs(buffValueTemp2) + " <color=yellow>" + tempDefence.nameID + "</color> for " + buffTurnsTemp2 + " turns";
        return customDesc;
    }

    public override void OnPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        realTarget = CombatCardHandler.Instance.targetClicked;

        base.OnPlayCard(cardScript, entityUsedCard);

       
        BuffSystemManager.Instance.AddBuffDebuff(realTarget, tempStrength, buffValueTemp1, buffTurnsTemp1);

        //temp 
        BuffSystemManager.Instance.AddBuffDebuff(realTarget, tempDefence, buffValueTemp2, buffTurnsTemp2);

    }

    public override void OnAiPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        realTarget = AIManager.Instance.GetRandomAlly(entityUsedCard); ;

        base.OnPlayCard(cardScript, entityUsedCard);

        BuffSystemManager.Instance.AddBuffDebuff(entityUsedCard, tempStrength, buffValueTemp1, buffTurnsTemp1);

        //temp 
        BuffSystemManager.Instance.AddBuffDebuff(entityUsedCard, tempDefence, buffValueTemp2, buffTurnsTemp2);

    }






}
