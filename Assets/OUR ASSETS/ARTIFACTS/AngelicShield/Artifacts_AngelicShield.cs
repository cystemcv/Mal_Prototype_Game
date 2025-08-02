using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Artifacts_AngelicShield", menuName = "Item/Artifacts/Artifacts_AngelicShield")]
public class Artifacts_AngelicShield : ScriptableItem
{
    [Title("UNIQUE ITEM ABILITY")]
    public int shieldMax = 10;
    public int shieldDescrease = 2;

    private int shieldAdded = 10;

    public override void Activate(ClassItemData classItem, CardScriptData cardScriptData)
    {

        MonoBehaviour runner = CombatCardHandler.Instance;

        if (shieldAdded <= 0)
        {
            return;
        }

        GameObject character = GameObject.FindGameObjectWithTag("Player");

        runner.StartCoroutine(Combat.Instance.AdjustTargetHealth(null, character, shieldAdded, false, SystemManager.AdjustNumberModes.SHIELD));

        ItemManager.Instance.AddItemOnActivateOrder(this, "+" + shieldAdded + " shield", false);
        //decrease it
        shieldAdded -= shieldDescrease;

    

    }

    public override void Initialiaze(ClassItemData classItem, CardScriptData cardScriptData)
    {
        shieldAdded = shieldMax;
    }




}
