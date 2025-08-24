using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Artifacts_PackOfCards", menuName = "Item/Artifacts/Artifacts_PackOfCards")]
public class Artifacts_PackOfCards : ScriptableItem
{

    public override void Picked(ClassItemData classItem, CardScriptData cardScriptData, GameObject target)
    {
        base.Picked(classItem, cardScriptData, target);

        CardListManager.Instance.cardsToChooseLimit += 1;
    }

}
