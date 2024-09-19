using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using static ScriptableCard;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    [Header("ENEMIES IN BATTLE")]
    public List<ScriptableEntity> scriptableEnemyList;


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

    public IEnumerator EnemyAiAct()
    {

        //get how many enemies will act
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            AIBrain aIBrain = enemy.GetComponent<AIBrain>();

            //if no ai brain the get continue to the next or deasd
            if (aIBrain == null || enemy.GetComponent<EntityClass>().entityMode == SystemManager.EntityMode.DEAD)
            {
                continue;
            }

            ScriptableCard scriptableCard = aIBrain.cardScriptList[aIBrain.aiLogicStep];
            float totalAbilitiesWaitTime = 0;

            //go throught every ability and calculate the waitTime
            foreach (CardAbilityClass cardAbilityClass in scriptableCard.cardAbilityClass)
            {
                totalAbilitiesWaitTime += cardAbilityClass.waitForAbility;
            }

            //execute ai
            aIBrain.ExecuteCommand();

            //loop between them and execute the command
            yield return new WaitForSeconds(totalAbilitiesWaitTime);
        }


    }

}
