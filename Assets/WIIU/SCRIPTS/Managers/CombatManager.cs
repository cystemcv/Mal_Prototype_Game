using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

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

    public void StartCombat()
    {

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
}
