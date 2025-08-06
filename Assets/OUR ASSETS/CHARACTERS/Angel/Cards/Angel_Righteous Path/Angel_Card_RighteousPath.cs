using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Angel_Card_RighteousPath", menuName = "Card/Angel/Angel_Card_RighteousPath")]
public class Angel_Card_RighteousPath : ScriptableCard
{
    private GameObject realTarget;
    public ScriptableBuffDebuff honor;

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        //BuffDebuffClass buffDebuffClass = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(entityUsedCard, darkBlade.nameID);

        customDesc += "Add " + BuffSystemManager.Instance.GetBuffDebuffColor(honor) + " to target";
        customDesc += "\nCannot be used on summons";
        return customDesc;
    }

    public override void OnPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        realTarget = CombatCardHandler.Instance.targetClicked;

        base.OnPlayCard(cardScriptData, entityUsedCard);

        BuffSystemManager.Instance.AddBuffDebuff(realTarget, honor, 1);


    }

    public override void OnAiPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {


        base.OnPlayCard(cardScriptData, entityUsedCard);
        realTarget = entityUsedCard.GetComponent<AIBrain>().targetForCard;
        BuffSystemManager.Instance.AddBuffDebuff(entityUsedCard, honor, 1);

    }

    public override void OnAiPlayTarget(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayTarget(cardScriptData, entityUsedCard);
        realTarget = AIManager.Instance.GetRandomAlly(entityUsedCard);
        entityUsedCard.GetComponent<AIBrain>().targetForCard = realTarget;
    }

}
