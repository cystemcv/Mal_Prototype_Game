using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Thief_ThiefMastery", menuName = "Item/CompanionPassives/Thief_ThiefMastery")]
public class Thief_ThiefMastery : ScriptableItem
{
    [Title("UNIQUE ITEM ABILITY")]
    public ScriptableCard copyCard;

    public override void Activate(ClassItem classItem)
    {

        if (classItem.level == 1)
        {
            //add tags to the copy ability
            copyCard.targetEntityTagList.Add(SystemManager.EntityTag.Enemy);
        }
        else if (classItem.level == 2)
        {
            //add tags to the copy ability
            copyCard.targetEntityTagList.Add(SystemManager.EntityTag.Enemy);
            copyCard.targetEntityTagList.Add(SystemManager.EntityTag.EnemySummon);
        }
        else if (classItem.level == 3)
        {
            //add tags to the copy ability
            copyCard.targetEntityTagList.Add(SystemManager.EntityTag.Enemy);
            copyCard.targetEntityTagList.Add(SystemManager.EntityTag.EnemySummon);
            copyCard.targetEntityTagList.Add(SystemManager.EntityTag.PlayerSummon);
        }

    }

    public override void Expired(ClassItem classItem)
    {
        copyCard.targetEntityTagList.Clear();
    }


}
