using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Common_Card_Spy", menuName = "Card/Common/Common_Card_Spy")]
public class Common_Card_Spy : ScriptableCard
{

    public int playTopCards = 1;

    private GameObject realTarget;
    private GameObject entityUsedCardGlobal;

    public CardScriptData globalCardScript;

    private int scalingPlayTopCards = 0;

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        scalingPlayTopCards = playTopCards + ((scalingLevelCardValue * cardScriptData.scalingLevelValue) / 2);

        customDesc += "Play top " + scalingPlayTopCards + " cards from deck";
        return customDesc;
    }

    public override void OnPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnPlayCard(cardScriptData, entityUsedCard);

        realTarget = CombatCardHandler.Instance.targetClicked;
        entityUsedCardGlobal = entityUsedCard;
        globalCardScript = cardScriptData;

        ExecuteCard(cardScriptData);

    }

    public override void OnAiPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayCard(cardScriptData, entityUsedCard);

        realTarget = AIManager.Instance.GetRandomTarget(entityUsedCard);
        entityUsedCardGlobal = entityUsedCard;

        ExecuteCard(cardScriptData);

    }

    public void ExecuteCard(CardScriptData cardScriptData)
    {

        MonoBehaviour runner = CombatCardHandler.Instance;


        // Start the coroutine for each hit
        runner.StartCoroutine(PlayTopCards(cardScriptData));

    }

    public IEnumerator PlayTopCards(CardScriptData cardScriptData)
    {

        scalingPlayTopCards = playTopCards + ((scalingLevelCardValue * cardScriptData.scalingLevelValue) / 2);

        for (int i=0; i< scalingPlayTopCards; i++)
        {
            DeckManager.Instance.PlayCardFromCombatDeck(0);
            yield return new WaitForSeconds(1f);
        }


        DeckManager.Instance.savedPlayedCard.cardScriptData = globalCardScript;

        //destroy the prefab
        if (DeckManager.Instance.savedPlayedCard.cardScriptData.scriptableCard.cardType == SystemManager.CardType.Focus)
        {
          DeckManager.Instance.DestroyPlayedCard(SystemManager.CardThrow.BANISH);
        }
        else
        {
          DeckManager.Instance.DestroyPlayedCard(SystemManager.CardThrow.DISCARD);
        }
    }

}
