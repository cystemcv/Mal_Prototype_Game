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


    //public void AddDebuffToEnemyTarget(ScriptableCardAbility scriptableCardAbility, GameObject target, int turnsAvailable)
    //{
    //    GameObject gridSystem = target.transform.Find("gameobjectUI").Find("BuffDebuffList").GetChild(0).gameObject;
    //    BuffDebuffClass[] gridSystemItems = gridSystem.GetComponentsInChildren<BuffDebuffClass>();
    //    List<BuffDebuffClass> gridSystemItemList = gridSystemItems.ToList();


    //    int index = gridSystemItemList.FindIndex(item => item.scriptableCardAbility.scriptableBuffDebuff.nameID == scriptableCardAbility.scriptableBuffDebuff.nameID);

    //    //if already exist just increase the counter
    //    if (index != -1)
    //    {
    //        gridSystemItemList[index].turnsAvailable += turnsAvailable;
    //        gridSystem.transform.GetChild(index).transform.Find("TEXT").GetComponent<TMP_Text>().text = gridSystemItemList[index].turnsAvailable.ToString();
    //    }
    //    else {

    //        //instantiate the gameobject prefab

    //        GameObject buffdebuffPrefabLocal = Instantiate(buffdebuffPrefab, gridSystem.transform.position, Quaternion.identity);

    //        buffdebuffPrefabLocal.transform.SetParent(gridSystem.transform);

    //        //create the debuff
    //        BuffDebuffClass debuffClass = buffdebuffPrefabLocal.GetComponent<BuffDebuffClass>();
    //        debuffClass.scriptableCardAbility = scriptableCardAbility;
    //        debuffClass.turnsAvailable = turnsAvailable;

    //        //ui
    //        buffdebuffPrefabLocal.GetComponent<Image>().sprite = debuffClass.scriptableCardAbility.scriptableBuffDebuff.icon;
    //        buffdebuffPrefabLocal.transform.Find("TEXT").GetComponent<TMP_Text>().text = debuffClass.turnsAvailable.ToString();

    //        EnemyClass enemyClass = target.GetComponent<EnemyClass>();
    //        enemyClass.listBuffDebuffClass.Add(buffdebuffPrefabLocal);
    //    }

    //}

    public EntityClass AddBuffDebuff(GameObject target, ScriptableBuffDebuff scriptableBuffDebuff, int buffDebuffValue, int turnsValue)
    {

        GameObject gridSystem = target.transform.Find("gameobjectUI").Find("BuffDebuffList").GetChild(0).GetChild(0).gameObject;
        BuffDebuffClass buffDebuffClass = GetBuffDebuffClassFromTarget(target, scriptableBuffDebuff.nameID);

        //if there is no buff then create it to the target
        if (buffDebuffClass == null)
        {
            //if not then add this buff
            //create the gameobject and add it
            GameObject buffdebuffPrefabLocal = Instantiate(buffdebuffPrefab, gridSystem.transform.position, Quaternion.identity);
            buffdebuffPrefabLocal.transform.SetParent(gridSystem.transform);
            buffdebuffPrefabLocal.transform.localScale = new Vector3(1, 1, 1);
            buffDebuffClass = buffdebuffPrefabLocal.GetComponent<BuffDebuffClass>();

            buffDebuffClass.CreateBuffOnTarget(scriptableBuffDebuff, target, buffDebuffValue, turnsValue);

            //first time it gets created it should not increase the stacks just the turns
            if (!buffDebuffClass.infiniteDuration)
            {
                buffDebuffClass.tempVariable = buffDebuffValue;
            }
        }

        //increase the value of the buff
        if (!buffDebuffClass.infiniteDuration)
        {
            buffDebuffClass.ModifyTurnsAvailable(turnsValue);
        }
        else if (buffDebuffClass.infiniteDuration)
        {
            buffDebuffClass.ModifyValueAvailable(buffDebuffValue);
        }

        //activate the buff
        buffDebuffClass.scriptableBuffDebuff.OnApplyBuff(target, buffDebuffValue, turnsValue);

         //get the buffed class
         EntityClass entityClass = target.GetComponent<EntityClass>();

        return entityClass;

    }

    //public void AddBuffDebuffToTarget(ScriptableCardAbility scriptableCardAbility, GameObject target, int variableValue)
    //{
    //    GameObject gridSystem = target.transform.Find("gameobjectUI").Find("BuffDebuffList").GetChild(0).gameObject;
    //    BuffDebuffClass buffDebuffClass = GetBuffDebuffClassFromTarget(target, scriptableCardAbility.scriptableBuffDebuff.nameID);

    //    //there is already a buff with this id, then add more turns into it
    //    if (buffDebuffClass != null)
    //    {
    //        if (!buffDebuffClass.infiniteDuration)
    //        {
    //            buffDebuffClass.ModifyTurnsAvailable(variableValue);
    //        }
    //        else if (buffDebuffClass.infiniteDuration)
    //        {
    //            buffDebuffClass.ModifyValueAvailable(variableValue);
    //        }
       
    //    }
    //    else
    //    {
    //        //if not then add this buff
    //        //create the gameobject and add it
    //        GameObject buffdebuffPrefabLocal = Instantiate(buffdebuffPrefab, gridSystem.transform.position, Quaternion.identity);
    //        buffdebuffPrefabLocal.transform.SetParent(gridSystem.transform);
    //        buffDebuffClass = buffdebuffPrefabLocal.GetComponent<BuffDebuffClass>();

    //        buffDebuffClass.CreateBuffOnTarget(scriptableCardAbility,target, variableValue);

    //        if (!buffDebuffClass.infiniteDuration)
    //        {
    //            buffDebuffClass.ModifyTurnsAvailable(variableValue);
    //        }
    //        else if (buffDebuffClass.infiniteDuration)
    //        {
    //            buffDebuffClass.ModifyValueAvailable(variableValue);
    //        }


    //    }



    //}

    public BuffDebuffClass GetBuffDebuffClassFromTarget(GameObject target, string buffDebuffNameID)
    {
        BuffDebuffClass buffDebuffClass = null;

        GameObject gridSystem = target.transform.Find("gameobjectUI").Find("BuffDebuffList").GetChild(0).GetChild(0).gameObject;
        BuffDebuffClass[] gridSystemItems = gridSystem.GetComponentsInChildren<BuffDebuffClass>();
        List<BuffDebuffClass> gridSystemItemList = gridSystemItems.ToList();


        int index = gridSystemItemList.FindIndex(item => item.scriptableBuffDebuff.nameID == buffDebuffNameID);

        if (index != -1)
        {
            buffDebuffClass = gridSystemItemList[index];
        }

        return buffDebuffClass;
    }

    public void DecreaseValueTargetBuffDebuff(GameObject target, string buffDebuffNameID, int valueToDecrease)
    {

        BuffDebuffClass buffDebuffClass = GetBuffDebuffClassFromTarget(target, buffDebuffNameID);

        buffDebuffClass.ModifyValueAvailable(valueToDecrease);

        //then destroy
        if (buffDebuffClass.tempVariable <= 0)
        {
            Destroy(buffDebuffClass.gameObject);
        }

    }

    public List<BuffDebuffClass> GetAllBuffDebuffFromTarget(GameObject target)
    {

        GameObject gridSystem = target.transform.Find("gameobjectUI").Find("BuffDebuffList").GetChild(0).GetChild(0).gameObject;

        BuffDebuffClass[] gridSystemItems = gridSystem.GetComponentsInChildren<BuffDebuffClass>();

        List<BuffDebuffClass> gridSystemItemList = gridSystemItems.ToList();

        return gridSystemItemList;
    }

    public IEnumerator ActivateAllBuffsDebuffs()
    {

        //get all characters
        GameObject[] characters = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject character in characters)
        {


            List<BuffDebuffClass> buffDebuffClassList = GetAllBuffDebuffFromTarget(character);

            foreach (BuffDebuffClass buffdebufPRefab in buffDebuffClassList)
            {
                BuffDebuffClass buffDebuffClass = buffdebufPRefab;

                bool activated = false;
                //activate buffs/debuffs
                if (SystemManager.Instance.combatTurn == SystemManager.CombatTurns.playerStartTurn)
                {
              
                    activated = buffDebuffClass.scriptableBuffDebuff.OnCharacterTurnStart(character);
                 
                    //if activated then we decrease the turns
                    if (activated)
                    {
                        buffDebuffClass.ModifyTurnsAvailable(-1);
                    }
                }
                else if (SystemManager.Instance.combatTurn == SystemManager.CombatTurns.enemyStartTurn)
                {
                    activated = buffDebuffClass.scriptableBuffDebuff.OnEnemyTurnStart(character);

                    //if activated then we decrease the turns
                    if (activated)
                    {
                        buffDebuffClass.ModifyTurnsAvailable(-1);
                    }
                }
                else if (SystemManager.Instance.combatTurn == SystemManager.CombatTurns.playerEndTurn)
                {
                    activated = buffDebuffClass.scriptableBuffDebuff.OnCharacterTurnEnd(character);

                    //if activated then we decrease the turns
                    if (activated)
                    {
                        buffDebuffClass.ModifyTurnsAvailable(-1);
                    }
                }
                else if (SystemManager.Instance.combatTurn == SystemManager.CombatTurns.enemyEndTurn)
                {
                    activated = buffDebuffClass.scriptableBuffDebuff.OnEnemyTurnEnd(character);

                    //if activated then we decrease the turns
                    if (activated)
                    {
                        buffDebuffClass.ModifyTurnsAvailable(-1);
                    }
                }


  

                //check if destroyed
                buffDebuffClass.CheckIfExpired();

            }

            yield return null;
        }

        //get all enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {


            List<BuffDebuffClass> buffDebuffClassList = GetAllBuffDebuffFromTarget(enemy);

            foreach (BuffDebuffClass buffdebufPRefab in buffDebuffClassList)
            {
                BuffDebuffClass buffDebuffClass = buffdebufPRefab;


                bool activated = false;
                //activate buffs/debuffs
                if (SystemManager.Instance.combatTurn == SystemManager.CombatTurns.playerStartTurn)
                {
                    activated = buffDebuffClass.scriptableBuffDebuff.OnCharacterTurnStart(enemy);

                    //if activated then we decrease the turns
                    if (activated)
                    {
                        buffDebuffClass.ModifyTurnsAvailable(-1);
                    }
                }
                else if (SystemManager.Instance.combatTurn == SystemManager.CombatTurns.enemyStartTurn)
                {
                    activated = buffDebuffClass.scriptableBuffDebuff.OnEnemyTurnStart(enemy);

                    //if activated then we decrease the turns
                    if (activated)
                    {
                        buffDebuffClass.ModifyTurnsAvailable(-1);
                    }
                }
                else if (SystemManager.Instance.combatTurn == SystemManager.CombatTurns.playerEndTurn)
                {
                    activated = buffDebuffClass.scriptableBuffDebuff.OnCharacterTurnEnd(enemy);

                    //if activated then we decrease the turns
                    if (activated)
                    {
                        buffDebuffClass.ModifyTurnsAvailable(-1);
                    }
                }
                else if (SystemManager.Instance.combatTurn == SystemManager.CombatTurns.enemyEndTurn)
                {
                    activated = buffDebuffClass.scriptableBuffDebuff.OnEnemyTurnEnd(enemy);

                    //if activated then we decrease the turns
                    if (activated)
                    {
                        buffDebuffClass.ModifyTurnsAvailable(-1);
                    }
                }


                //if activated then we decrease the turns
                if (activated)
                {
                    buffDebuffClass.ModifyTurnsAvailable(-1);
                }

                //check if destroyed
                buffDebuffClass.CheckIfExpired();
            }
        }
    }

}
