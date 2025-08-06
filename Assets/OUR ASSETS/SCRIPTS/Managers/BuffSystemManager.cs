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

    public EntityClass AddBuffDebuff(GameObject target, ScriptableBuffDebuff scriptableBuffDebuff, int value)
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

            buffDebuffClass.CreateBuffOnTarget(scriptableBuffDebuff, target, value);

        }

        //increase the value of the buff
        if (!buffDebuffClass.infiniteDuration)
        {
            buffDebuffClass.ModifyTurnsAvailable(value);
        }
        else if (buffDebuffClass.infiniteDuration)
        {
            buffDebuffClass.ModifyValueAvailable(value);
        }

        //activate the buff
        buffDebuffClass.scriptableBuffDebuff.OnApplyBuff(target, value);

        //get the buffed class
        EntityClass entityClass = target.GetComponent<EntityClass>();

        return entityClass;

    }

    public IEnumerator AddBuffDebuffIE(GameObject target, ScriptableBuffDebuff scriptableBuffDebuff, int value)
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

            buffDebuffClass.CreateBuffOnTarget(scriptableBuffDebuff, target, value);

            //first time it gets created it should not increase the stacks just the turns
            if (!buffDebuffClass.infiniteDuration)
            {
                buffDebuffClass.tempValue = value;
            }
        }

        //increase the value of the buff
        if (!buffDebuffClass.infiniteDuration)
        {
            buffDebuffClass.ModifyTurnsAvailable(value);
        }
        else if (buffDebuffClass.infiniteDuration)
        {
            buffDebuffClass.ModifyValueAvailable(value);
        }

        //activate the buff
        buffDebuffClass.scriptableBuffDebuff.OnApplyBuff(target, value);

        //get the buffed class
        EntityClass entityClass = target.GetComponent<EntityClass>();

        yield return entityClass;

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

        if (buffDebuffClass == null)
        {
            return;
        }

        buffDebuffClass.ModifyValueAvailable(valueToDecrease);

        //then destroy
        if (buffDebuffClass.tempValue <= 0)
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
        List<GameObject> characters = SystemManager.Instance.FindGameObjectsWithTags(SystemManager.Instance.GetPlayerTagsList());

        foreach (GameObject character in characters)
        {


            List<BuffDebuffClass> buffDebuffClassList = GetAllBuffDebuffFromTarget(character);

            foreach (BuffDebuffClass buffDebuffClass in buffDebuffClassList)
            {

                //activate buffs/debuffs
                if (SystemManager.Instance.combatTurn == SystemManager.CombatTurns.playerStartTurn)
                {
                    buffDebuffClass.scriptableBuffDebuff.OnCharacterTurnStart(character);

                    //reduce the turns
                    if (!buffDebuffClass.scriptableBuffDebuff.infiniteDuration && Combat.Instance.turns != 0)
                    {
                        buffDebuffClass.ModifyTurnsAvailable(-1);
                    }

                }
                else if (SystemManager.Instance.combatTurn == SystemManager.CombatTurns.playerEndTurn)
                {
                    buffDebuffClass.scriptableBuffDebuff.OnCharacterTurnEnd(character);
                }


                //check if destroyed
                buffDebuffClass.CheckIfExpired();

            }

            yield return null;
        }

        //get all enemies
        List<GameObject> enemies = SystemManager.Instance.FindGameObjectsWithTags(SystemManager.Instance.GetEnemyTagsList());

        foreach (GameObject enemy in enemies)
        {


            List<BuffDebuffClass> buffDebuffClassList = GetAllBuffDebuffFromTarget(enemy);

            foreach (BuffDebuffClass buffDebuffClass in buffDebuffClassList)
            {
                //activate buffs/debuffs
             if (SystemManager.Instance.combatTurn == SystemManager.CombatTurns.enemyStartTurn)
                {
                    buffDebuffClass.scriptableBuffDebuff.OnEnemyTurnStart(enemy);

                    //reduce the turns
                    if (!buffDebuffClass.scriptableBuffDebuff.infiniteDuration && Combat.Instance.turns != 0)
                    {
                        buffDebuffClass.ModifyTurnsAvailable(-1);
                    }
                }
                else if (SystemManager.Instance.combatTurn == SystemManager.CombatTurns.enemyEndTurn)
                {
                    buffDebuffClass.scriptableBuffDebuff.OnEnemyTurnEnd(enemy);
                }

                //check if destroyed
                buffDebuffClass.CheckIfExpired();
            }
        }
    }


    public IEnumerator ActivateBuffsDebuffs_OnGettingHit(GameObject caster, GameObject target)
    {


        List<BuffDebuffClass> buffDebuffClassList = GetAllBuffDebuffFromTarget(target);

        Debug.Log("In here 1");

        foreach (BuffDebuffClass buffdebufPRefab in buffDebuffClassList)
        {
            Debug.Log("In here loop " + buffdebufPRefab.scriptableBuffDebuff.nameID);
            BuffDebuffClass buffDebuffClass = buffdebufPRefab;

            buffDebuffClass.scriptableBuffDebuff.OnGettingHit(caster, target, 0, 0);
        }

        yield return null;

    }

    public IEnumerator ActivateBuffsDebuffs_OnPlayCard(CardScriptData cardScriptData,GameObject caster, GameObject target)
    {


        List<BuffDebuffClass> buffDebuffClassList = GetAllBuffDebuffFromTarget(caster);

        foreach (BuffDebuffClass buffdebufPRefab in buffDebuffClassList)
        {
            BuffDebuffClass buffDebuffClass = buffdebufPRefab;

            buffDebuffClass.scriptableBuffDebuff.OnPlayCard(cardScriptData, caster, target);
        }

        yield return null;

    }

    public string GetBuffDebuffColor(ScriptableBuffDebuff scriptableBuffDebuff)
    {
        if (scriptableBuffDebuff.infiniteDuration)
        {
            return "<color=#" + SystemManager.Instance.colorSkyBlue + ">" + scriptableBuffDebuff.nameID + "</color>";
        }
        else
        {
            return "<color=#" + SystemManager.Instance.colorDiscordRed + ">" + scriptableBuffDebuff.nameID + "</color>";
        }
       
    }
}
