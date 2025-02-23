using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Companion_Thief", menuName = "Companions/Companion_Thief")]
public class Companion_Thief : ScriptableCompanion
{

    public ScriptableCard copy;
    public ScriptableCard steal;
    public ScriptableItem stealCompanionUpgrade; //steal

    public override void OnPlayCard()
    {



    }

    public override void OnabilityActivate()
    {

        base.OnabilityActivate();

        GameObject companionBtn = GameObject.FindGameObjectWithTag("CompanionButton");
        if (!companionBtn.GetComponent<UI_CombatButton>().activatedButton)
        {
            return;
        }

        
        companionBtn.GetComponent<UI_CombatButton>().activatedButton = false;


        if (ItemManager.Instance.CheckIfItemExistOnList(StaticData.companionItemList, stealCompanionUpgrade) is ClassItem)
        {
            //add steal card on hand
            DeckManager.Instance.AddScriptableCardToHand(steal);
        }
        else
        {
            //add copy card on hand
            DeckManager.Instance.AddScriptableCardToHand(copy);
        }






    }


}
