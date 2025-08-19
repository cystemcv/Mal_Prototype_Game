using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Artifacts_TimeMachine", menuName = "Item/Artifacts/Artifacts_TimeMachine")]
public class Artifacts_TimeMachine : ScriptableItem
{


    public override void Picked(ClassItemData classItem, CardScriptData cardScriptData, GameObject target)
    {

        if (CustomDungeonGenerator.Instance.scalingLevel > 0)
        {
            CustomDungeonGenerator.Instance.scalingLevel -= 1;
        }
        else
        {
            CustomDungeonGenerator.Instance.stepsTaken = 0;
        }

        CustomDungeonGenerator.Instance.SetUpScalingText();
    }



}
