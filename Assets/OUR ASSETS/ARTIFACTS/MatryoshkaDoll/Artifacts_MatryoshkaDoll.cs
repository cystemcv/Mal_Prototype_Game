using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Artifacts_MatryoshkaDoll", menuName = "Item/Artifacts/Artifacts_MatryoshkaDoll")]
public class Artifacts_MatryoshkaDoll : ScriptableItem
{

    public override void Picked(ClassItemData classItem, CardScriptData cardScriptData, GameObject target)
    {
        base.Picked(classItem, cardScriptData, target);

        ItemManager.Instance.artifactsToChooseLimit += 1;
    }

}
