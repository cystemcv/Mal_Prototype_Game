using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffDebuffClass : MonoBehaviour
{

    public GameObject targetWithDebuff;
    public ScriptableCardAbility scriptableCardAbility;

    public int turnsAvailable;

    public int tempVariable = 0;

    public void CreateBuffOnTarget(ScriptableCardAbility scriptableCardAbility, GameObject target, int turnsAvailabeP)
    {
        //create the debuff
        //buffDebuffClass = buffdebuffPrefabLocal.GetComponent<BuffDebuffClass>();
        this.scriptableCardAbility = scriptableCardAbility;

        IncreaseTurnsAvailable(turnsAvailabeP);

        //add the target with the debuff
        targetWithDebuff = target;

        //add icon
        this.gameObject.GetComponent<Image>().sprite = scriptableCardAbility.scriptableBuffDebuff.icon;
        if (scriptableCardAbility.scriptableBuffDebuff.isBuff == true)
        {
            this.gameObject.GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorLightBlue);
        }
        else
        {
            this.gameObject.GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorRed);
        }

    }

    public void DecreaseTurnsAvailable(int turnsAvailable)
    {
        this.turnsAvailable -= turnsAvailable;
        UpdateBuffDebuffUI();
    }

    public void IncreaseTurnsAvailable(int turnsAvailable)
    {
        this.turnsAvailable += turnsAvailable;
        UpdateBuffDebuffUI();
    }

    public void UpdateBuffDebuffUI()
    {
        gameObject.transform.Find("TEXT").GetComponent<TMP_Text>().text = turnsAvailable.ToString();
    }

    public void CheckIfExpired()
    {
        if (turnsAvailable <= 0)
        {
            scriptableCardAbility.OnExpireBuffDebuff(targetWithDebuff);
            Destroy(this.gameObject);
        }
    }

}
