using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Artifacts_CounterGloves", menuName = "Item/Artifacts/Artifacts_CounterGloves")]
public class Artifacts_CounterGloves : ScriptableItem
{
    [Title("UNIQUE ITEM ABILITY")]
    public int dmgDone = 1;

    public override void Activate(ClassItem classItem)
    {
        GameObject character = GameObject.FindGameObjectWithTag("Player");

        character.GetComponent<EntityClass>().counterDamage += dmgDone;
    }




}
