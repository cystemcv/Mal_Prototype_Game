using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Curse_Card_Ghost", menuName = "Card/Curse/Curse_Card_Ghost")]
public class Curse_Card_Ghost : ScriptableCard
{

    public int reduceMana = 0;

    private GameObject realTarget;
    private GameObject entityUsedCardGlobal;

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);


        customDesc += "Does nothing!";

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
      
    }




}
