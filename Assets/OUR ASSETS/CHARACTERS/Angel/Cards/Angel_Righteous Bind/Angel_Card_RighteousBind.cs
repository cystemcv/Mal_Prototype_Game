using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Angel_Card_RighteousBind", menuName = "Card/Angel/Angel_Card_RighteousBind")]
public class Angel_Card_RighteousBind : ScriptableCard
{
    private GameObject realTarget;
    public ScriptableBuffDebuff root;
    public ScriptableBuffDebuff righteous;
    public int rootAmount = 2;

    private int scalingDebuff = 0;

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        //scaling
        scalingDebuff = rootAmount + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        customDesc += "Add " + scalingDebuff + " " + BuffSystemManager.Instance.GetBuffDebuffColor(root) + " to target";
        customDesc += "Add " + scalingDebuff + " " + BuffSystemManager.Instance.GetBuffDebuffColor(righteous) + " to self for each target hit";
        return customDesc;
    }

    public override void OnPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnPlayCard(cardScriptData, entityUsedCard);
        realTarget = CombatCardHandler.Instance.targetClicked;
        Activate(cardScriptData, entityUsedCard);

    }

    public override void OnAiPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayCard(cardScriptData, entityUsedCard);
        realTarget = entityUsedCard.GetComponent<AIBrain>().targetForCard;
        CombatCardHandler.Instance.posClickedTargeting = Combat.Instance.CheckCardTargets(realTarget, cardScriptData.scriptableCard);
        Activate(cardScriptData, entityUsedCard);


    }

    public void Activate(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        MonoBehaviour runner = CombatCardHandler.Instance;
        //scaling
        scalingDebuff = rootAmount + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        List<CombatPosition> targets = CombatCardHandler.Instance.posClickedTargeting;

        foreach (CombatPosition target in targets)
        {
            if (target.entityOccupiedPos != null)
            {
                BuffSystemManager.Instance.AddBuffDebuff(target.entityOccupiedPos, root, scalingDebuff);
                BuffSystemManager.Instance.AddBuffDebuff(entityUsedCard, righteous, scalingDebuff);
            }
        }
    }

    public override void OnAiPlayTarget(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayTarget(cardScriptData, entityUsedCard);
        realTarget = AIManager.Instance.GetRandomTarget(entityUsedCard);
        entityUsedCard.GetComponent<AIBrain>().targetForCard = realTarget;
    }

}
