using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Common_RubySerpent", menuName = "Card/Common/Common_RubySerpent")]
public class Common_RubySerpent : ScriptableCard
{

    public int damageAmount = 18;
    public ScriptableBuffDebuff burn;
    public int burnAmount = 4;

    private GameObject realTarget;
    private GameObject entityUsedCardGlobal;

    public override string OnCardDescription(CardScript cardScript, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScript, entityUsedCard);

        int calculatedDamage = (Combat.Instance == null) ? damageAmount : Combat.Instance.CalculateEntityDmg(damageAmount, entityUsedCard, realTarget);
        customDesc += "Deal " + DeckManager.Instance.GetCalculatedValueString(damageAmount, calculatedDamage) + " to an Enemy on Odd Turn Number!<br>";
        customDesc += "Add x" + burnAmount + " " + BuffSystemManager.Instance.GetBuffDebuffColor(burn) + " to an Enemy on Even Turn Number!<br>";
        customDesc += "<color=yellow>" + scriptableKeywords[0].keywordName + "</color>";

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

        realTarget = AIManager.Instance.GetRandomTarget(entityUsedCard);
        entityUsedCardGlobal = entityUsedCard;

        ExecuteCard();

    }

    public void ExecuteCard()
    {
        MonoBehaviour runner = CombatCardHandler.Instance;

        if (IsOdd(Combat.Instance.turns))
        {
            int calculatedDamage = (Combat.Instance == null) ? damageAmount : Combat.Instance.CalculateEntityDmg(damageAmount, entityUsedCardGlobal, realTarget);
            runner.StartCoroutine(Combat.Instance.AdjustTargetHealth(entityUsedCardGlobal, realTarget, calculatedDamage, false, SystemManager.AdjustNumberModes.ATTACK));
        }
        else
        {
            BuffSystemManager.Instance.AddBuffDebuff(realTarget, burn, burnAmount);
        }
    }

    bool IsOdd(int number)
    {
        return number % 2 != 0;
    }



}
