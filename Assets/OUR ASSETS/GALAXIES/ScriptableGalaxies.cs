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

    [Title("GENERATOR SETTINGS")]
    public float bossRoomChance = 0;
    public float emptyRoomChance = 0;
    public float rewardRoomChance = 0;
    public float eventRoomChance = 0;
    public float restRoomChance = 0;

    public int maxRoomLevels = 5; // Maximum number of room levels
    public int allowedPercentage = 30;
    public int distanceBetweenRooms = 2; // Distance you want to put between rooms

}
