using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Curse_Card_Madness", menuName = "Card/Curse/Curse_Card_Madness")]
public class Curse_Card_Madness : ScriptableCard
{

    public int damageAmount = 0;
    public int multiHits = 0;

    private GameObject realTarget;
    private GameObject entityUsedCardGlobal;

    public override string OnCardDescription(CardScript cardScript, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScript, entityUsedCard);


        customDesc += "When you draw this card play 1 random card! (Random Target)";

        return customDesc;
    }

    public override void OnPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {

    }

    public override void OnAiPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {

    }

    public override void OnDrawCard(CardScript cardScript)
    {
        ExecuteCard(cardScript);
    }

    public void ExecuteCard(CardScript cardScript)
    {
        Combat.Instance.discardEffectCardList.Add(cardScript);
    }


    public override void OnDelayEffect(CardScript cardScript)
    {
        DeckManager.Instance.PlayCardFromHandRandom();
    }



}
