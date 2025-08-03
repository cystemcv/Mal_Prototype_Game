using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Angel_Card_RighteousEnergy", menuName = "Card/Angel/Angel_Card_RighteousEnergy")]
public class Angel_Card_RighteousEnergy : ScriptableCard
{

    public int buffValue = 0;

    private GameObject realTarget;
    private GameObject entityUsedCardGlobal;

    public ScriptableBuffDebuff righteous;

    private int buffValueFinal = 0;
    private int scalingBuff = 0;

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        //scaling
        scalingBuff = buffValue + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        customDesc += "Add " + scalingBuff + " " + BuffSystemManager.Instance.GetBuffDebuffColor(righteous) +
            "\nDouble this amount if there is no other " + BuffSystemManager.Instance.GetBuffDebuffColor(righteous);

        return customDesc;
    }

    public override void OnPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        realTarget = CombatCardHandler.Instance.targetClicked;

        base.OnPlayCard(cardScriptData, entityUsedCard);

        BuffDebuffClass buffDebuff = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(realTarget, righteous.nameID);

        //scaling
        scalingBuff = buffValue + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        if (buffDebuff == null)
        {
            buffValueFinal = scalingBuff * 2;
        }
        else
        {
            buffValueFinal = scalingBuff;
        }

        //strength
        BuffSystemManager.Instance.AddBuffDebuff(realTarget, righteous, buffValueFinal);
    }

    public override void OnAiPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
      
        base.OnPlayCard(cardScriptData, entityUsedCard);
        realTarget = entityUsedCard.GetComponent<AIBrain>().targetForCard;

        BuffDebuffClass buffDebuff = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(realTarget, righteous.nameID);

        //scaling
        scalingBuff = buffValue + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        if (buffDebuff == null)
        {
            buffValueFinal = scalingBuff * 2;
        }

        //strength
        BuffSystemManager.Instance.AddBuffDebuff(realTarget, righteous, buffValueFinal);
    }

    public override void OnAiPlayTarget(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayTarget(cardScriptData, entityUsedCard);
        realTarget = AIManager.Instance.GetRandomAlly(entityUsedCard);
        entityUsedCard.GetComponent<AIBrain>().targetForCard = realTarget;
    }




}
