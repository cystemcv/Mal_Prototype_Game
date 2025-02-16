using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Thief_Card_Copy", menuName = "Card/Thief/Thief_Card_Copy")]
public class Thief_Card_Copy : ScriptableCard
{

    public int shieldAmount = 0;

    private GameObject realTarget;

    public override string OnCardDescription(CardScript cardScript, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScript, entityUsedCard);

        int calculatedShield = Combat.Instance.CalculateEntityShield(shieldAmount, entityUsedCard, realTarget);
        customDesc += "Copy a monster card and add it to your hand";

        return customDesc;
    }

    public override void OnPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        CopyCard(cardScript, entityUsedCard);

    }

    public override void OnAiPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {

        CopyCard(cardScript, entityUsedCard);

    }

    public override void OnInitializeCard(CardScript cardScript)
    {


    }

    public void CopyCard(CardScript cardScript, GameObject entityUsedCard)
    {
        base.OnPlayCard(cardScript, entityUsedCard);


        //get a random card from enemies


        //get all enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        //get random enemy
        int index = UnityEngine.Random.Range(0, enemies.Length);

        //get the card
        //generate cardScript
        CardScript cardScriptTemp = new CardScript();
        AIBrain aIBrain = enemies[index].GetComponent<AIBrain>();



        cardScriptTemp.scriptableCard = aIBrain.cardScriptList[aIBrain.aiLogicStep];

        Debug.Log("cardScriptTemp.scriptableCard : ", cardScriptTemp.scriptableCard);

        //add it to the hand
        DeckManager.Instance.handCards.Add(cardScriptTemp);

        //instantiate the card
        DeckManager.Instance.InitializeCardPrefab(cardScriptTemp, UI_Combat.Instance.HAND, true, false);

        //rearrange hand
        HandManager.Instance.SetHandCards();
    }

}
