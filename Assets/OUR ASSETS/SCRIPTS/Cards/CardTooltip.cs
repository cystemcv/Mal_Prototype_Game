using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{


    public void OnPointerEnter(PointerEventData eventData)
    {

        ShowCardToolTip();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    
    }

    public void ShowCardToolTip()
    {

        CardScript cardScript = this.gameObject.GetComponent<CardScript>();
        CardScriptData cardScriptData = new CardScriptData();
        CardListCardEvents cardListCardEvents = this.gameObject.GetComponent<CardListCardEvents>();
        ScriptableCard scriptableCard = null;

        if (cardScript == null)
        {
            return;
        }

        cardScriptData = cardScript.cardScriptData;

        //if (cardScriptData != null)
        //{
            scriptableCard = cardScriptData.scriptableCard;
        //}
        //else if (cardListCardEvents != null)
        //{
        //    scriptableCard = cardListCardEvents.scriptableCard;
        //}

        if (scriptableCard == null)
        {
            return;
        }

        foreach (ScriptableKeywords scriptableKeyword in scriptableCard.scriptableKeywords)
        {
            this.gameObject.GetComponent<TooltipContent>().description += "<color=yellow>" + scriptableKeyword.keywordName + "</color> : " + scriptableKeyword.keywordDescription + "<br>";
        }

        foreach (ScriptableBuffDebuff scriptableBuffDebuff in scriptableCard.scriptableBuffDebuffs)
        {
            this.gameObject.GetComponent<TooltipContent>().description += BuffSystemManager.Instance.GetBuffDebuffColor(scriptableBuffDebuff) + " : " + scriptableBuffDebuff.description + "<br>";
        }

        foreach (ScriptableHazard scriptableHazard in scriptableCard.scriptableHazards)
        {
            this.gameObject.GetComponent<TooltipContent>().description += "<color=#" + SystemManager.Instance.colorGolden + ">" + scriptableHazard.hazardName + "</color> : " + scriptableHazard.hazardDescription + "<br>";
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.gameObject.GetComponent<TooltipContent>().description = "";
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.gameObject.GetComponent<TooltipContent>().description = "";
    }
}
