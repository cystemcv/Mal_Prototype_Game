using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Angel_Card_RainCloud", menuName = "Card/Angel/Angel_Card_RainCloud")]
public class Angel_Card_RainCloud : ScriptableCard
{

    public int buffValue = 0;

    private GameObject realTarget;
    private GameObject entityUsedCardGlobal;

    public ScriptableBuffDebuff wet;


    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        customDesc += "Add " + buffValue + " " + BuffSystemManager.Instance.GetBuffDebuffColor(wet);

        return customDesc;
    }

    public override void OnPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        realTarget = CombatCardHandler.Instance.targetClicked;

        base.OnPlayCard(cardScriptData, entityUsedCard);

        //strength
        BuffSystemManager.Instance.AddBuffDebuff(realTarget, wet, buffValue);
    }

    public override void OnAiPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnPlayCard(cardScriptData, entityUsedCard);
        realTarget = entityUsedCard.GetComponent<AIBrain>().targetForCard;
        //strength
        BuffSystemManager.Instance.AddBuffDebuff(realTarget, wet, buffValue);
    }

    public override void OnAiPlayTarget(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayTarget(cardScriptData, entityUsedCard);
        realTarget = AIManager.Instance.GetRandomAlly(entityUsedCard);
        entityUsedCard.GetComponent<AIBrain>().targetForCard = realTarget;
    }




}
