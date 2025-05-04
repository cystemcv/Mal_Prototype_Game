using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Thief_BootsOfSpeed", menuName = "Item/CompanionPassives/Thief_BootsOfSpeed")]
public class Thief_BootsOfSpeed : ScriptableItem
{
    [Title("UNIQUE ITEM ABILITY")]
    public int reduceCd = 1; 

    public override void Activate(ClassItem classItem, CardScript cardScript)
    {
        Combat.Instance.reduceCompanionStartingCd = reduceCd * classItem.level;
    }

    public override void Expired(ClassItem classItem, CardScript cardScript)
    {
        Combat.Instance.reduceCompanionStartingCd = 0;
    }


}
