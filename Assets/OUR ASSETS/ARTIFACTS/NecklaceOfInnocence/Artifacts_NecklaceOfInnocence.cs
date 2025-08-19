using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Artifacts_NecklaceOfInnocence", menuName = "Item/Artifacts/Artifacts_NecklaceOfInnocence")]
public class Artifacts_NecklaceOfInnocence : ScriptableItem
{
    [Title("UNIQUE ITEM ABILITY")]
    public int armor = 6;


    public override void Activate(ClassItemData classItem, CardScriptData cardScriptData, GameObject target)
    {

        MonoBehaviour runner = CombatCardHandler.Instance;

        GameObject character = GameObject.FindGameObjectWithTag("Player");

        runner.StartCoroutine(Combat.Instance.AdjustTargetHealth(null, character, armor, false, SystemManager.AdjustNumberModes.ARMOR));

        ItemManager.Instance.AddItemOnActivateOrder(this, "+" + armor + " armor", false);

    }






}
