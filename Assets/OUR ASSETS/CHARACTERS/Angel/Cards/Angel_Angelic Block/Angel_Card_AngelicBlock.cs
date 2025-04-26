using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Angel_Card_AngelicBlock", menuName = "Card/Angel/Angel_Card_AngelicBlock")]
public class Angel_Card_AngelicBlock : ScriptableCard
{

    public int shieldAmount = 0;

    private GameObject realTarget;

    public override string OnCardDescription(CardScript cardScript, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScript, entityUsedCard);

        int calculatedShield = Combat.Instance.CalculateEntityShield(shieldAmount, entityUsedCard, realTarget);
        customDesc += "Add " + DeckManager.Instance.GetCalculatedValueString(shieldAmount, calculatedShield) + " shield to character";

        return customDesc;
    }

    public override void OnPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        base.OnPlayCard(cardScript, entityUsedCard);

        MonoBehaviour runner = CombatCardHandler.Instance;

        realTarget = CombatCardHandler.Instance.targetClicked;

        int calculatedShield = Combat.Instance.CalculateEntityShield(shieldAmount, entityUsedCard, realTarget);
        runner.StartCoroutine(Combat.Instance.AdjustTargetHealth(entityUsedCard, realTarget, calculatedShield, false, SystemManager.AdjustNumberModes.SHIELD));

    }

    public override void OnAiPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        base.OnAiPlayCard(cardScript, entityUsedCard);

        MonoBehaviour runner = CombatCardHandler.Instance;

        realTarget = AIManager.Instance.GetRandomAlly(entityUsedCard);

        int calculatedShield = Combat.Instance.CalculateEntityShield(shieldAmount, entityUsedCard, realTarget);
        runner.StartCoroutine(Combat.Instance.AdjustTargetHealth(entityUsedCard, realTarget, calculatedShield, false, SystemManager.AdjustNumberModes.SHIELD));
    }



}
