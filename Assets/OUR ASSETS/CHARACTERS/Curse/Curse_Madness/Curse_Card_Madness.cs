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

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);


        customDesc += "When you draw this card play 1 random card! (Random Target)";

        return customDesc;
    }

    public override void OnPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {

    }

    public override void OnAiPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {

    }

    public override void OnDrawCard(CardScriptData cardScriptData)
    {
        ExecuteCard(cardScriptData);
    }

    public void ExecuteCard(CardScriptData cardScriptData)
    {
        Combat.Instance.discardEffectCardList.Add(cardScriptData);
    }


    public override void OnDelayEffect(CardScriptData cardScriptData)
    {
        DeckManager.Instance.PlayCardFromHandRandom();
    }



}
