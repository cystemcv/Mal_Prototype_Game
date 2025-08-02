using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Thief_Card_Copy", menuName = "Card/Thief/Thief_Card_Copy")]
public class Thief_Card_Copy : ScriptableCard
{

    public int shieldAmount = 0;

    private GameObject realTarget;

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        int calculatedShield = (Combat.Instance == null) ? shieldAmount : Combat.Instance.CalculateEntityShield(shieldAmount, entityUsedCard, realTarget);
        customDesc += "Copy a monster card and add it to your hand";

        return customDesc;
    }

    public override void OnPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        CopyCard(cardScriptData, entityUsedCard);

    }

    public override void OnAiPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {

        CopyCard(cardScriptData, entityUsedCard);

    }

    public override void OnInitializeCard(CardScriptData cardScriptData)
    {


    }

    public void CopyCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnPlayCard(cardScriptData, entityUsedCard);


        //get the card
        //generate cardScript
        CardScriptData cardScriptDataTemp = new CardScriptData();
        //get a random card from enemies

        AIBrain aIBrain = null;

        if (this.targetEntityTagList.Count <= 0)
        {
            //get all enemies
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            //get random enemy
            int index = UnityEngine.Random.Range(0, enemies.Length);
            aIBrain = enemies[index].GetComponent<AIBrain>();
        }
        else
        {
            realTarget = CombatCardHandler.Instance.targetClicked;
            aIBrain = realTarget.GetComponent<AIBrain>();
        }


        cardScriptDataTemp.scriptableCard = aIBrain.scriptableCardToUse;

        Debug.Log("cardScriptTemp.scriptableCard : ", cardScriptDataTemp.scriptableCard);

        //add it to the hand
        DeckManager.Instance.handCards.Add(cardScriptDataTemp);

        //instantiate the card
        DeckManager.Instance.InitializeCardPrefab(cardScriptDataTemp, UI_Combat.Instance.HAND, true, false);

        //rearrange hand
        HandManager.Instance.SetHandCards();
    }

}
