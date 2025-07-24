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
        CardListCardEvents cardListCardEvents = this.gameObject.GetComponent<CardListCardEvents>();
        ScriptableCard scriptableCard = null;

        if (cardScript != null)
        {
            scriptableCard = cardScript.scriptableCard;
        }
        else if (cardListCardEvents != null)
        {
            scriptableCard = cardListCardEvents.scriptableCard;
        }

        foreach (ScriptableKeywords scriptableKeyword in scriptableCard.scriptableKeywords)
        {
            this.gameObject.GetComponent<TooltipContent>().description += "<color=yellow>" + scriptableKeyword.keywordName + "</color> : " + scriptableKeyword.keywordDescription + "<br>";
        }

        foreach (ScriptableBuffDebuff scriptableBuffDebuff in scriptableCard.scriptableBuffDebuffs)
        {
            this.gameObject.GetComponent<TooltipContent>().description += BuffSystemManager.Instance.GetBuffDebuffColor(scriptableBuffDebuff) + " : " + scriptableBuffDebuff.description + "<br>";
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
