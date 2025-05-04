using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Thief_MasterfullPlan", menuName = "Item/CompanionPassives/Thief_MasterfullPlan")]
public class Thief_MasterfullPlan : ScriptableItem
{
    [Title("UNIQUE ITEM ABILITY")]
    public int reduceCd = 1; 

    public override void Activate(ClassItem classItem, CardScript cardScript)
    {
        Combat.Instance.reduceCompanionAbilityCd = reduceCd * classItem.level;
    }

    public override void Expired(ClassItem classItem, CardScript cardScript)
    {
        Combat.Instance.reduceCompanionAbilityCd = 0;
    }


}
