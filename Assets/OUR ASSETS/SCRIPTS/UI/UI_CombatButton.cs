using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using TheraBytes.BetterUi;
using Michsky.MUIP;

public class UI_CombatButton : MonoBehaviour
{

    public int cdButton = 0;
    public Color primarySavedColor;
    public Color secondarySavedColor;

    public Image buttonIcon;
    public TMP_Text buttonText;


    public float disabledAlpha = 100;
    public float maxAlpha = 255;


    public bool companionButton = false;


    // Start is called before the first frame update
    void Start()
    {


        if (companionButton) {

            //if null get the first choice
            if (StaticData.staticScriptableCompanion == null)
            {
                StaticData.staticScriptableCompanion = CharacterManager.Instance.companionList[0].Clone();
            }

            this.gameObject.GetComponent<ButtonManager>().SetText(StaticData.staticScriptableCompanion.companionAbilityName);
            this.gameObject.GetComponent<ButtonManager>().SetIcon(StaticData.staticScriptableCompanion.companionAbilityIcon);
        }




        // Subscribe to the TurnsChanged event
        if (Combat.Instance != null)
        {
            Combat.Instance.TurnsChanged += OnTurnsChanged;
        }

    }

    void OnDestroy()
    {
        // Unsubscribe from the TurnsChanged event to avoid memory leaks
        if (Combat.Instance != null)
        {
            Combat.Instance.TurnsChanged -= OnTurnsChanged;
        }
    }

    // Event handler for when Turns changes
    public void OnTurnsChanged(int newTurns)
    {
        if (cdButton > 0)
        {
            cdButton--;
            UpdateButton();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateButton()
    {

        if (cdButton != 0)
        {
            this.gameObject.GetComponent<ButtonManager>().Interactable(false);

        }
        else
        {
            this.gameObject.GetComponent<ButtonManager>().Interactable(true);
        }



    }

    public void ActivateCompanionAbility()
    {
        if (companionButton)
        {
            StaticData.staticScriptableCompanion.OnabilityActivate();
        }
    }



}
