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




    public IEnumerator GenerateIntends(List<string> tags)
    {
        List<GameObject> entityList = entityList = SystemManager.Instance.FindGameObjectsWithTags(tags);


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

}
