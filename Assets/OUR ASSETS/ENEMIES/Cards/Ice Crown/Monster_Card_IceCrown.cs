using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster_Card_IceCrown", menuName = "Card/Monster/Monster_Card_IceCrown")]
public class Monster_Card_IceCrown : ScriptableCard
{
    private GameObject realTarget;
    public ScriptableBuffDebuff counter;

    private int scalingDmg = 0;

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        BuffDebuffClass buffDebuffClass = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(entityUsedCard, counter.nameID);

        int dmg = 0;

        //scaling
        scalingDmg =  (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        if (buffDebuffClass != null)
        {
            dmg = buffDebuffClass.tempValue + scalingDmg;
        }

        customDesc += "Deal Double Damage (" + 2 * dmg + ") from " + BuffSystemManager.Instance.GetBuffDebuffColor(counter) + " stacks to target";

        return customDesc;
    }

    public override void OnPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnPlayCard(cardScriptData, entityUsedCard);

        MonoBehaviour runner = CombatCardHandler.Instance;

        realTarget = CombatCardHandler.Instance.targetClicked;

        BuffDebuffClass buffDebuffClass = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(entityUsedCard, counter.nameID);

        int dmg = 0;

        //scaling
        scalingDmg = (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        if (buffDebuffClass != null)
        {
            dmg = buffDebuffClass.tempValue + scalingDmg;
        }

        runner.StartCoroutine(Combat.Instance.AdjustTargetHealth(entityUsedCard, realTarget, 2 * dmg, false, SystemManager.AdjustNumberModes.ATTACK));


    }

    public override void OnAiPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayCard(cardScriptData, entityUsedCard);

        MonoBehaviour runner = CombatCardHandler.Instance;

        realTarget = entityUsedCard.GetComponent<AIBrain>().targetForCard;

        BuffDebuffClass buffDebuffClass = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(entityUsedCard, counter.nameID);

        int dmg = 0;

        //scaling
        scalingDmg = (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        if (buffDebuffClass != null)
        {
            dmg = buffDebuffClass.tempValue + scalingDmg;
        }

        runner.StartCoroutine(Combat.Instance.AdjustTargetHealth(entityUsedCard, realTarget, 2 * dmg, false, SystemManager.AdjustNumberModes.ATTACK));
    }

    public override void OnAiPlayTarget(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayTarget(cardScriptData, entityUsedCard);
        realTarget = AIManager.Instance.GetRandomTarget(entityUsedCard);
        entityUsedCard.GetComponent<AIBrain>().targetForCard = realTarget;
    }

}
