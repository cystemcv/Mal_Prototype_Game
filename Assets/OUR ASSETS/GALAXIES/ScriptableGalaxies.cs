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

    public List<ScriptableEvent> scriptableRestList;

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

    [Title("SHOP ECONOMY CARDS")]
    //shop economics
    public int minCommonCardCost = 0;
    public int maxCommonCardCost = 0;
    public int minRareCardCost = 0;
    public int maxRareCardCost = 0;
    public int minEpicCardCost = 0;
    public int maxEpicCardCost = 0;
    public int minLegendaryCardCost = 0;
    public int maxLegendaryCardCost = 0;

    [Title("SHOP ECONOMY ARTIFACTS")]
    public int minCommonArtifactCost = 0;
    public int maxCommonArtifactCost = 0;
    public int minRareArtifactCost = 0;
    public int maxRareArtifactCost = 0;
    public int minEpicArtifactCost = 0;
    public int maxEpicArtifactCost = 0;
    public int minLegendaryArtifactCost = 0;
    public int maxLegendaryArtifactCost = 0;

    [Title("SHOP ECONOMY ITEMS")]
    public int minCommonItemCost = 0;
    public int maxCommonItemCost = 0;
    public int minRareItemCost = 0;
    public int maxRareItemCost = 0;
    public int minEpicItemCost = 0;
    public int maxEpicItemCost = 0;
    public int minLegendaryItemCost = 0;
    public int maxLegendaryItemCost = 0;
}
