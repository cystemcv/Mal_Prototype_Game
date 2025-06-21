using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptablePlanets;

[CreateAssetMenu(fileName = "ScriptableBattleGrounds", menuName = "scriptableGalaxies/ScriptableBattleGrounds")]
public class ScriptableBattleGrounds : ScriptableObject
{

    public GameObject battleGround;
    public SystemManager.BattleGroundType battleGroundType = SystemManager.BattleGroundType.WATER;
    public bool isSpaceShip = false;

    [FoldoutGroup("CONDITIONS")]
    public List<ItemClassPlanet> itemClassPlanet;

}
