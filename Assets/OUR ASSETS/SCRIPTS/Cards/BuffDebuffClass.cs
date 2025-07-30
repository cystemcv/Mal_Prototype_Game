using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffDebuffClass : MonoBehaviour
{

    public GameObject targetWithDebuff;
    public ScriptableBuffDebuff scriptableBuffDebuff;

    public int tempValue = 0;

    public bool infiniteDuration = false;

    public void CreateBuffOnTarget(ScriptableBuffDebuff scriptableBuffDebuff, GameObject target,int value)
    {
        //create the debuff
        //buffDebuffClass = buffdebuffPrefabLocal.GetComponent<BuffDebuffClass>();
        this.scriptableBuffDebuff = scriptableBuffDebuff;
        infiniteDuration = scriptableBuffDebuff.infiniteDuration;


        //add the target with the debuff
        targetWithDebuff = target;

        //add icon
        this.gameObject.transform.Find("Icon").GetComponent<Image>().sprite = scriptableBuffDebuff.icon;

        if (scriptableBuffDebuff.infiniteDuration)
        {
            gameObject.transform.Find("TEXT").GetComponent<TMP_Text>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorSkyBlue);
        }
        else
        {
            gameObject.transform.Find("TEXT").GetComponent<TMP_Text>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorDiscordRed);
        }

        //add tooltip
        this.gameObject.GetComponent<TooltipContent>().description = BuffSystemManager.Instance.GetBuffDebuffColor(scriptableBuffDebuff) + " : " + scriptableBuffDebuff.description;

    }


    public void ModifyTurnsAvailable(int turnsAvailable)
    {
        if (infiniteDuration)
        {
            return;
        }

        this.tempValue += turnsAvailable;
        UpdateBuffDebuffUI();
    }

    public void ModifyValueAvailable(int valueAvailable)
    {
        if (!infiniteDuration)
        {
            return;
        }

        tempValue += valueAvailable;
        UpdateBuffDebuffUI_InfiniteDuration();
    }

    public void UpdateBuffDebuffUI()
    {
        gameObject.transform.Find("TEXT").GetComponent<TMP_Text>().text = tempValue.ToString();
    }

    public void UpdateBuffDebuffUI_InfiniteDuration()
    {
        gameObject.transform.Find("TEXT").GetComponent<TMP_Text>().text = tempValue.ToString();
    }

    public void CheckIfExpired()
    {

        if (infiniteDuration)
        {
            return;
        }

        if (tempValue <= 0)
        {
            scriptableBuffDebuff.OnExpireBuffDebuff(targetWithDebuff);
            Destroy(this.gameObject);
        }
    }

}
