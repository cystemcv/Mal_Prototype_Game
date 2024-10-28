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

    public ScriptableCard scriptableCard;


    public override void OnPlayCard()
    {



    }

    public override void OnabilityActivate()
    {

        //get the copy card


        //add card on hand

        DeckManager.Instance.AddScriptableCardToHand(scriptableCard);


    }


}
