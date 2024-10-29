using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticData : MonoBehaviour
{

    public static List<CardScript> staticMainDeck = new List<CardScript>();

    public static ScriptableEntity staticCharacter;
    public static ScriptableCompanion staticScriptableCompanion;

    public static GameObject staticDungeonParent;
    public static bool staticDungeonParentGenerated = false;

    public static List<ClassItem> inventory = new List<ClassItem>();
    public static List<ClassItem> loot = new List<ClassItem>();
}
