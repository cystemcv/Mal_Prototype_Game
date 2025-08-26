using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster_Card_Electrify", menuName = "Card/Monster/Monster_Card_Electrify")]
public class Monster_Card_Electrify : ScriptableCard
{

    public ScriptableHazard scriptableHazard;

    private GameObject realTarget;
    private GameObject entityUsedCardGlobal;

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        customDesc += "Add a <color=#" + SystemManager.Instance.colorGolden + ">" + scriptableHazard.hazardName + "</color> to a zone";
        return customDesc;
    }

    public override void OnPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnPlayCard(cardScriptData, entityUsedCard);

        realTarget = CombatCardHandler.Instance.targetClicked;
        entityUsedCardGlobal = entityUsedCard;

        ExecuteCard(cardScriptData);

    }

    public override void OnAiPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayCard(cardScriptData, entityUsedCard);

        realTarget = entityUsedCard.GetComponent<AIBrain>().targetForCard;
        CombatCardHandler.Instance.posClickedTargeting = Combat.Instance.CheckCardTargets(realTarget, cardScriptData.scriptableCard);
        entityUsedCardGlobal = entityUsedCard;

        ExecuteCard(cardScriptData);

    }



    public void ExecuteCard(CardScriptData cardScriptData)
    {

        if (realTarget == null)
        {
            return;
        }

        MonoBehaviour runner = CombatCardHandler.Instance;
        List<CombatPosition> targets = CombatCardHandler.Instance.posClickedTargeting;

        // Start the coroutine for each hit
        foreach (CombatPosition target in targets)
        {
            Combat.Instance.AddHazard(scriptableHazard, target);
        }

    }

    public override void OnAiPlayTarget(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayTarget(cardScriptData, entityUsedCard);

        realTarget = AIManager.Instance.GetRandomTarget(entityUsedCard);

        entityUsedCard.GetComponent<AIBrain>().targetForCard = realTarget;

    }

}
