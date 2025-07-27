using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Common_Card_Mine", menuName = "Card/Common/Common_Card_Mine")]
public class Common_Card_Mine : ScriptableCard
{

    public ScriptableHazard scriptableHazard;

    private GameObject realTarget;
    private GameObject entityUsedCardGlobal;

    public override string OnCardDescription(CardScript cardScript, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScript, entityUsedCard);

        customDesc += "Add a <color=#BF40BF>" + scriptableHazard.hazardName + "</color> to a zone";

        return customDesc;
    }

    public override void OnPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        //base.OnPlayCard(cardScript, entityUsedCard);

        realTarget = CombatCardHandler.Instance.targetClicked;
        entityUsedCardGlobal = entityUsedCard;

        ExecuteCard();

    }

    public override void OnAiPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        //base.OnAiPlayCard(cardScript, entityUsedCard);

        realTarget = AIManager.Instance.GetRandomTarget(entityUsedCard);
        entityUsedCardGlobal = entityUsedCard;

        ExecuteCard();

    }

    public void ExecuteCard()
    {



        MonoBehaviour runner = CombatCardHandler.Instance;

        CombatPosition combatPosition = Combat.Instance.GetCombatPosition(realTarget);

        Combat.Instance.AddHazard(scriptableHazard, combatPosition);

    }

}
