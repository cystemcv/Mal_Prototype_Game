using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Angel_Card_RighteousLight", menuName = "Card/Angel/Angel_Card_RighteousLight")]
public class Angel_Card_RighteousLight : ScriptableCard
{

    public int drawCards = 0;

    private GameObject realTarget;
    private GameObject entityUsedCardGlobal;

    private int scalingDraw = 0;

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        //scaling
        scalingDraw = drawCards + ((scalingLevelCardValue * cardScriptData.scalingLevelValue) / 2);

        customDesc += "Draw " + scalingDraw + " cards from deck";
        return customDesc;
    }

    public override void OnPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        realTarget = CombatCardHandler.Instance.targetClicked;

        base.OnPlayCard(cardScriptData, entityUsedCard);

  
        MonoBehaviour runner = CombatCardHandler.Instance; // Ensure this is a valid MonoBehaviour in your scene
                                                           //hit at least one time if its 0

        //scaling
        scalingDraw = drawCards + ((scalingLevelCardValue * cardScriptData.scalingLevelValue) / 2);

        runner.StartCoroutine(DeckManager.Instance.DrawMultipleCards(scalingDraw, 0));


    }

    public override void OnAiPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        realTarget = AIManager.Instance.GetRandomAlly(entityUsedCard); ;

        base.OnPlayCard(cardScriptData, entityUsedCard);

        MonoBehaviour runner = CombatCardHandler.Instance; // Ensure this is a valid MonoBehaviour in your scene
                                                           //hit at least one time if its 0

        //scaling
        scalingDraw = drawCards + ((scalingLevelCardValue * cardScriptData.scalingLevelValue) / 2);

        runner.StartCoroutine(DeckManager.Instance.DrawMultipleCards(scalingDraw, 0));
    }






}
