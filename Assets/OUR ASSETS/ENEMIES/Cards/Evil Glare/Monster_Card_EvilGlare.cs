using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster_Card_EvilGlare", menuName = "Card/Monster/Monster_Card_EvilGlare")]
public class Monster_Card_EvilGlare : ScriptableCard
{
    private GameObject realTarget;
    public ScriptableBuffDebuff weaken;
    public int weakenTurns = 0;


    private int scalingDebuff = 0;

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        //scaling
        scalingDebuff = weakenTurns + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        customDesc += "Add " + scalingDebuff + " " + BuffSystemManager.Instance.GetBuffDebuffColor(weaken);

        return customDesc;
    }

    public override void OnPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnPlayCard(cardScriptData, entityUsedCard);

        MonoBehaviour runner = CombatCardHandler.Instance;

        realTarget = CombatCardHandler.Instance.targetClicked;

        //scaling
        scalingDebuff = weakenTurns + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        BuffSystemManager.Instance.AddBuffDebuff(realTarget, weaken, scalingDebuff);
    }

    public override void OnAiPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayCard(cardScriptData, entityUsedCard);

        MonoBehaviour runner = CombatCardHandler.Instance;

        realTarget = entityUsedCard.GetComponent<AIBrain>().targetForCard;

        //scaling
        scalingDebuff = weakenTurns + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        BuffSystemManager.Instance.AddBuffDebuff(realTarget, weaken, scalingDebuff);
    }

    public override void OnAiPlayTarget(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayTarget(cardScriptData, entityUsedCard);
        realTarget = AIManager.Instance.GetRandomTarget(entityUsedCard);
        entityUsedCard.GetComponent<AIBrain>().targetForCard = realTarget;
    }

}
