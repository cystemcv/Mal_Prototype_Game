using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Angel_Card_Determination", menuName = "Card/Angel/Angel_Card_Determination")]
public class Angel_Card_Determination : ScriptableCard
{

    public int buffValueStrength = 0;
    public int buffValuedDefence = 0;

    private GameObject realTarget;
    private GameObject entityUsedCardGlobal;

    public ScriptableBuffDebuff strength;
    public ScriptableBuffDebuff defence;

    public override string OnCardDescription(CardScript cardScript, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScript, entityUsedCard);

        customDesc += "Add " + buffValueStrength + " <color=yellow>" + strength.nameID + "</color>";
        customDesc += "\nAdd " + buffValuedDefence + " <color=yellow>" + defence.nameID + "</color>";
        return customDesc;
    }

    public override void OnPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        realTarget = CombatCardHandler.Instance.targetClicked;

        base.OnPlayCard(cardScript, entityUsedCard);

        //strength
        BuffSystemManager.Instance.AddBuffDebuff(realTarget, strength, buffValueStrength, 0);

        //defence
        BuffSystemManager.Instance.AddBuffDebuff(realTarget, defence, buffValuedDefence, 0);


    }

    public override void OnAiPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        realTarget = AIManager.Instance.GetRandomAlly(entityUsedCard); ;

        base.OnPlayCard(cardScript, entityUsedCard);


        //strength
        BuffSystemManager.Instance.AddBuffDebuff(realTarget, strength, buffValueStrength, 0);

        //defence
        BuffSystemManager.Instance.AddBuffDebuff(realTarget, defence, buffValuedDefence, 0);
    }






}
