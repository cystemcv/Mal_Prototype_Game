using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    public int manaMaxAvailable = 3;
    public int manaAvailable = 0;     
    
    public int ManaAvailable
    {
        get { return manaAvailable; }
        set
        {
            if (manaAvailable == value) return;
            manaAvailable = value;
            if (onManaChange != null)
                onManaChange();
        }
    }

    public delegate void OnManaChange();
    public event OnManaChange onManaChange;

    private void OnEnable()
    {
        onManaChange += ManaDetectEvent;
    }

    private void OnDisable()
    {
        onManaChange -= ManaDetectEvent;
    }

    public void ManaDetectEvent()
    {

        //show the mana on UI
        UIManager.Instance.manaText.GetComponent<TMP_Text>().text = manaAvailable.ToString();

        //go throught each card in the hand and update them
        foreach (GameObject cardPrefab in HandManager.Instance.cardsInHandList) {
            UpdateCardAfterManaChange(cardPrefab);
        } 

    }

    public void UpdateCardAfterManaChange(GameObject cardPrefab)
    {
        ScriptableCard scriptableCard = cardPrefab.GetComponent<CardScript>().scriptableCard;
        TMP_Text cardManaCostText = cardPrefab.transform.GetChild(0).transform.Find("ManaBg").Find("ManaImage").Find("ManaText").GetComponent<TMP_Text>();


        //check the mana cost of each
        if (CombatManager.Instance.manaAvailable < scriptableCard.primaryManaCost)
        {
            //cannot be played
            cardManaCostText.color = new Color(255, 0, 0, 255);
        }
        else
        {
            //can be played
            cardManaCostText.color = new Color(255, 255, 255, 255);
        }
    }

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

    public void RefillMana()
    {
        //initialize mana and UI
        ManaAvailable = manaMaxAvailable;
    }

    public void StartCombat()
    {
        //give mana to player
        RefillMana();

        //change into combat mode
        SystemManager.Instance.currentSystemMode = SystemManager.SystemModes.COMBAT;

        //close the UI window
        UIManager.Instance.UIMENU.SetActive(false);

        //open the UI combat menu
        UIManager.Instance.UICOMBAT.SetActive(true);

        //test deck
        DeckManager.Instance.BuildStartingDeck();

   

        //initialize the characters

        //initialize the enemies

        //we draw for each character




    }

    public void PlayerTurn()
    {
        //mana should go back to full
        manaAvailable = manaMaxAvailable;
    }
}
