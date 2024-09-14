using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardScript : MonoBehaviour
{

    public ScriptableCard scriptableCard;

    public List<AIAbilityHolder> aIAbilityHolders;

    public GameObject whoUsedCard;

    [Header("ID")]
    public string cardID;

    //cards temp values
    public int tempValue = 0; //used for any ability that need to store value
    public int primaryManaCost = 0;
    public bool resetManaCost = false;
    public bool changedMana = false;

    public CardScript()
    {

        //add an id to this scriptableCard, this is in order to identify it by comparisons
        cardID = System.Guid.NewGuid().ToString();
    }


}
