using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Angel_Card_AngelicBarrier", menuName = "Card/Angel/Angel_Card_AngelicBarrier")]
public class Angel_Card_AngelicBarrier : ScriptableCard
{

    public int buffValue = 0;
    public int buffValueTemp = 0;
    public int buffTurnsTemp = 0;

    private GameObject realTarget;
    private GameObject entityUsedCardGlobal;

    public ScriptableBuffDebuff defence;
    public ScriptableBuffDebuff tempDefence;

    public override string OnCardDescription(CardScript cardScript, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScript, entityUsedCard);

        customDesc += "Add " +  buffValue + " " + BuffSystemManager.Instance.GetBuffDebuffColor(defence);
        customDesc += "\nAdd " + buffValueTemp + " " + BuffSystemManager.Instance.GetBuffDebuffColor(tempDefence) + " for " + buffTurnsTemp + " turns";
        return customDesc;
    }

    public override void OnPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        realTarget = CombatCardHandler.Instance.targetClicked;

        base.OnPlayCard(cardScript, entityUsedCard);

       
        BuffSystemManager.Instance.AddBuffDebuff(realTarget, defence, buffValue);

        //temp 
        BuffSystemManager.Instance.AddBuffDebuff(realTarget, tempDefence, buffTurnsTemp);

    }

    public override void OnAiPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        realTarget = AIManager.Instance.GetRandomAlly(entityUsedCard); ;

        base.OnPlayCard(cardScript, entityUsedCard);

        //strength
        BuffSystemManager.Instance.AddBuffDebuff(entityUsedCard, defence, buffValue);

        //temp strength
        BuffSystemManager.Instance.AddBuffDebuff(entityUsedCard, tempDefence, buffValueTemp);
    }






}
