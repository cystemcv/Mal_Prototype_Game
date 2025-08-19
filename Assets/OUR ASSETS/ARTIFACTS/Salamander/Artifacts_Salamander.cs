using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Artifacts_Salamander", menuName = "Item/Artifacts/Artifacts_Salamander")]
public class Artifacts_Salamander : ScriptableItem
{
    [Title("UNIQUE ITEM ABILITY")]
    public int multiHits = 1;
    public int dmgDone = 1;
    public float waitMultiHit = 0.3f;

    public int cardActivationCost = 2;

    public GameObject SpawnEffectPrefab;
    public float SpawnEffectPrefabTimer;

    

    public override void Activate(ClassItemData classItem, CardScriptData cardScriptData, GameObject target)
    {

        //check if mana of card is 3
        if (cardScriptData.scriptableCard.primaryManaCost != cardActivationCost)
        {
            return;
        }

        //then loop
        if (multiHits <= 0)
        {
            multiHits = 1;
        }

        ItemManager.Instance.AddItemOnActivateOrder(this, "Deal " + dmgDone + " to all enemies", false);

        MonoBehaviour runner = CombatCardHandler.Instance; // Ensure this is a valid MonoBehaviour in your scene
                                                           //hit at least one time if its 0

        // Start the coroutine for each hit
        runner.StartCoroutine(ExecuteMultiHits());
    }


    // Coroutine to handle multiple hits with delay
    private IEnumerator ExecuteMultiHits()
    {

        MonoBehaviour runner = CombatCardHandler.Instance;

        for (int i = 0; i < multiHits; i++)
        {
            // Use ability on each hit

            List<GameObject> targetsFound = new List<GameObject>();

            //get all enemies
            targetsFound = SystemManager.Instance.FindGameObjectsWithTags(SystemManager.Instance.GetEnemyTagsList());


            //then loop
            foreach (GameObject targetFound in targetsFound)
            {
                if (targetFound == null)
                {
                    continue;
                }


                if (targetFound.GetComponent<EntityClass>().entityMode != SystemManager.EntityMode.DEAD)
                {

                    //spawn prefab
                    SystemManager.Instance.SpawnEffectPrefab(SpawnEffectPrefab,targetFound, SpawnEffectPrefabTimer);


                    yield return runner.StartCoroutine(Combat.Instance.AdjustTargetHealth(null, targetFound, dmgDone, false, SystemManager.AdjustNumberModes.ATTACK));
                }
            }

            // Wait between hits
            yield return new WaitForSeconds(waitMultiHit);
        }
    }




}
