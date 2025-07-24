using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster_Card_GentleGrab", menuName = "Card/Monster/Monster_Card_GentleGrab")]
public class Monster_Card_GentleGrab : ScriptableCard
{
    private GameObject realTarget;
    public ScriptableBuffDebuff root;
    public int rootAmount = 3;

    public override string OnCardDescription(CardScript cardScript, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScript, entityUsedCard);

        customDesc += "Add " + rootAmount + " " + BuffSystemManager.Instance.GetBuffDebuffColor(root) + " to target";

        return customDesc;
    }

    public override void OnPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        base.OnPlayCard(cardScript, entityUsedCard);

        MonoBehaviour runner = CombatCardHandler.Instance;

        realTarget = CombatCardHandler.Instance.targetClicked;

        BuffSystemManager.Instance.AddBuffDebuff(realTarget, root,  rootAmount);

    }

    public override void OnAiPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        base.OnAiPlayCard(cardScript, entityUsedCard);

        MonoBehaviour runner = CombatCardHandler.Instance;

        realTarget = AIManager.Instance.GetRandomTarget(entityUsedCard);

        BuffSystemManager.Instance.AddBuffDebuff(realTarget, root,  rootAmount);
    }

}
