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

        customDesc += "Add a <color=#" + SystemManager.Instance.colorGolden + ">" + scriptableHazard.hazardName + "</color> to a zone";
        customDesc += "\n<color=yellow>" + this.scriptableKeywords[0].keywordName + "</color>";
        return customDesc;
    }

    public override void OnPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        base.OnPlayCard(cardScript, entityUsedCard);

        realTarget = CombatCardHandler.Instance.targetClicked;
        entityUsedCardGlobal = entityUsedCard;

        ExecuteCard();

    }

    public override void OnAiPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        base.OnAiPlayCard(cardScript, entityUsedCard);

        realTarget = entityUsedCard.GetComponent<AIBrain>().targetForCard;
        entityUsedCardGlobal = entityUsedCard;

        ExecuteCard();

    }

    public override void OnAiPlayTarget(CardScript cardScript, GameObject entityUsedCard)
    {
        base.OnAiPlayTarget(cardScript, entityUsedCard);

        string tag = "";
        if (SystemManager.Instance.GetEnemyTagsList().Contains(entityUsedCard.tag))
        {
            tag = "PlayerPos";
        }
        else
        {
            tag = "EnemyPos";
        }

        if (Combat.Instance.CheckIfSpawnPosAreFull(tag))
        {
            return;
        }

        CombatPosition combatPosition = Combat.Instance.GetSpawnPosition(tag);
        realTarget = combatPosition.position.gameObject.transform.Find("Visual").gameObject;
        entityUsedCard.GetComponent<AIBrain>().targetForCard = realTarget;
    }

    public void ExecuteCard()
    {

        if (realTarget == null)
        {
            return;
        }

        MonoBehaviour runner = CombatCardHandler.Instance;

        CombatPosition combatPosition = Combat.Instance.GetCombatPosition(realTarget);

        Combat.Instance.AddHazard(scriptableHazard, combatPosition);

    }

}
