using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BuffSystemManager;

[CreateAssetMenu(fileName = "ScriptableBuffDebuff", menuName = "BuffsDebuffs/ScriptableBuffDebuff")]
public class ScriptableBuffDebuff : ScriptableObject
{

    public string nameID;
    public Sprite icon;
    public string description;

}