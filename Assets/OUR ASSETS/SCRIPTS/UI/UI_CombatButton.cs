using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using TheraBytes.BetterUi;

public class UI_CombatButton : MonoBehaviour
{

    public int cdButton = 0;
    public Color primarySavedColor;
    public Color secondarySavedColor;

    public Image buttonIcon;
    public TMP_Text buttonText;


    public float disabledAlpha = 100;
    public float maxAlpha = 255;

    public bool activatedButton = false;

    public bool companionButton = false;

    // Start is called before the first frame update
    void Start()
    {

        buttonIcon = this.gameObject.transform.Find("Icon").GetComponent<Image>();
        buttonText = this.gameObject.transform.Find("Text").GetComponent<TMP_Text>();



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
            DisableButton();

        }
        else
        {
            EnableButton();
        }



    }



    public void DisableButton()
    {
        // Get the current color of the sprite
        Color colorP = buttonIcon.color;

        // Modify the alpha value
        colorP.a = disabledAlpha;

        buttonIcon.color = colorP;

        buttonText.gameObject.SetActive(true);
        buttonText.text = cdButton.ToString();

    }

    public void EnableButton()
    {

        // Get the current color of the sprite
        Color colorP = buttonIcon.color;

        // Modify the alpha value
        colorP.a = maxAlpha;

        buttonIcon.color = colorP;

        buttonText.gameObject.SetActive(false);
        buttonText.text = "";

    }
}
