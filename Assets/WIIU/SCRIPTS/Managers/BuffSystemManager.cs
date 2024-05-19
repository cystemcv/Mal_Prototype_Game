using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class BuffSystemManager : MonoBehaviour
{
    public static BuffSystemManager Instance;

    //for buff/debuff system
    //public enum buffDebuffName { NONE, ATTACKUP, POISON };
    public GameObject buffdebuffPrefab;

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


    public void AddDebuffToEnemyTarget(ScriptableCardAbility scriptableCardAbility, GameObject target, int turnForDebuff)
    {
        GameObject gridSystem = target.transform.Find("BuffDebuffList").GetChild(0).gameObject;
        BuffDebuffClass[] gridSystemItems = gridSystem.GetComponentsInChildren<BuffDebuffClass>();
        List<BuffDebuffClass> gridSystemItemList = gridSystemItems.ToList();


        int index = gridSystemItemList.FindIndex(item => item.scriptableCardAbility.scriptableBuffDebuff.nameID == scriptableCardAbility.scriptableBuffDebuff.nameID);

        //if already exist just increase the counter
        if (index != -1)
        {
            gridSystemItemList[index].turnsAvailable += turnForDebuff;
            gridSystem.transform.GetChild(index).transform.Find("TEXT").GetComponent<TMP_Text>().text = gridSystemItemList[index].turnsAvailable.ToString();
        }
        else {

            //instantiate the gameobject prefab
        
            GameObject buffdebuffPrefabLocal = Instantiate(buffdebuffPrefab, gridSystem.transform.position, Quaternion.identity);

            buffdebuffPrefabLocal.transform.SetParent(gridSystem.transform);

            //create the debuff
            BuffDebuffClass debuffClass = buffdebuffPrefabLocal.GetComponent<BuffDebuffClass>();
            debuffClass.scriptableCardAbility = scriptableCardAbility;
            debuffClass.turnsAvailable = turnForDebuff;

            //ui
            buffdebuffPrefabLocal.GetComponent<Image>().sprite = debuffClass.scriptableCardAbility.scriptableBuffDebuff.icon;
            buffdebuffPrefabLocal.transform.Find("TEXT").GetComponent<TMP_Text>().text = debuffClass.turnsAvailable.ToString();

            EnemyClass enemyClass = target.GetComponent<EnemyClass>();
            enemyClass.listBuffDebuffClass.Add(buffdebuffPrefabLocal);
        }

    }
    
    public void EnemyTurnStartBD()
    {

        //get all characters
        GameObject[] characters = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject character in characters)
        {
            CharacterClass characterClass = character.GetComponent<CharacterClass>();
           
            foreach (GameObject buffdebufPRefab in characterClass.listBuffDebuffClass)
            {
                BuffDebuffClass buffDebuffClass = buffdebufPRefab.GetComponent<BuffDebuffClass>();
                //activate buffs/debuffs
                bool activated = buffDebuffClass.scriptableCardAbility.OnEnemyTurnStart(character);

                if (activated)
                {
                    //decrease by 1
                    buffDebuffClass.turnsAvailable -= 1;

                    //update ui
                    buffdebufPRefab.transform.Find("TEXT").GetComponent<TMP_Text>().text = buffDebuffClass.turnsAvailable.ToString();
                }

                if (buffDebuffClass.turnsAvailable <= 0)
                {
                    Destroy(buffdebufPRefab);
                }
            }
        }

        //get all enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            EnemyClass enemyClass = enemy.GetComponent<EnemyClass>();

            foreach (GameObject buffdebufPRefab in enemyClass.listBuffDebuffClass)
            {
                BuffDebuffClass buffDebuffClass = buffdebufPRefab.GetComponent<BuffDebuffClass>();
                //activate buffs/debuffs
                bool activated = buffDebuffClass.scriptableCardAbility.OnEnemyTurnStart(enemy);

                if (activated)
                {
                    //decrease by 1
                    buffDebuffClass.turnsAvailable -= 1;

                    //update ui
                    buffdebufPRefab.transform.Find("TEXT").GetComponent<TMP_Text>().text = buffDebuffClass.turnsAvailable.ToString();
                }

                if (buffDebuffClass.turnsAvailable <= 0)
                {
                    Destroy(buffdebufPRefab);
                }
            }
        }
    }

}
