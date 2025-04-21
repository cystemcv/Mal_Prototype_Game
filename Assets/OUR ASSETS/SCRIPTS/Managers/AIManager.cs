using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using static ScriptableCard;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance;




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

    public IEnumerator AiAct(List<string> tags)
    {


        //get how many enemies will act
        List<GameObject> entities = SystemManager.Instance.FindGameObjectsWithTags(tags);

        foreach (GameObject entity in entities)
        {

            if(entity == null)
            {
                continue;
            }

            AIBrain aIBrain = entity.GetComponent<AIBrain>();

            //if no ai brain the get continue to the next or deasd
            if (aIBrain == null || entity.GetComponent<EntityClass>().entityMode == SystemManager.EntityMode.DEAD)
            {
                continue;
            }

            ScriptableCard scriptableCard = aIBrain.cardScriptList[aIBrain.aiLogicStep];
            float totalAbilitiesWaitTime = 0;

            ////go throught every ability and calculate the waitTime
            //foreach (CardAbilityClass cardAbilityClass in scriptableCard.cardAbilityClass)
            //{
            //    totalAbilitiesWaitTime += cardAbilityClass.waitForAbility;
            //}

            //execute ai
            aIBrain.ExecuteCommand();

            //loop between them and execute the command
            yield return new WaitForSeconds(scriptableCard.abilityEffectLifetime);

            if (aIBrain.intend != null)
            {
                aIBrain.intend.SetActive(false);
            }
        }


    }




    public IEnumerator GenerateIntends(List<string> tags)
    {
        List<GameObject> entityList = SystemManager.Instance.FindGameObjectsWithTags(tags);


        foreach (GameObject entity in entityList)
        {

            AIBrain aIBrain = entity.GetComponent<AIBrain>();

            //if no ai brain the get continue to the next or deasd
            if (aIBrain == null || entity.GetComponent<EntityClass>().entityMode == SystemManager.EntityMode.DEAD)
            {
                continue;
            }

            //generate new intends
            aIBrain.GenerateIntend();
        }

        yield return null;

    }

    public GameObject GetRandomTarget(GameObject entityUsedCard)
    {

        List<GameObject> targetsFound = new List<GameObject>();

        //get all targets
        if (SystemManager.Instance.GetPlayerTagsList().Contains(entityUsedCard.tag))
        {
            targetsFound = SystemManager.Instance.FindGameObjectsWithTags(SystemManager.Instance.GetEnemyTagsList());
        }
        else
        {
            targetsFound = SystemManager.Instance.FindGameObjectsWithTags(SystemManager.Instance.GetPlayerTagsList());
        }

        int randomTarget = UnityEngine.Random.Range(0, targetsFound.Count);

        //then loop
        GameObject realTarget = targetsFound[randomTarget];
        return realTarget;

    }

    public GameObject GetRandomAlly(GameObject entityUsedCard)
    {

        List<GameObject> targetsFound = new List<GameObject>();

        //get all targets
        if (SystemManager.Instance.GetPlayerTagsList().Contains(entityUsedCard.tag))
        {
            targetsFound = SystemManager.Instance.FindGameObjectsWithTags(SystemManager.Instance.GetPlayerTagsList());
        }
        else
        {
            targetsFound = SystemManager.Instance.FindGameObjectsWithTags(SystemManager.Instance.GetEnemyTagsList());
        }

        int randomTarget = UnityEngine.Random.Range(0, targetsFound.Count);

        //then loop
        GameObject realTarget = targetsFound[randomTarget];
        return realTarget;

    }

    public List<GameObject> GetAllTargets(GameObject entityUsedCard)
    {

        List<GameObject> targetsFound = new List<GameObject>();

        //get all targets
        if (SystemManager.Instance.GetPlayerTagsList().Contains(entityUsedCard.tag))
        {
            targetsFound = SystemManager.Instance.FindGameObjectsWithTags(SystemManager.Instance.GetEnemyTagsList());
        }
        else
        {
            targetsFound = SystemManager.Instance.FindGameObjectsWithTags(SystemManager.Instance.GetPlayerTagsList());
        }

        return targetsFound;

    }

    public List<GameObject> GetAllAllies(GameObject entityUsedCard)
    {

        List<GameObject> targetsFound = new List<GameObject>();

        //get all targets
        if (SystemManager.Instance.GetPlayerTagsList().Contains(entityUsedCard.tag))
        {
            targetsFound = SystemManager.Instance.FindGameObjectsWithTags(SystemManager.Instance.GetPlayerTagsList());
        }
        else
        {
            targetsFound = SystemManager.Instance.FindGameObjectsWithTags(SystemManager.Instance.GetEnemyTagsList());
        }

        return targetsFound;

    }

}
