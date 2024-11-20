using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Artifacts_CrescendoDial", menuName = "Item/Artifacts/Artifacts_CrescendoDial")]
public class Artifacts_CrescendoDial : ScriptableItem
{
    [Title("UNIQUE ITEM ABILITY")]
    public int maxCardsToPlay = 3;
    public int cardsToDraw = 1;

    private int cardsPlayed = 0;

    public override void Activate(ClassItem classItem)
    {


        //BuffSystemManager.Instance.AddBuffDebuffToTarget(this, realTarget, 3);

        ////get the buff or debuff to do things
        //BuffDebuffClass buffDebuffClass = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(realTarget, this.scriptableBuffDebuff.nameID);

        ////increase the variable that will store the temp attack
        //buffDebuffClass.tempVariable += DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList);

        ////increase character attack
        //EntityClass entityClass = realTarget.GetComponent<EntityClass>();
        //entityClass.attack += DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList);


    }

    public override void Expired(ClassItem classItem)
    {
 
    }




}
