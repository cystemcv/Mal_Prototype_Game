using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster_Card_EvilGlare", menuName = "Card/Monster/Monster_Card_EvilGlare")]
public class Monster_Card_EvilGlare : ScriptableCard
{
    private GameObject realTarget;
    public ScriptableBuffDebuff weaken;
    public int weakenTurns = 0;

    public override string OnCardDescription(CardScript cardScript, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScript, entityUsedCard);

        customDesc += "Add <color=yellow>" + weaken.nameID + "</color> for " + weakenTurns + " to target";

        return customDesc;
    }

    public override void OnPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        base.OnPlayCard(cardScript, entityUsedCard);

        MonoBehaviour runner = CombatCardHandler.Instance;

        realTarget = CombatCardHandler.Instance.targetClicked;

        BuffSystemManager.Instance.AddBuffDebuff(realTarget, weaken, 0, weakenTurns);
    }

    public override void OnAiPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        base.OnAiPlayCard(cardScript, entityUsedCard);

        MonoBehaviour runner = CombatCardHandler.Instance;

        realTarget = AIManager.Instance.GetRandomTarget(entityUsedCard);

        BuffSystemManager.Instance.AddBuffDebuff(realTarget, weaken, 0, weakenTurns);
    }

}
