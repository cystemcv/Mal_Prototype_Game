using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Combat : MonoBehaviour
{
    public static UI_Combat Instance;

    public delegate void OnManaChange();
    public event OnManaChange onManaChange;

    //variables

    public GameObject manaText;
    public GameObject deckUIObject;
    public GameObject discardUIObject;
    public GameObject banishedUIObject;

    [Header("SPAWN OF DISCARD CARD")]
    public GameObject handFullSpawnCard;

    [Header("SPAWN OF DISCARD CARD")]
    public GameObject CheckEnemyCard;

    [Header("OTHER UI")]
    public GameObject chooseACardScreen;

    [Header("NOTIFICATIONS")]
    public GameObject notificationParent;
    public GameObject notificationPb;


    [Header("LEADER")]
    public GameObject leaderIndicator;

    [Header("CARD LINE RENDERER")]
    public LineRenderer cardLineRenderer;
    public GameObject cardLineRendererArrow;


    public float fillbarVelocity = 0;
    public float fillbarSmoothValue = 0.3f;

    [Header("UI NUMBER ON SCREEN - DMG,HEAL,SHIELD")]
    public GameObject numberOnScreen;

    [Header("CONDITIONS")]
    public GameObject victory;
    public GameObject gameover;

    [Header("HAND")]
    public GameObject HAND;
    public GameObject CARDPLAYED;

    public int ManaAvailable
    {
        get { return CombatManager.Instance.manaAvailable; }
        set
        {
            if (CombatManager.Instance.manaAvailable == value) return;
            CombatManager.Instance.manaAvailable = value;
            if (onManaChange != null)
                onManaChange();
        }
    }

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

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
        manaText.GetComponent<TMP_Text>().text = CombatManager.Instance.manaAvailable.ToString();

        //go throught each card in the hand and update them
        foreach (GameObject cardPrefab in HandManager.Instance.cardsInHandList)
        {
            UpdateCardAfterManaChange(cardPrefab);
        }

    }

    public void UpdateCardAfterManaChange(GameObject cardPrefab)
    {
        ScriptableCard scriptableCard = cardPrefab.GetComponent<CardScript>().scriptableCard;
        CardScript cardScript = cardPrefab.GetComponent<CardScript>();
        TMP_Text cardManaCostText = cardPrefab.transform.GetChild(0).transform.Find("ManaBg").Find("ManaImage").Find("ManaText").GetComponent<TMP_Text>();


        //check the mana cost of each
        if (CombatManager.Instance.manaAvailable < cardScript.primaryManaCost)
        {
            //cannot be played
            cardManaCostText.color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorRed);
        }
        else
        {
            //can be played
            cardManaCostText.color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorWhite);
        }
    }

    public void OnNotification(string message, float waitTime)
    {
        // Instantiate at position (0, 0, 0) and zero rotation.
        GameObject notificationPrefab = Instantiate(notificationPb, notificationParent.transform.position, Quaternion.identity);

        //set it as a child of the parent
        notificationPrefab.transform.SetParent(notificationParent.transform);

        //open the notification throught animations
        Animator notificationAnimator = notificationPrefab.GetComponent<Animator>();
        notificationAnimator.SetTrigger("On");

        //set the text to our message
        notificationPrefab.transform.Find("TEXT").GetComponent<TMP_Text>().text = message;

        //start waiting time
        StartCoroutine(WaitNotification(notificationPrefab, waitTime));
    }

    public void OffNotification(GameObject notificationPrefab, float timeToDestroy)
    {
        //open the notification throught animations
        Animator notificationAnimator = notificationPrefab.GetComponent<Animator>();
        notificationAnimator.SetTrigger("Off");

        Destroy(notificationPrefab, timeToDestroy);
    }

    IEnumerator WaitNotification(GameObject notificationPrefab, float waitTime)
    {



        // Wait for 2 seconds
        yield return new WaitForSeconds(waitTime);
        OffNotification(notificationPrefab, 2f);


    }

    // Start is called before the first frame update
    void Start()
    {

        //cardLineRenderer = this.transform.Find("TARGET LINERENDERER").GetComponent<LineRenderer>();

        // Set the initial positions of the line (start and end)
        cardLineRenderer.positionCount = 2;

        // Get the material of the Line Renderer
        Material material = cardLineRenderer.material;

        // Set the sorting order to a high value to ensure it renders on top
        material.renderQueue = 999999; // Adjust the value as needed
    }

    // Update is called once per frame
    void Update()
    {

        deckUIObject.transform.Find("TextHolder").Find("Text").GetComponent<TMP_Text>().text = DeckManager.Instance.combatDeck.Count.ToString();
        discardUIObject.transform.Find("TextHolder").Find("Text").GetComponent<TMP_Text>().text = DeckManager.Instance.discardedPile.Count.ToString();
        banishedUIObject.transform.Find("TextHolder").Find("Text").GetComponent<TMP_Text>().text = DeckManager.Instance.banishedPile.Count.ToString();

    }

    public void UpdateHealthBarSmoothly(float health, float maxHealth, Slider slider)
    {
        StartCoroutine(SmoothUpdateHealthBar(health, maxHealth, slider));
    }

    private IEnumerator SmoothUpdateHealthBar(float health, float maxHealth, Slider slider)
    {
        float targetValue = health / maxHealth;
        while (Mathf.Abs(slider.value - targetValue) > 0.01f)
        {
            slider.value = Mathf.SmoothDamp(slider.value, targetValue, ref fillbarVelocity, fillbarSmoothValue);
            yield return null; // Wait for the next frame
        }
        slider.value = targetValue; // Ensure it's set to the target value at the end
    }
}
