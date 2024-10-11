using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableBattles", menuName = "ScriptableBattles")]
public class ScriptableBattles : ScriptableObject
{

    public int worldLevel = 1;
    public string battleName = "";
    public List<ScriptableEntity> scriptableEntities;
   public GameObject battlegroundPrefab;

}
