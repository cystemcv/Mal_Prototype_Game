using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Artifacts_HealingSerum", menuName = "Item/Artifacts/Artifacts_HealingSerum")]
public class Artifacts_HealingSerum : ScriptableItem
{

    public override void Picked(ClassItemData classItem, CardScriptData cardScriptData, GameObject target)
    {
        base.Picked(classItem, cardScriptData, target);

        CharacterManager.Instance.restHealing += 10;
    }

}
