using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    public ScriptableBattles scriptableBattle;

    public List<ScriptableBattles> scriptableBattlesList_World1;
    public List<ScriptableBattles> scriptableBattlesList_World2;
    public List<ScriptableBattles> scriptableBattlesList_World3;
    public List<ScriptableBattles> scriptableBattlesList_World4;
    public List<ScriptableBattles> scriptableBattlesList_World5;




    public ScriptableBattles GetRandomScriptableBattle(int world)
    {
        ScriptableBattles scriptableBattle;

        if (world == 1)
        {

            int randomIndex = Random.Range(0,scriptableBattlesList_World1.Count);
            scriptableBattle = scriptableBattlesList_World1[randomIndex];

        }
        else if (world == 2)
        {

            int randomIndex = Random.Range(0, scriptableBattlesList_World2.Count);
            scriptableBattle = scriptableBattlesList_World2[randomIndex];

        }
        else if (world == 3)
        {

            int randomIndex = Random.Range(0, scriptableBattlesList_World3.Count);
            scriptableBattle = scriptableBattlesList_World3[randomIndex];

        }
        else if (world == 4)
        {

            int randomIndex = Random.Range(0, scriptableBattlesList_World4.Count);
            scriptableBattle = scriptableBattlesList_World4[randomIndex];

        }
        else if (world == 5)
        {

            int randomIndex = Random.Range(0, scriptableBattlesList_World5.Count);
            scriptableBattle = scriptableBattlesList_World5[randomIndex];

        }
        else
        {
            int randomIndex = Random.Range(0, scriptableBattlesList_World5.Count);
            scriptableBattle = scriptableBattlesList_World5[randomIndex];
        }

        return scriptableBattle;
    }


    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

}
