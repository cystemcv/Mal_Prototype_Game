using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Curse_Card_Ghost", menuName = "Card/Curse/Curse_Card_Ghost")]
public class Curse_Card_Ghost : ScriptableCard
{

    public int reduceMana = 0;

    private GameObject realTarget;
    private GameObject entityUsedCardGlobal;

    public override string OnCardDescription(CardScript cardScript, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScript, entityUsedCard);


        customDesc += "Does nothing!";

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
      
    }




}
