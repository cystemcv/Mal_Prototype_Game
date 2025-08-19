using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Artifacts_Chicken", menuName = "Item/Artifacts/Artifacts_Chicken")]
public class Artifacts_Chicken : ScriptableItem
{

    public int chancePerc = 10;
    public int healAdded = 6;

    public override void Activate(ClassItemData classItem, CardScriptData cardScriptData, GameObject target)
    {
        MonoBehaviour runner = CombatCardHandler.Instance;

        int randomChance = UnityEngine.Random.Range(0, 100);

        if (randomChance > chancePerc)
        {
            return;
        }

        GameObject character = GameObject.FindGameObjectWithTag("Player");

        runner.StartCoroutine(Combat.Instance.AdjustTargetHealth(null, character, healAdded, false, SystemManager.AdjustNumberModes.HEAL));


        ItemManager.Instance.AddItemOnActivateOrder(this, "Heal +" + healAdded, false);

    }
}
