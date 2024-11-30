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
    public string textSaved = "";

    public BetterImage backgroundGO;
    public TMP_Text buttonText;


    public float disabledAlpha = 100;
    public float maxAlpha = 255;

    public bool activatedButton = false;

    // Start is called before the first frame update
    void Start()
    {

        backgroundGO = this.gameObject.transform.Find("bg").GetComponent<BetterImage>();
        buttonText = this.gameObject.transform.Find("Text").GetComponent<TMP_Text>();

        primarySavedColor = backgroundGO.color;
        secondarySavedColor = backgroundGO.SecondColor;

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
        Color colorP = primarySavedColor;
        Color colorS = secondarySavedColor;

        // Modify the alpha value
        colorP.a = disabledAlpha;
        colorS.a = disabledAlpha;

        // Apply the modified color back to the SpriteRenderer
        backgroundGO.color = colorP;
        backgroundGO.SecondColor = colorS;

        buttonText.text = cdButton.ToString();

    }

    public void EnableButton()
    {

        // Get the current color of the sprite
        Color colorP = primarySavedColor;
        Color colorS = secondarySavedColor;

        // Modify the alpha value
        colorP.a = maxAlpha;
        colorS.a = maxAlpha;

        // Apply the modified color back to the SpriteRenderer
        backgroundGO.color = colorP;
        backgroundGO.SecondColor = colorS;

        buttonText.text = textSaved;

    }
}
