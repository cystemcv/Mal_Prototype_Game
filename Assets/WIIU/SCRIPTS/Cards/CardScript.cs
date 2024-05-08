using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardScript : MonoBehaviour
{

    public ScriptableCard scriptableCard;

    [Header("ID")]
    public string cardID;

   public CardScript()
    {
        
        //add an id to this scriptableCard, this is in order to identify it by comparisons
        cardID = System.Guid.NewGuid().ToString();
    }

}
