using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Artifacts_RainTotem", menuName = "Item/Artifacts/Artifacts_RainTotem")]
public class Artifacts_RainTotem : ScriptableItem
{
    [Title("UNIQUE ITEM ABILITY")]
    public ScriptableBuffDebuff scriptableBuffDebuff;
    public int amount = 1;
    public int startTurn = 3;

    public override void Activate(ClassItemData classItem, CardScript cardScript)
    {

        if (Combat.Instance.turns < startTurn)
        {
            return;
        }

        List<GameObject> allEntities = SystemManager.Instance.FindGameObjectsWithTags(SystemManager.Instance.GetEnemyTagsList());

        foreach (GameObject entity in allEntities)
        {
            BuffSystemManager.Instance.AddBuffDebuff(entity, scriptableBuffDebuff, amount);
        }

        ItemManager.Instance.AddItemOnActivateOrder(this, this.itemName + " Activated! 1 <color=yellow>wet</color> for all enemies!", false);

    }

    public override void Initialiaze(ClassItemData classItem, CardScript cardScript)
    {

    }




}
