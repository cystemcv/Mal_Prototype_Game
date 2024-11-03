using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Ability_CopyEnemyCard", menuName = "CardAbility/Ability_CopyEnemyCard")]
public class Ability_CopyEnemyCard : ScriptableCardAbility
{


    public override string AbilityDescription(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity)
    {

        string keyword = base.AbilityDescription(cardScript, cardAbilityClass, entity);
        string description = "Copy an enemy card and add it to your hand";

        string final = keyword + description;

        return final;
    }



    public override void OnPlayCard(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity, SystemManager.ControlBy controlBy)
    {

        base.OnPlayCard(cardScript, cardAbilityClass, entity, controlBy);


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
