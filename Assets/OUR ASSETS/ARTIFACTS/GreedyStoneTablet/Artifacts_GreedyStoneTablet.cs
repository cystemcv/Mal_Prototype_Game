using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Artifacts_GreedyStoneTablet", menuName = "Item/Artifacts/Artifacts_GreedyStoneTablet")]
public class Artifacts_GreedyStoneTablet : ScriptableItem
{
    [Title("UNIQUE ITEM ABILITY")]
    public int maxCardsToPlay = 3;
    public int cardsToDraw = 1;

    private int cardsPlayed = 0;

    public override void Activate(ClassItem classItem)
    {

       if(StaticData.artifact_CardScript.scriptableCard.cardType == SystemManager.CardType.Attack)
        {
            cardsPlayed += 1;
        }

        if (cardsPlayed >= maxCardsToPlay)
        {

            float waitAbilitiesTime = 0f;

            foreach (CardAbilityClass scriptableCardAbility in StaticData.artifact_CardScript.scriptableCard.cardAbilityClass)
            {
                waitAbilitiesTime += scriptableCardAbility.waitForAbility;
            }

            waitAbilitiesTime += 0.05f;

               MonoBehaviour runner = CombatCardHandler.Instance; // Ensure this is a valid MonoBehaviour in your scene
                                                               //hit at least one time if its 0

            runner.StartCoroutine(DeckManager.Instance.DrawMultipleCards(cardsToDraw, waitAbilitiesTime));

            //reset back to 0
            cardsPlayed = 0;
        }


    }

    public override void Expired(ClassItem classItem)
    {
        cardsPlayed = 0;
    }




}
