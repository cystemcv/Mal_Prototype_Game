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

    public List<ScriptableEntity> scriptableEntities = new List<ScriptableEntity>();



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

            ScriptableCard scriptableCard = aIBrain.scriptableCardToUse;
            float totalAbilitiesWaitTime = 0;

            ////go throught every ability and calculate the waitTime
            //foreach (CardAbilityClass cardAbilityClass in scriptableCard.cardAbilityClass)
            //{
            //    totalAbilitiesWaitTime += cardAbilityClass.waitForAbility;
            //}

            //execute ai
            aIBrain.ExecuteCommand();

            //loop between them and execute the command
            if (scriptableCard != null)
            {
                yield return new WaitForSeconds(scriptableCard.abilityEffectLifetime);
            }
  

            if (aIBrain.intend != null)
            {
                aIBrain.intend.SetActive(false);
            }
        }


    }


    //public IEnumerator AiActInitialize(List<string> tags)
    //{


    //    //get how many enemies will act
    //    List<GameObject> entities = SystemManager.Instance.FindGameObjectsWithTags(tags);

    //    foreach (GameObject entity in entities)
    //    {

    //        if (entity == null)
    //        {
    //            continue;
    //        }

    //        AIBrain aIBrain = entity.GetComponent<AIBrain>();

    //        //if no ai brain the get continue to the next or deasd
    //        if (aIBrain == null || entity.GetComponent<EntityClass>().entityMode == SystemManager.EntityMode.DEAD)
    //        {
    //            continue;
    //        }

    //        if (entity.GetComponent<EntityClass>().scriptableEntity.aICommandsInitialize == null)
    //        {
    //            continue;
    //        }

    //        if (entity.GetComponent<EntityClass>().scriptableEntity.aICommandsInitialize.aiScriptableCards.Count == 0)
    //        {
    //            continue;
    //        }

    //        //ScriptableCard scriptableCard = entity.GetComponent<EntityClass>().scriptableEntity.aICommandsInitialize.aiScriptableCards[0];
    //        float totalAbilitiesWaitTime = 0;

    //        aIBrain.scriptableEntity = entity.GetComponent<EntityClass>().scriptableEntity;

    //        //execute ai
    //        aIBrain.ExecuteInitializeCommand();

    //        //loop between them and execute the command
    //        //yield return new WaitForSeconds(scriptableCard.abilityEffectLifetime);

    //        if (aIBrain.intend != null)
    //        {
    //            aIBrain.intend.SetActive(false);
    //        }
    //    }


    //}



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

            if (aIBrain.justSpawned)
            {
                aIBrain.justSpawned = false;
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

    public GameObject GetRandomTargetBasedOnTags(List<string> tags)
    {
        List<GameObject> gameObjects = SystemManager.Instance.FindGameObjectsWithTags(tags);

        int randomTarget = UnityEngine.Random.Range(0, gameObjects.Count);

        //then loop
        GameObject realTarget = gameObjects[randomTarget];
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

    public int ReturnAICardLevel(int originalDmg,GameObject entityUsedCard)
    {
        int finalDmg = originalDmg;

        if(entityUsedCard.GetComponent<AIBrain>() != null){
            AIBrain aIBrain = entityUsedCard.GetComponent<AIBrain>();

            if (aIBrain.aICommandCardToUse.modifiedCardValueMin != 0)
            {
                finalDmg = Random.Range(aIBrain.aICommandCardToUse.modifiedCardValueMin, aIBrain.aICommandCardToUse.modifiedCardValueMax);
            }
           
        }

        return finalDmg;
    }

}
