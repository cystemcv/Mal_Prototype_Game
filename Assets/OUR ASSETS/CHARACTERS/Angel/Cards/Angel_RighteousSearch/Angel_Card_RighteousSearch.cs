using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static SystemManager;

[CreateAssetMenu(fileName = "Angel_Card_RighteousSearch", menuName = "Card/Angel/Angel_Card_RighteousSearch")]
public class Angel_Card_RighteousSearch : ScriptableCard
{

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        customDesc += "Choose 1 random Righteous Named Card";

        return customDesc;
    }

    public override void OnPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnPlayCard(cardScriptData, entityUsedCard);
        Activate();
    }

    public override void OnAiPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayCard(cardScriptData, entityUsedCard);
        Activate();
    }


    public void Activate()
    {
        var allowedClasses = new List<MainClass> { MainClass.COMMON };
        allowedClasses.Add(StaticData.staticCharacter.mainClass);

        List<ScriptableCard> cardList = CardListManager.Instance.ChooseCards(allowedClasses, null, null, null, "Righteous", 3, false);

        CardListManager.Instance.OpenCardListChoice(cardList);

    }


}
