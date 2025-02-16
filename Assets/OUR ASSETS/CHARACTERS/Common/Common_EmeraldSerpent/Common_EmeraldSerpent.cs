using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Common_EmeraldSerpent", menuName = "Card/Common/Common_EmeraldSerpent")]
public class Common_EmeraldSerpent : ScriptableCard
{

    public int shieldAmount = 10;
    public int armorAmount = 5;

    private GameObject realTarget;
    private GameObject entityUsedCardGlobal;

    public override string OnCardDescription(CardScript cardScript, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScript, entityUsedCard);

        int calculatedShield = Combat.Instance.CalculateEntityShield(shieldAmount, entityUsedCard, realTarget);
        int calculatedArmor = Combat.Instance.CalculateEntityArmor(armorAmount, entityUsedCard, realTarget);
        customDesc += "Add " + DeckManager.Instance.GetCalculatedValueString(armorAmount, calculatedArmor) + " armor to character on Odd Turn Number!<br>";
        customDesc += "Add " + DeckManager.Instance.GetCalculatedValueString(shieldAmount, calculatedShield) + " shield to character on Even Turn Number!<br>";
        customDesc += "<color=yellow>" + scriptableKeywords[0].keywordName + "</color>";

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
   
      
        if (IsOdd(Combat.Instance.turns))
        {
            int calculatedArmor = Combat.Instance.CalculateEntityArmor(armorAmount, entityUsedCardGlobal, realTarget);
            Combat.Instance.AdjustTargetHealth(null, entityUsedCardGlobal, calculatedArmor, false, SystemManager.AdjustNumberModes.ARMOR);
        }
        else
        {
            int calculatedShield = Combat.Instance.CalculateEntityShield(shieldAmount, entityUsedCardGlobal, realTarget);
            Combat.Instance.AdjustTargetHealth(null, entityUsedCardGlobal, calculatedShield, false, SystemManager.AdjustNumberModes.SHIELD);
        }
    }

    bool IsOdd(int number)
    {
        return number % 2 != 0;
    }



}
