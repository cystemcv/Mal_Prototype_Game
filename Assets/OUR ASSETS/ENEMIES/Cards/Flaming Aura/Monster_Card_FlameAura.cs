using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster_Card_FlameAura", menuName = "Card/Monster/Monster_Card_FlameAura")]
public class Monster_Card_FlameAura : ScriptableCard
{

    public ScriptableCard statusBurn;

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        customDesc += "Add 3 Status Burn Cards to the Hero Deck and shuffle it";

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
        //add the cards
        for (int i=0;i<3;i++)
        {
            CardScriptData burnCard = new CardScriptData();
            burnCard.scriptableCard = statusBurn;
            DeckManager.Instance.AddCardOnCombatDeck(burnCard);
        }

        //shuffle combat deck
        DeckManager.Instance.ShuffleDeck(DeckManager.Instance.combatDeck);



    }

}
