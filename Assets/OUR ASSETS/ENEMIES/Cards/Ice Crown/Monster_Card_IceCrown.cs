using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster_Card_IceCrown", menuName = "Card/Monster/Monster_Card_IceCrown")]
public class Monster_Card_IceCrown : ScriptableCard
{
    private GameObject realTarget;
    public ScriptableBuffDebuff counter;

    public override string OnCardDescription(CardScript cardScript, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScript, entityUsedCard);

        BuffDebuffClass buffDebuffClass = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(entityUsedCard, counter.nameID);

        int dmg = 0;

        if (buffDebuffClass != null)
        {
            dmg = buffDebuffClass.tempVariable;
        }

        customDesc += "Deal Double Damage (" + 2 * dmg + ") from " + BuffSystemManager.Instance.GetBuffDebuffColor(counter) + " stacks to target";

        return customDesc;
    }

    public override void OnPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        base.OnPlayCard(cardScript, entityUsedCard);

        MonoBehaviour runner = CombatCardHandler.Instance;

        realTarget = CombatCardHandler.Instance.targetClicked;

        BuffDebuffClass buffDebuffClass = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(entityUsedCard, counter.nameID);

        int dmg = 0;

        if (buffDebuffClass != null)
        {
            dmg = buffDebuffClass.tempVariable;
        }

        runner.StartCoroutine(Combat.Instance.AdjustTargetHealth(entityUsedCard, realTarget, 2 * dmg, false, SystemManager.AdjustNumberModes.ATTACK));


    }

    public override void OnAiPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        base.OnAiPlayCard(cardScript, entityUsedCard);

        MonoBehaviour runner = CombatCardHandler.Instance;

        realTarget = AIManager.Instance.GetRandomTarget(entityUsedCard);

        BuffDebuffClass buffDebuffClass = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(entityUsedCard, counter.nameID);

        int dmg = 0;

        if (buffDebuffClass != null)
        {
            dmg = buffDebuffClass.tempVariable;
        }

        runner.StartCoroutine(Combat.Instance.AdjustTargetHealth(entityUsedCard, realTarget, 2 * dmg, false, SystemManager.AdjustNumberModes.ATTACK));
    }

}
