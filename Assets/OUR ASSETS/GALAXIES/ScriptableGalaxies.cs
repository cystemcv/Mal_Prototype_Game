using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableBattleGrounds", menuName = "scriptableGalaxies/scriptableGalaxies")]
public class ScriptableGalaxies : ScriptableObject
{
    [Title("GALAXY")]
    public string galaxyName = "";

    [Title("GALAXY PLANETS")]
    public List<ScriptablePlanets> scriptablePlanets;
    public List<ScriptablePlanets> scriptableStartingPlanets;

    public List<ScriptableEvent> scriptableEventList;
    public List<ScriptableEvent> scriptableEventRewardsList;

    public List<ScriptableEvent> scriptableShopList;

    [Title("GENERATOR SETTINGS")]
    public int minCombatRoom = 10;
    public int maxCombatRoom = 10;
    
    public int minEliteCombatRoom = 0;
    public int maxEliteCombatRoom = 2;

    public int minBossRoom = 1;
    public int maxBossRoom = 2;

    public int minRewardRoom = 1;
    public int maxRewardRoom = 3;

    public int minEventRoom = 6;
    public int maxEventRoom = 10;

    public int minRestRoom = 1;
    public int maxRestRoom = 3;

    public int minHiddenRoom = 0;
    public int maxHiddenRoom = 2;

    public int minShopRoom = 0;
    public int maxShopRoom = 2;

    public int startingRooms = 6;

    //public int maxRoomLevels = 5; // Maximum number of room levels
    public int allowedPercentage = 30;
    public int allowedPercentageConnectBoss = 30;
    public int distanceBetweenRooms = 2; // Distance you want to put between rooms

}
