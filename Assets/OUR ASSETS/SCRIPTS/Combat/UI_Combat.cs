using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Combat : MonoBehaviour
{
    public static UI_Combat Instance;

    [Header("TUTORIALS")]
    public TutorialData tutorial_entity;
    public TutorialData tutorial_cards;
    public TutorialData tutorial_enemies;
    public TutorialData tutorial_positioning;
    public TutorialData tutorial_targeting;
    public TutorialData tutorial_buffsDebuffs;


    //variables

    public bool uiCombatEnable = false;
    public GameObject manaInfo;
    public GameObject deckInfo;
    public GameObject discardInfo;
    public GameObject banishedInfo;
    public GameObject moveHeroButton;

    [Header("SPAWN OF DISCARD CARD")]
    public GameObject handFullSpawnCard;

    [Header("SPAWN OF DISCARD CARD")]
    public GameObject CheckEnemyCard;


    [Header("NOTIFICATIONS")]
    public GameObject notificationParent;
    public GameObject notificationPb;

    [Header("ENTITY UI")]
    public GameObject commonGameobjectUI;
    public GameObject playerGameobjectUI;

    [Header("INDICATORS")]
    public GameObject displayCardName;

    [Header("CARD LINE RENDERER")]
    public LineRenderer cardLineRenderer;
    public GameObject cardLineRendererArrow;
    public GameObject moveHeroText;


    public float fillbarVelocity = 0;
    public float fillbarSmoothValue = 0.3f;

    [Header("UI NUMBER ON SCREEN - DMG,HEAL,SHIELD")]
    public GameObject numberOnScreen;

    [Header("HAND")]
    public GameObject HAND;
    public GameObject CARDPLAYED;

    public GameObject endTurnButton;
    public GameObject companionAbilityButton;

    [Header("PLAYED CARDS")]
    public GameObject playedCardsGO;
    public GameObject playedCardPrefab;


    public GameObject targettingPrefab;




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



    public GameObject AddPlayedCardUI(PlayedCard playedCard)
    {

        GameObject playedCardUI = Instantiate(playedCardPrefab, playedCardsGO.transform.position, Quaternion.identity);
        playedCardUI.transform.SetParent(playedCardsGO.transform);
        playedCardUI.transform.localScale = new Vector3(1, 1, 1);

        playedCardUI.transform.Find("CardName").GetComponent<TMP_Text>().text = playedCard.scriptableCard.cardName;
        playedCardUI.transform.Find("CardTimer").GetComponent<TMP_Text>().text = playedCard.timer.ToString("F1");

        return playedCardUI;

    }

    public void RemovePlayedCardUI(PlayedCard playedCard)
    {
        playedCard.playedCardUI.GetComponent<Animator>().SetTrigger("End");
        Destroy(playedCard.playedCardUI, 1f);
    }


    public IEnumerator OnNotification(string message, float waitTime)
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
        yield return StartCoroutine(WaitNotification(notificationPrefab, waitTime));

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

        UIManager.Instance.topPanelCombat.SetActive(true);

        //cardLineRenderer = this.transform.Find("TARGET LINERENDERER").GetComponent<LineRenderer>();

        // Set the initial positions of the line (start and end)
        cardLineRenderer.positionCount = 2;

        // Get the material of the Line Renderer
        Material material = cardLineRenderer.material;

        // Set the sorting order to a high value to ensure it renders on top
        material.renderQueue = 999999; // Adjust the value as needed

        CombatManager.Instance.UiTrainingButton();

        //manaInfo.GetComponent<ButtonManager>().Interactable(false);
    }

    // Update is called once per frame
    void Update()
    {




        //deckInfo.transform.Find("Text").GetComponent<TMP_Text>().text = DeckManager.Instance.combatDeck.Count.ToString();
        //discardInfo.transform.Find("Text").GetComponent<TMP_Text>().text = DeckManager.Instance.discardedPile.Count.ToString();
        //banishedInfo.transform.Find("Text").GetComponent<TMP_Text>().text = DeckManager.Instance.banishedPile.Count.ToString();

    }

    private void FixedUpdate()
    {
        deckInfo.GetComponent<ButtonManager>().SetOnlyText("DECK [" + DeckManager.Instance.combatDeck.Count.ToString() + "]");
        discardInfo.GetComponent<ButtonManager>().SetOnlyText("DISCARD [" + DeckManager.Instance.discardedPile.Count.ToString() + "]");
        banishedInfo.GetComponent<ButtonManager>().SetOnlyText("BANISH [" + DeckManager.Instance.banishedPile.Count.ToString() + "]");
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

    public void BackToAdventure()
    {
        ItemManager.Instance.HideLootParent();
        ItemManager.Instance.HideInventory();


        SystemManager.Instance.LoadScene("scene_Adventure", 0f, true, false);
    }

    public void BackToMainMenu()
    {
        ItemManager.Instance.HideLootParent();
        ItemManager.Instance.HideInventory();

        //allo dungeon generation again
        StaticData.staticDungeonParentGenerated = false;
        SystemManager.Instance.LoadScene("scene_MainMenu", 0f, true, true);
    }

    public void ActivateCompanionAbility()
    {



        StaticData.staticScriptableCompanion.OnabilityActivate();

    }

    public void OpenDeckCardList()
    {
        UIManager.Instance.OpenDeckList();
    }

    public void OpenDiscardCardList()
    {
        UIManager.Instance.OpenDiscardList();
    }

    public void OpenBanishedCardList()
    {
        UIManager.Instance.OpenBanishedList();
    }

    public void HideCardList()
    {
        UIManager.Instance.HideCardList();
    }


    public void ActivateMoveButton()
    {
        SystemManager.Instance.abilityMode = SystemManager.AbilityModes.MOVEHERO;

        List<string> tagList = new List<string>();
        tagList.Add("Player");
        tagList.Add("PlayerSummon");
        tagList.Add("PlayerPos");
        List<GameObject> targetPosList = Combat.Instance.FindPosTargeting(tagList);

        List<GameObject> targets = SystemManager.Instance.FindGameObjectsWithTags(tagList);

        targets.AddRange(targetPosList);
    }


    public void DisableCombatUI()
    {
        uiCombatEnable = false;
        manaInfo.GetComponent<ButtonManager>().Interactable(false); 
        deckInfo.GetComponent<ButtonManager>().Interactable(false); 
        discardInfo.GetComponent<ButtonManager>().Interactable(false); 
        banishedInfo.GetComponent<ButtonManager>().Interactable(false);
        endTurnButton.GetComponent<ButtonManager>().Interactable(false);
        companionAbilityButton.GetComponent<ButtonManager>().Interactable(false);

    }

    public void EnableCombatUI()
    {
        uiCombatEnable = true;
        manaInfo.GetComponent<ButtonManager>().Interactable(true); 
        deckInfo.GetComponent<ButtonManager>().Interactable(true); 
        discardInfo.GetComponent<ButtonManager>().Interactable(true);
        banishedInfo.GetComponent<ButtonManager>().Interactable(true);
        endTurnButton.GetComponent<ButtonManager>().Interactable(true);
        companionAbilityButton.GetComponent<ButtonManager>().Interactable(true);
    }

}
