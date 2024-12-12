using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffDebuffClass : MonoBehaviour
{

    public GameObject targetWithDebuff;
    public ScriptableBuffDebuff scriptableBuffDebuff;

    public int turnsAvailable;
    public int tempVariable = 0;

    public bool infiniteDuration = false;

    public void CreateBuffOnTarget(ScriptableBuffDebuff scriptableBuffDebuff, GameObject target,int buffDebuffValue, int turnsAvailabeP)
    {
        //create the debuff
        //buffDebuffClass = buffdebuffPrefabLocal.GetComponent<BuffDebuffClass>();
        this.scriptableBuffDebuff = scriptableBuffDebuff;
        infiniteDuration = scriptableBuffDebuff.infiniteDuration;


        //add the target with the debuff
        targetWithDebuff = target;

        //add icon
        this.gameObject.transform.Find("Icon").GetComponent<Image>().sprite = scriptableBuffDebuff.icon;


    }


    public void ModifyTurnsAvailable(int turnsAvailable)
    {
        if (infiniteDuration)
        {
            return;
        }

        this.turnsAvailable += turnsAvailable;
        UpdateBuffDebuffUI();
    }

    public void ModifyValueAvailable(int valueAvailable)
    {
        if (!infiniteDuration)
        {
            return;
        }

        tempVariable += valueAvailable;
        UpdateBuffDebuffUI_InfiniteDuration();
    }

    public void UpdateBuffDebuffUI()
    {
        gameObject.transform.Find("TEXT").GetComponent<TMP_Text>().text = turnsAvailable.ToString();
    }

    public void UpdateBuffDebuffUI_InfiniteDuration()
    {
        gameObject.transform.Find("TEXT").GetComponent<TMP_Text>().text = tempVariable.ToString();
    }

    public void CheckIfExpired()
    {

        if (infiniteDuration)
        {
            return;
        }

        if (turnsAvailable <= 0)
        {
            scriptableBuffDebuff.OnExpireBuffDebuff(targetWithDebuff);
            Destroy(this.gameObject);
        }
    }

}
