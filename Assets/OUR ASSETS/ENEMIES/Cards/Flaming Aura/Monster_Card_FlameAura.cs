using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster_Card_FlameAura", menuName = "Card/Monster/Monster_Card_FlameAura")]
public class Monster_Card_FlameAura : ScriptableCard
{

    public int howManyBurnCards = 0;
    public ScriptableCard statusBurn;

    private int scalingBurnCards = 0;

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        //scaling
        scalingBurnCards = howManyBurnCards + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        customDesc += "Add " + scalingBurnCards + " Status Burn Cards to the Hero Deck and shuffle it";

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
        scalingBurnCards = howManyBurnCards + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        //add the cards
        for (int i=0;i< scalingBurnCards; i++)
        {
            CardScriptData burnCard = new CardScriptData();
            burnCard.scriptableCard = statusBurn;
            DeckManager.Instance.AddCardOnCombatDeck(burnCard);
        }

        //shuffle combat deck
        DeckManager.Instance.ShuffleDeck(DeckManager.Instance.combatDeck);



    }

}
