using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Curse_Card_BlueMana", menuName = "Card/Curse/Curse_Card_BlueMana")]
public class Curse_Card_BlueMana : ScriptableCard
{

    public int reduceMana = 0;

    private GameObject realTarget;
    private GameObject entityUsedCardGlobal;

    public override string OnCardDescription(CardScript cardScript, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScript, entityUsedCard);


        customDesc += "When you draw this card reduce your mana by " + reduceMana;

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
        Combat.Instance.ModifyMana(-1 * reduceMana);

    }




}
