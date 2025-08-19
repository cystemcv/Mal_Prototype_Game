using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Artifacts_KnightArmor", menuName = "Item/Artifacts/Artifacts_KnightArmor")]
public class Artifacts_KnightArmor : ScriptableItem
{
    [Title("UNIQUE ITEM ABILITY")]
    public int armorMax = 7;
    public int armorTurns = 3;

    private int armorTurnsPassed = 0;
    private bool armorRegenActivated = false;

    public override void Activate(ClassItemData classItem, CardScriptData cardScriptData, GameObject target)
    {



        GameObject character = GameObject.FindGameObjectWithTag("Player");
        EntityClass entityClass = character.GetComponent<EntityClass>();

        //start counting
        if (entityClass.armor < armorMax)
        {
            armorTurnsPassed += 1;
        }
        //otherwise reset counting
        else
        {
            armorTurnsPassed = 0;
        }

        if (armorTurnsPassed >= armorTurns)
        {
            armorRegenActivated = true;
        }
        else
        {
            armorRegenActivated = false;
        }


    }

    public override void Expired(ClassItemData classItem, CardScriptData cardScriptData, GameObject target)
    {
        if (armorRegenActivated == false)
        {
            return;
        }

        MonoBehaviour runner = CombatCardHandler.Instance;

        GameObject character = GameObject.FindGameObjectWithTag("Player");
        EntityClass entityClass = character.GetComponent<EntityClass>();

        if (entityClass.armor < armorMax)
        {
            int remainder = armorMax - entityClass.armor;

            runner.StartCoroutine(Combat.Instance.AdjustTargetHealth(null, character, remainder, false, SystemManager.AdjustNumberModes.ARMOR));

            ItemManager.Instance.AddItemOnActivateOrder(this,  "Added Armor +" + remainder, false);
        }

        armorRegenActivated = false;
        armorTurnsPassed = 0;

    }

    public override void Initialiaze(ClassItemData classItem, CardScriptData cardScriptData, GameObject target)
    {
        GameObject character = GameObject.FindGameObjectWithTag("Player");
        EntityClass entityClass = character.GetComponent<EntityClass>();

        if (entityClass.armor >= armorMax)
        {
            return;
        }

        MonoBehaviour runner = CombatCardHandler.Instance;

        runner.StartCoroutine(Combat.Instance.AdjustTargetHealth(null, character, armorMax, false, SystemManager.AdjustNumberModes.ARMOR));


        ItemManager.Instance.AddItemOnActivateOrder(this, "Added Armor +" + armorMax, false);
    }




}
