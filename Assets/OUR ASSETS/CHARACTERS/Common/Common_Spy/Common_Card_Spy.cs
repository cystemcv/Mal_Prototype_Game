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

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        customDesc += "Play top " + playTopCards + " cards from deck";
        return customDesc;
    }

    public override void OnPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnPlayCard(cardScriptData, entityUsedCard);

        realTarget = CombatCardHandler.Instance.targetClicked;
        entityUsedCardGlobal = entityUsedCard;
        globalCardScript = cardScriptData;

        ExecuteCard();

    }

    public override void OnAiPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayCard(cardScriptData, entityUsedCard);

        realTarget = AIManager.Instance.GetRandomTarget(entityUsedCard);
        entityUsedCardGlobal = entityUsedCard;

        ExecuteCard();

    }

    public void ExecuteCard()
    {

        MonoBehaviour runner = CombatCardHandler.Instance;


        // Start the coroutine for each hit
        runner.StartCoroutine(PlayTopCards());

    }

    public IEnumerator PlayTopCards()
    {
        for(int i=0; i<playTopCards; i++)
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
