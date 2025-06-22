using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Thief_Card_Steal", menuName = "Card/Thief/Thief_Card_Steal")]
public class Thief_Card_Steal : ScriptableCard
{

    public int shieldAmount = 0;

    private GameObject realTarget;

    public override string OnCardDescription(CardScript cardScript, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScript, entityUsedCard);

        customDesc += "Steal a monster card and add it to your hand, forcing the enemy to advance to the next turn card!";

        return customDesc;
    }

    public override void OnPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        StealCard(cardScript, entityUsedCard);

    }

    public override void OnAiPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {

        StealCard(cardScript, entityUsedCard);

    }

    public override void OnInitializeCard(CardScript cardScript)
    {


    }

    public void StealCard(CardScript cardScript, GameObject entityUsedCard)
    {
        base.OnPlayCard(cardScript, entityUsedCard);


        //get the card
        //generate cardScript
        CardScript cardScriptTemp = new CardScript();
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


        cardScriptTemp.scriptableCard = aIBrain.cardScriptList[aIBrain.aiLogicStep];

        Debug.Log("cardScriptTemp.scriptableCard : ", cardScriptTemp.scriptableCard);

        //add it to the hand
        DeckManager.Instance.handCards.Add(cardScriptTemp);

        //instantiate the card
        DeckManager.Instance.InitializeCardPrefab(cardScriptTemp, UI_Combat.Instance.HAND, true, false);

        //rearrange hand
        HandManager.Instance.SetHandCards();

    

        if (aIBrain != null)
        {
            aIBrain.IncreaseAiStep();
            aIBrain.GenerateIntend();
        }
    }

}
