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

    private int scalingShield = 0;
    private int scalingArmor = 0;

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        //scaling
        scalingShield = shieldAmount + (scalingLevelCardValue * cardScriptData.scalingLevelValue);
        scalingArmor = armorAmount + ((scalingLevelCardValue * cardScriptData.scalingLevelValue) / 2);

        int calculatedShield = (Combat.Instance == null) ? scalingShield : Combat.Instance.CalculateEntityShield(scalingShield, entityUsedCard, realTarget);
        int calculatedArmor =  (Combat.Instance == null) ? scalingArmor : Combat.Instance.CalculateEntityArmor(scalingArmor, entityUsedCard, realTarget);
        customDesc += "Add " + DeckManager.Instance.GetCalculatedValueString(scalingArmor, calculatedArmor) + " armor to character on Odd Turn Number!<br>";
        customDesc += "Add " + DeckManager.Instance.GetCalculatedValueString(scalingShield, calculatedShield) + " shield to character on Even Turn Number!<br>";
        customDesc += "<color=yellow>" + scriptableKeywords[0].keywordName + "</color>";

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
        entityUsedCardGlobal = entityUsedCard;

        ExecuteCard(cardScriptData);

    }

    public override void OnAiPlayTarget(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayTarget(cardScriptData, entityUsedCard);
        realTarget = AIManager.Instance.GetRandomAlly(entityUsedCard);
        entityUsedCard.GetComponent<AIBrain>().targetForCard = realTarget;
    }

    public void ExecuteCard(CardScriptData cardScriptData)
    {

        MonoBehaviour runner = CombatCardHandler.Instance; // Ensure this is a valid MonoBehaviour in your scene
                                                           //hit at least one time if its 0

        //scaling
        scalingShield = shieldAmount + (scalingLevelCardValue * cardScriptData.scalingLevelValue);
        scalingArmor = armorAmount + ((scalingLevelCardValue * cardScriptData.scalingLevelValue) / 2);

        if (IsOdd(Combat.Instance.turns))
        {
            int calculatedArmor = (Combat.Instance == null) ? scalingArmor : Combat.Instance.CalculateEntityArmor(scalingArmor, entityUsedCardGlobal, realTarget);
            runner.StartCoroutine(Combat.Instance.AdjustTargetHealth(null, entityUsedCardGlobal, calculatedArmor, false, SystemManager.AdjustNumberModes.ARMOR));
        }
        else
        {
            int calculatedShield = (Combat.Instance == null) ? scalingShield : Combat.Instance.CalculateEntityShield(scalingShield, entityUsedCardGlobal, realTarget);
            runner.StartCoroutine(Combat.Instance.AdjustTargetHealth(null, entityUsedCardGlobal, calculatedShield, false, SystemManager.AdjustNumberModes.SHIELD));
        }
    }

    bool IsOdd(int number)
    {
        return number % 2 != 0;
    }



}
