using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Artifacts_RegenArmor", menuName = "Item/Artifacts/Artifacts_RegenArmor")]
public class Artifacts_RegenArmor : ScriptableItem
{
    [Title("UNIQUE ITEM ABILITY")]
    public int regenAmount = 2;
    public int regenOnTurn = 2;
    public int regenMultiplierArmor = 4;

    public override void Activate(ClassItemData classItem, CardScriptData cardScriptData)
    {

        if (Combat.Instance.turns != regenOnTurn)
        {
            return;
        }

        GameObject character = GameObject.FindGameObjectWithTag("Player");
        EntityClass entityClass = character.GetComponent<EntityClass>();

        MonoBehaviour runner = CombatCardHandler.Instance;

        if (entityClass.health >= entityClass.maxHealth)
        {
            //then increase armor
            runner.StartCoroutine(Combat.Instance.AdjustTargetHealth(null, character, regenAmount * regenMultiplierArmor, false, SystemManager.AdjustNumberModes.ARMOR));
            ItemManager.Instance.AddItemOnActivateOrder(this, "Armor +" + regenAmount * regenMultiplierArmor, false);
        }
        else
        {
            runner.StartCoroutine(Combat.Instance.AdjustTargetHealth(null, character, regenAmount, false, SystemManager.AdjustNumberModes.HEAL));
            ItemManager.Instance.AddItemOnActivateOrder(this, "Heal +" + regenAmount, false);
        }




    }

    public override void Initialiaze(ClassItemData classItem, CardScriptData cardScriptData)
    {

    }




}
