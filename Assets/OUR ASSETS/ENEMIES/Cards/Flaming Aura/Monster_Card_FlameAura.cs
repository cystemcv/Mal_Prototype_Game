using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster_Card_FlameAura", menuName = "Card/Monster/Monster_Card_FlameAura")]
public class Monster_Card_FlameAura : ScriptableCard
{

    public ScriptableCard statusBurn;

    public override string OnCardDescription(CardScript cardScript, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScript, entityUsedCard);

        customDesc += "Add 3 Status Burn Cards to the Hero Deck and shuffle it";

        return customDesc;
    }

    public override void OnPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        base.OnPlayCard(cardScript, entityUsedCard);
        PlayCard(cardScript, entityUsedCard);
    }

    public override void OnAiPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        base.OnAiPlayCard(cardScript, entityUsedCard);
        PlayCard(cardScript, entityUsedCard);
    }

    public void PlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        //add the cards
        for (int i=0;i<3;i++)
        {
            CardScript burnCard = new CardScript();
            burnCard.scriptableCard = statusBurn;
            DeckManager.Instance.AddCardOnCombatDeck(burnCard);
        }

        //shuffle combat deck
        DeckManager.Instance.ShuffleDeck(DeckManager.Instance.combatDeck);



    }

}
