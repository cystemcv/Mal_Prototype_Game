using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableBattleGrounds", menuName = "scriptableGalaxies/ScriptableBattleGrounds")]
public class ScriptableBattleGrounds : ScriptableObject
{

    public GameObject battleGround;
    public SystemManager.BattleGroundType battleGroundType = SystemManager.BattleGroundType.WATER;

}
