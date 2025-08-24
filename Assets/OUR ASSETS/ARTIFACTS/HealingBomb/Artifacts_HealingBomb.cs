using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Artifacts_HealingBomb", menuName = "Item/Artifacts/Artifacts_HealingBomb")]
public class Artifacts_HealingBomb : ScriptableItem
{

    public int healAmount = 3;

    public override void Activate(ClassItemData classItem, CardScriptData cardScriptData, GameObject target)
    {
        base.Activate(classItem, cardScriptData, target);

        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("PlayerSummon");

        foreach(GameObject summon in gameObjects)
        {
            Combat.Instance.AdjustTargetHealth(null, summon, healAmount, false, SystemManager.AdjustNumberModes.HEAL);
        }

    }

}
