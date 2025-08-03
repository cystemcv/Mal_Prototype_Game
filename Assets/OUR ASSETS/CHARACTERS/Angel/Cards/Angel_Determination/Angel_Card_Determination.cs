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

    private int cardScalingStrength = 0;
    private int cardScalingDefence = 0;

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        //scaling
        cardScalingStrength = buffValueStrength + (scalingLevelCardValue * cardScriptData.scalingLevelValue);
        cardScalingDefence = buffValuedDefence + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        customDesc += "Add " + cardScalingStrength + " " + BuffSystemManager.Instance.GetBuffDebuffColor(strength);
        customDesc += "\nAdd " + cardScalingDefence + " " + BuffSystemManager.Instance.GetBuffDebuffColor(defence);
        return customDesc;
    }

    public override void OnPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        realTarget = CombatCardHandler.Instance.targetClicked;

        base.OnPlayCard(cardScriptData, entityUsedCard);

        //scaling
        cardScalingStrength = buffValueStrength + (scalingLevelCardValue * cardScriptData.scalingLevelValue);
        cardScalingDefence = buffValuedDefence + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        //strength
        BuffSystemManager.Instance.AddBuffDebuff(realTarget, strength, cardScalingStrength);

        //defence
        BuffSystemManager.Instance.AddBuffDebuff(realTarget, defence, cardScalingDefence);


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
        cardScalingStrength = buffValueStrength + (scalingLevelCardValue * cardScriptData.scalingLevelValue);
        cardScalingDefence = buffValuedDefence + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        //strength
        BuffSystemManager.Instance.AddBuffDebuff(realTarget, strength, cardScalingStrength);

        //defence
        BuffSystemManager.Instance.AddBuffDebuff(realTarget, defence, cardScalingDefence);
    }


    public override void OnAiPlayTarget(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayTarget(cardScriptData, entityUsedCard);

        realTarget = AIManager.Instance.GetRandomAlly(entityUsedCard);

        entityUsedCard.GetComponent<AIBrain>().targetForCard = realTarget;

    }



}
