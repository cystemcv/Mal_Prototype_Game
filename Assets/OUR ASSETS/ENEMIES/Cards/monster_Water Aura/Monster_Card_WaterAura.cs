using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster_Card_WaterAura", menuName = "Card/Monster/Monster_Card_WaterAura")]
public class Monster_Card_WaterAura : ScriptableCard
{

    public int howManyCards = 0;
    public ScriptableCard statusWet;

    private int scalingCards = 0;

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        //scaling
        scalingCards = howManyCards + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        customDesc += "Add " + scalingCards + " Status Wet Cards to the Hero Deck and shuffle it";

        return customDesc;
    }

    public override void OnPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnPlayCard(cardScriptData, entityUsedCard);
        PlayCard(cardScriptData, entityUsedCard);
    }

    public override void OnAiPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayCard(cardScriptData, entityUsedCard);
        PlayCard(cardScriptData, entityUsedCard);
    }

    public void PlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {

        //scaling
        scalingCards = howManyCards + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        //add the cards
        for (int i=0;i< scalingCards; i++)
        {
            CardScriptData burnCard = new CardScriptData();
            burnCard.scriptableCard = statusWet;
            DeckManager.Instance.AddCardOnCombatDeck(burnCard);
        }

        //shuffle combat deck
        DeckManager.Instance.ShuffleDeck(DeckManager.Instance.combatDeck);



    }

}
