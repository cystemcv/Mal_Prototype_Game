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

        customDesc += "Add " + buffValueStrength + " " + BuffSystemManager.Instance.GetBuffDebuffColor(strength);
        customDesc += "\nAdd " + buffValuedDefence + " " + BuffSystemManager.Instance.GetBuffDebuffColor(defence);
        return customDesc;
    }

    public override void OnPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        realTarget = CombatCardHandler.Instance.targetClicked;

        base.OnPlayCard(cardScript, entityUsedCard);

        //strength
        BuffSystemManager.Instance.AddBuffDebuff(realTarget, strength, buffValueStrength);

        //defence
        BuffSystemManager.Instance.AddBuffDebuff(realTarget, defence, buffValuedDefence);


    }

    public override void OnAiPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
   

        base.OnPlayCard(cardScript, entityUsedCard);

        realTarget = entityUsedCard.GetComponent<AIBrain>().targetForCard;

        if (realTarget == null)
        {
            return;
        }

        //strength
        BuffSystemManager.Instance.AddBuffDebuff(realTarget, strength, buffValueStrength);

        //defence
        BuffSystemManager.Instance.AddBuffDebuff(realTarget, defence, buffValuedDefence);
    }


    public override void OnAiPlayTarget(CardScript cardScript, GameObject entityUsedCard)
    {
        base.OnAiPlayTarget(cardScript, entityUsedCard);

        realTarget = AIManager.Instance.GetRandomAlly(entityUsedCard);

        entityUsedCard.GetComponent<AIBrain>().targetForCard = realTarget;

    }



}
