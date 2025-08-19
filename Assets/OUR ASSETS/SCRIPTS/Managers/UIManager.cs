using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;
using System.Linq;

public class OptionsSettings
{
    public List<CardScriptData> cardScriptDataList;
    public UIManager.CardListMode cardListMode;
    public bool enableCloseButton;
    public int enableMinSelection;
    public int enableMaxSelection;
    public string title;
    public Action onConfirmAction;
    public bool allowClassButtons;
    public bool allowDuplicates = false;
}

public class UIManager : MonoBehaviour
{



    public static UIManager Instance;

    private Dictionary<TMP_Text, Tweener> activeTweeners = new Dictionary<TMP_Text, Tweener>();

    public GameObject tooltipGlobalGO;

    public GameObject tooltipPanel;
    public TMP_Text tooltipText;  // Use Text instead of TextMeshPro if preferred
                                  // Padding to prevent tooltip from overlapping hovered element
    public float offsetPadding = 10f;

    private RectTransform panelRectTransform;

    //inventory
    public GameObject inventoryMain;
    public GameObject lootMain;
    public GameObject inventoryGO;
    public GameObject companionGO;
    public GameObject artifactGO;
    public GameObject lootGO;

    public TMP_Text lootText;
    public TMP_Text inventoryText;
    public TMP_Text companionText;
    public TMP_Text artifactText;
    public TMP_Text lootTextDescription;
    public TMP_Text inventoryTextDescription;
    public TMP_Text companionTextDescription;
    public TMP_Text artifactTextDescription;

    public GameObject combatEndWindow;

    public GameObject ChooseGroupUI;

    //item activation ordr
    public GameObject itemActivateOrderPanel;
    public GameObject ItemActivateOrderPrefab;

    [Header("TOP PANEL COMBAT")]
    public GameObject topPanelCombat;

    [Header("RESULTS WINDOW")]
    public GameObject resultsWindow;
    public GameObject resultsWindow_Text;
    public GameObject resultsWindow_Title;
    public GameObject resultsWindow_ScoringText;
    public GameObject resultsWindow_NextButton;
    public GameObject resultsWindow_LootButton;
    public GameObject resultsWindow_EndButton;

    [Header("SETTINGS")]
    //modal stuff
    [SerializeField] private ModalWindowManager modalWindowManager;
    [SerializeField] private Sprite modalIcon;

    [Header("NOTIFICATIONS")]
    public GameObject notificationParent;
    public GameObject notificationPb;

    [Header("CARD LIST")]
    public bool enableCloseButton = false;
    public int enableMaxSelection = 0;
    public int enableMinSelection = 0;
    public List<CardScriptData> selectedCardList = new List<CardScriptData>();
    public enum CardListMode { VIEW, EDIT }
    public CardListMode cardListMode = CardListMode.VIEW;
    public GameObject cardListGO;
    public GameObject cardListGOContent;
    public GameObject cardPrefabScaleWithScreenOverlay;
    List<CardScriptData> cardScriptDataList = new List<CardScriptData>();
    public bool cardScriptAllowDuplicates = false;

    public GameObject eventGO;
    public GameObject eventButtonPrefab;
    public GameObject closeButtonPrefab;

    public GameObject shopUI;

    public GameObject turnText;

    private int eventConversation = 0;
    private bool eventButtonsShown = false;
    private bool canExitEvent = false;

    [Header("CRAFTING")]
    public GameObject craftingPanelUI;
    public GameObject craftingPanelUI_content;
    public GameObject recipeUIPrefab;

    [Header("GALAXY")]
    public GameObject galaxyScaling;

    public GameObject QuickMenu;

    public void ToggleActivateOrderVisibility()
    {
        // Toggle the active state of the parent of itemActivateOrderPanel
        Transform parent = itemActivateOrderPanel.transform.parent;
        if (parent != null)
        {
            bool isActive = parent.gameObject.activeSelf;
            parent.gameObject.SetActive(!isActive);
        }
        else
        {
            Debug.LogWarning("ItemActivateOrderPanel has no parent!");
        }
    }


    //public delegate void OnCurrentModeChange();
    //public event OnCurrentModeChange onCurrentModeChange;

    ////this variable has event so we use getter and setter
    //public SystemManager.SystemModes UICurrentMode
    //{
    //    get { return SystemManager.Instance.systemMode; }
    //    set
    //    {
    //        if (SystemManager.Instance.systemMode == value) return;
    //        SystemManager.Instance.systemMode = value;
    //        if (onCurrentModeChange != null)
    //            onCurrentModeChange();
    //    }
    //}


    //public delegate void OnCurrentUIScreenChange();
    //public event OnCurrentUIScreenChange onCurrentUIScreenChange;

    ////this variable has event so we use getter and setter
    //public  SystemManager.UIScreens CurrentUIScreen
    //{
    //    get { return SystemManager.Instance.currentUIScreen; }
    //    set
    //    {
    //        if (SystemManager.Instance.currentUIScreen == value) return;
    //        SystemManager.Instance.currentUIScreen = value;
    //        if (onCurrentUIScreenChange != null)
    //            onCurrentUIScreenChange();
    //    }


    //}

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


        panelRectTransform = tooltipPanel.GetComponent<RectTransform>();
        tooltipPanel.SetActive(false);

    }



    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

    }



    public void ShowTooltip(string content, Vector2 hoverPosition, Vector2 hoverSize, GameObject hoverObject)
    {
        tooltipText.text = content;
        tooltipPanel.SetActive(true);

        UIManager.Instance.AdjustTooltipPosition(hoverPosition, hoverSize, FindParentCanvas(hoverObject.transform));
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }
    private void AdjustTooltipPosition(Vector2 hoverPosition, Vector2 hoverSize, Canvas canvas)
    {
        // Get the RectTransform of the tooltip and the canvas
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 tooltipSize = panelRectTransform.sizeDelta;

        // Convert hover screen position to local position within canvas space
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, hoverPosition, canvas.worldCamera, out Vector2 localHoverPosition);

        // Calculate the desired position for the tooltip above the hovered element
        Vector2 targetPosition = localHoverPosition + new Vector2(0, hoverSize.y / 2 + tooltipSize.y / 2 + offsetPadding);

        // Check if the tooltip goes off the top of the screen
        if (targetPosition.y + tooltipSize.y / 2 > canvasRect.rect.height / 2)
        {
            // Position the tooltip below the hovered element
            targetPosition.y = localHoverPosition.y - (hoverSize.y / 2 + tooltipSize.y + offsetPadding);
        }

        // Ensure the tooltip does not go off the bottom of the screen
        if (targetPosition.y - tooltipSize.y / 2 < -canvasRect.rect.height / 2)
        {
            targetPosition.y = -canvasRect.rect.height / 2 + tooltipSize.y / 2 + offsetPadding;
        }

        // Ensure the tooltip is within the left and right bounds of the screen
        float halfTooltipWidth = tooltipSize.x / 2;
        targetPosition.x = Mathf.Clamp(targetPosition.x, -canvasRect.rect.width / 2 + halfTooltipWidth + offsetPadding,
                                        canvasRect.rect.width / 2 - halfTooltipWidth - offsetPadding);

        // Ensure the tooltip remains within the canvas bounds after calculating position
        targetPosition.x = Mathf.Clamp(targetPosition.x, -canvasRect.rect.width / 2 + halfTooltipWidth,
                                        canvasRect.rect.width / 2 - halfTooltipWidth);

        targetPosition.y = Mathf.Clamp(targetPosition.y, -canvasRect.rect.height / 2 + tooltipSize.y / 2,
                                        canvasRect.rect.height / 2 - tooltipSize.y / 2);

        // Set the tooltip position to the calculated target position
        panelRectTransform.localPosition = targetPosition;
    }



    private Canvas FindParentCanvas(Transform element)
    {
        Transform current = element;
        while (current != null)
        {
            Canvas canvas = current.GetComponent<Canvas>();
            if (canvas != null)
                return canvas;
            current = current.parent;
        }
        return null; // No canvas found in the parent hierarchy
    }


    public void OpenQuickMenu() {
        QuickMenu.SetActive(true);
        FeedbackManager.Instance.PlayOnTarget(QuickMenu.transform, FeedbackManager.Instance.mm_OpenPanel_Prefab);
        Time.timeScale = 0f; // Pause game
    }

    public void CloseQuickMenu()
    {
        QuickMenu.SetActive(false);
        Time.timeScale = 1f; // Resume game
    }

    //open
    public void OpenModal(string title, string desc, UnityEngine.Events.UnityAction function)
    {
        //remove all the listeners so we dont have stack of listeners
        modalWindowManager.gameObject.SetActive(true);
        modalWindowManager.onConfirm.RemoveAllListeners();
        modalWindowManager.onCancel.RemoveAllListeners();


        //play the modal sound
        AudioManager.Instance.PlaySfx("UI_Modal");

        //modal details
        modalWindowManager.icon = modalIcon; // Change icon
        modalWindowManager.titleText = title; // Change title
        modalWindowManager.descriptionText = desc; // Change desc
        modalWindowManager.UpdateUI(); // Update UI
        //modalWindowManager.Open(); // Open window
        modalWindowManager.AnimateWindow(); // Close/Open window automatically
        modalWindowManager.onConfirm.AddListener(CloseModal); // Add confirm events
        modalWindowManager.onConfirm.AddListener(function); // Add confirm events
        modalWindowManager.onCancel.AddListener(CloseModal); // Add cancel events

        //move the modal in the correct position (this is to keep it open and do not interfene with scene)
        modalWindowManager.gameObject.transform.parent.localPosition = new Vector3(0, 0, 0);
    }

    public void CloseModal()
    {
        StartCoroutine(CloseWindowManager());
        AudioManager.Instance.PlaySfx("UI_Cancel");
        modalWindowManager.Close(); // Close window
    }

    public void CloseModalConfirm()
    {
        StartCoroutine(CloseWindowManager());
        AudioManager.Instance.PlaySfx("UI_Confirm");
        modalWindowManager.Close(); // Close window
    }

    public IEnumerator CloseWindowManager()
    {
        yield return new WaitForSeconds(0.2f);
        modalWindowManager.gameObject.SetActive(false);
    }

    //public void GoToScene(string sceneName)
    //{
    //    AudioManager.Instance.PlaySfx("UI_Confirm");
    //    AudioManager.Instance.PlayMusic("Gameplay_1");

    //    //change UI to gameplay
    //    UIManager.Instance.UICurrentMode = SystemManager.SystemModes.GAMEPLAY;

    //    //make UI inactive
    //    SystemManager.Instance.EnableDisable_UIManager(false);

    //    //load scene
    //    SceneManager.LoadScene(sceneName);


    //}

    public void GoToMainMenu()
    {
        OpenModal("BACK TO MAIN MENU", "Are you sure you want to go back to Main Menu? (Any unsaved changes will be lost!)", GoToMainMenuConfirm);
    }

    public void GoToMainMenuConfirm()
    {

        //hide the dungeon generator
        CustomDungeonGenerator.Instance.HideCustomDungeonGenerator();

        UIManager.Instance.CloseQuickMenu();

        //make UI inactive
        //SystemManager.Instance.EnableDisable_UIManager(true);

        //load scene
        //SceneManager.LoadScene("scene_MainMenu");
        SystemManager.Instance.LoadScene("scene_MainMenu", 0f,true,true);
    }

    //public void CloseUIWindow()
    //{
    //    AudioManager.Instance.PlaySfx("UI_goBack");

    //    SystemManager.Instance.EnableDisable_UIManager(false);
    //}

    //public void ExitGame()
    //{
    //    Application.Quit();
    //}





    ////notifications
    //public void OnNotification(string message, float waitTime)
    //{
    //    // Instantiate at position (0, 0, 0) and zero rotation.
    //    GameObject notificationPrefab = Instantiate(notificationPb, notificationParent.transform.position, Quaternion.identity);

    //    //set it as a child of the parent
    //    notificationPrefab.transform.SetParent(notificationParent.transform);

    //    //open the notification throught animations
    //    Animator notificationAnimator = notificationPrefab.GetComponent<Animator>();
    //    notificationAnimator.SetTrigger("On");

    //    //set the text to our message
    //    notificationPrefab.transform.Find("TEXT").GetComponent<TMP_Text>().text = message;

    //    //start waiting time
    //    StartCoroutine(WaitNotification(notificationPrefab, waitTime));
    //}

    //public void OffNotification(GameObject notificationPrefab, float timeToDestroy)
    //{
    //    //open the notification throught animations
    //    Animator notificationAnimator = notificationPrefab.GetComponent<Animator>();
    //    notificationAnimator.SetTrigger("Off");

    //    Destroy(notificationPrefab, timeToDestroy);
    //}

    //IEnumerator WaitNotification(GameObject notificationPrefab, float waitTime)
    //{



    //    // Wait for 2 seconds
    //    yield return new WaitForSeconds(waitTime);
    //    OffNotification(notificationPrefab, 2f);


    //}




    public void DisableButton(GameObject button)
    {
        //disable it
        button.GetComponent<Button>().interactable = false;

        //then we disable everything except the bg

        foreach (Transform child in button.transform)
        {

            //we want to get the color

            //check if its not the background because we do not want to affect this
            if (child.name == "background")
            {

                continue;
            }

            //check if its a text
            TMP_Text childText = child.GetComponent<TMP_Text>();

            if (childText != null)
            {
                Color32 originalColor = childText.color;

                //change the color to transparetn
                childText.color = new Color32(originalColor.r, originalColor.g, originalColor.b, 30);
            }


            //check if its an image
            Image childImage = child.GetComponent<Image>();

            if (childImage != null)
            {
                Color32 originalColor = childImage.color;

                //change the color to transparetn
                childImage.color = new Color32(originalColor.r, originalColor.g, originalColor.b, 30);
            }

        }


    }

    public void EnableButton(GameObject button)
    {
        //disable it
        button.GetComponent<Button>().interactable = true;

        //then we disable everything except the bg

        foreach (Transform child in button.transform)
        {

            //we want to get the color

            //check if its not the background because we do not want to affect this
            if (child.name == "background")
            {

                continue;
            }

            //check if its a text
            TMP_Text childText = child.GetComponent<TMP_Text>();

            if (childText != null)
            {
                Color32 originalColor = childText.color;

                //change the color to transparetn
                childText.color = new Color32(originalColor.r, originalColor.g, originalColor.b, 255);
            }


            //check if its an image
            Image childImage = child.GetComponent<Image>();

            if (childImage != null)
            {
                Color32 originalColor = childImage.color;

                //change the color to transparetn
                childImage.color = new Color32(originalColor.r, originalColor.g, originalColor.b, 255);
            }

        }


    }

    public void SkipChooseGroupUI()
    {
        //resume
        SystemManager.Instance.abilityMode = SystemManager.AbilityModes.NONE;

        SystemManager.Instance.DestroyAllChildren(UIManager.Instance.ChooseGroupUI.transform.Find("ChooseContainer").gameObject);

        UIManager.Instance.ChooseGroupUI.SetActive(false);

    }

    public void OpenCardList()
    {



    }

    public void OpenMainDeckList()
    {
        OptionsSettings optionsSettings = new OptionsSettings();
        optionsSettings.cardScriptDataList = StaticData.staticMainDeck;
        optionsSettings.cardListMode = CardListMode.VIEW;
        optionsSettings.enableCloseButton = true;
        optionsSettings.enableMinSelection = 0;
        optionsSettings.enableMaxSelection = 0;
        optionsSettings.title = "Deck";
        optionsSettings.onConfirmAction = CardList_DoNothing;
        optionsSettings.allowClassButtons = false;
        ShowCardList(optionsSettings);
    }

    public void OpenDeckList()
    {
        OptionsSettings optionsSettings = new OptionsSettings();
        optionsSettings.cardScriptDataList = DeckManager.Instance.combatDeck;
        optionsSettings.cardListMode = CardListMode.VIEW;
        optionsSettings.enableCloseButton = true;
        optionsSettings.enableMinSelection = 0;
        optionsSettings.enableMaxSelection = 0;
        optionsSettings.title = "Remaining Deck";
        optionsSettings.onConfirmAction = CardList_DoNothing;
        optionsSettings.allowClassButtons = false;
        ShowCardList(optionsSettings);
    }

    private void CardList_DoNothing()
    {
        Debug.Log("Does nothing");
    }

    public void OpenDiscardList()
    {
        OptionsSettings optionsSettings = new OptionsSettings();
        optionsSettings.cardScriptDataList = DeckManager.Instance.discardedPile;
        optionsSettings.cardListMode = CardListMode.VIEW;
        optionsSettings.enableCloseButton = true;
        optionsSettings.enableMinSelection = 0;
        optionsSettings.enableMaxSelection = 0;
        optionsSettings.title = "Discarded Cards";
        optionsSettings.onConfirmAction = CardList_DoNothing;
        optionsSettings.allowClassButtons = false;
        ShowCardList(optionsSettings);
    }

    public void OpenBanishedList()
    {
        OptionsSettings optionsSettings = new OptionsSettings();
        optionsSettings.cardScriptDataList = DeckManager.Instance.banishedPile;
        optionsSettings.cardListMode = CardListMode.VIEW;
        optionsSettings.enableCloseButton = true;
        optionsSettings.enableMinSelection = 0;
        optionsSettings.enableMaxSelection = 0;
        optionsSettings.title = "Banished Cards";
        optionsSettings.onConfirmAction = CardList_DoNothing;
        optionsSettings.allowClassButtons = false;
        ShowCardList(optionsSettings);
    }

    //-----------------

    public void HideCardList()
    {
        ResetCardList();

        this.cardListGO.SetActive(false);
    }



    public void ShowCardList(OptionsSettings optionsSettings)
    {
        cardScriptAllowDuplicates = optionsSettings.allowDuplicates;
        ResetCardList();



        this.cardListMode = optionsSettings.cardListMode;
        this.enableCloseButton = optionsSettings.enableCloseButton;
        this.enableMinSelection = optionsSettings.enableMinSelection;
        this.enableMaxSelection = optionsSettings.enableMaxSelection;
        this.cardScriptDataList = optionsSettings.cardScriptDataList;


        GameObject confirmButtonGO = this.cardListGO.transform.Find("Buttons").Find("btn_Confirm").gameObject;
        //enableMaxSelection_Selected = 0;
        selectedCardList.Clear();

        this.cardListGO.SetActive(true);
        FeedbackManager.Instance.PlayOnTarget(this.cardListGO.transform, FeedbackManager.Instance.mm_OpenPanel_Prefab);
        this.cardListGO.transform.Find("Others").Find("Title").GetComponent<TMP_Text>().text = optionsSettings.title;

        GameObject closeButton = this.cardListGO.transform.Find("Buttons").Find("btn_Close").gameObject;

        if (!this.enableCloseButton)
        {
            closeButton.SetActive(false);
        }
        else
        {
            closeButton.SetActive(true);
        }

        GameObject selectionText = this.cardListGO.transform.Find("Others").Find("SelectionText").gameObject;


        if (enableMaxSelection > 0)
        {
            confirmButtonGO.SetActive(true);
            selectionText.SetActive(true);
            selectionText.GetComponent<TMP_Text>().text = selectedCardList.Count + "/" + this.enableMaxSelection;
        }
        else
        {
            selectionText.SetActive(false);
            confirmButtonGO.SetActive(false);
        }

        DeleteAllListTabButtons();
        if (optionsSettings.allowClassButtons)
        {
            //create the class buttons and also filter the list 
            CreateClassButtonsForCardList();
        }

        AssignCardsOnCardList();


        Button confirmButton = confirmButtonGO.GetComponent<Button>();
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            Debug.Log("Confirm button clicked!");
            if (optionsSettings.onConfirmAction == null) Debug.LogWarning("onConfirmAction is null");
            else
            {
                optionsSettings.onConfirmAction.Invoke();
            }
            cardListGO.SetActive(false);



        });

    }



    public void CreateClassButtonsForCardList()
    {

        List<string> cardClassesStrings = CardListManager.Instance.GetAllClassCardsFromList(this.cardScriptDataList);

        if (cardClassesStrings.Count == 0)
        {
            return;
        }

        List<SystemManager.MainClass> cardClasses = CardListManager.Instance.ConvertStringListToMainClassList(cardClassesStrings);
        FillCardListDataByClass(cardClasses[0]);

        CreateCardListTabButtons(cardClasses);
    }

    public void CreateCardListTabButtons(List<SystemManager.MainClass> cardClasses)
    {
        foreach (SystemManager.MainClass mainClass in cardClasses)
        {

            //instantiate the button
            GameObject classButton = Instantiate(CardListManager.Instance.cardListTabButton, CardListManager.Instance.cardListTabButtonParent.transform, false);

            //change the text
            classButton.GetComponent<ButtonManager>().SetText(mainClass.ToString());

            //assign the value
            classButton.GetComponent<CardListButtonTab>().mainClass = mainClass;

            CardListManager.Instance.cardListTabButtonList.Add(classButton);

        }
    }

    public void DeleteAllListTabButtons()
    {
        SystemManager.Instance.DestroyAllChildren(CardListManager.Instance.cardListTabButtonParent);
        CardListManager.Instance.cardListTabButtonList.Clear();
    }

    public void FillCardListDataByClass(SystemManager.MainClass mainClass)
    {
        List<SystemManager.MainClass> firstClass = new List<SystemManager.MainClass>();
        firstClass.Add(mainClass);

        //filter from the first class
        List<ScriptableCard> scriptableCards = CardListManager.Instance.ChooseCards(firstClass, null, null, null, null, null, true);

        //remove the list
        this.cardScriptDataList.Clear();

        foreach (ScriptableCard scriptableCard in scriptableCards)
        {
            CardScriptData cardScriptData = new CardScriptData();
            cardScriptData.scriptableCard = scriptableCard;

            this.cardScriptDataList.Add(cardScriptData);
        }
    }

    public void ResetCardList()
    {
        foreach (Transform card in cardListGOContent.transform)
        {
            Destroy(card.gameObject);
        }
    }

    public void AssignCardsOnCardList()
    {



        foreach (CardScriptData cardScriptData in this.cardScriptDataList)
        {
            //CardScriptData cardScriptData = new CardScriptData();
            //cardScriptData.scriptableCard = scriptableCard;
            //instantiate the card
            GameObject card = DeckManager.Instance.InitializeCardPrefab(cardScriptData, cardListGOContent, false, false);

            //do extra stuff on card
            Destroy(card.GetComponent<CanvasScaler>());
            Destroy(card.GetComponent<GraphicRaycaster>());
            Destroy(card.GetComponent<Canvas>());

            //disable scripts not needed
            card.GetComponent<CardScript>().enabled = false;
            card.GetComponent<CardEvents>().enabled = false;

            //enable scripts needed
            card.GetComponent<CardListCardEvents>().enabled = true;
            card.GetComponent<Button>().enabled = true;
            card.GetComponent<CustomButton>().enabled = true;
            card.GetComponent<CustomButton>().playFeedbacks = false;

            card.GetComponent<CardListCardEvents>().markedGO = card.transform.Find("Panel").Find("UtilityFront").Find("Marked").gameObject;
            card.GetComponent<CardListCardEvents>().markedGO.SetActive(false);

            card.GetComponent<CardListCardEvents>().markedNumberGO = card.transform.Find("Panel").Find("UtilityFront").Find("MarkedNumber").gameObject;
            card.GetComponent<CardListCardEvents>().markedNumberGO.SetActive(false);

            card.GetComponent<CardListCardEvents>().scriptableCard = cardScriptData.scriptableCard;
            card.GetComponent<CardListCardEvents>().cardScriptData = cardScriptData;

        }



    }

    public IEnumerator StartUIEvent()
    {
        eventConversation = 0;
        eventButtonsShown = false;
        canExitEvent = false;

        //show the event
        eventGO.SetActive(true);

        GameObject buttonEventParent = eventGO.transform.Find("Buttons").gameObject;

        //destroy all previous event buttons
        yield return StartCoroutine(SystemManager.Instance.DestroyAllChildrenIE(buttonEventParent));

        yield return StartCoroutine(ShowNextConversation());

        yield return null;
    }

    public void ShowNextConversation_Click()
    {

        if (canExitEvent)
        {
            BackToAdventure();
        }

        if (eventButtonsShown)
        {
            return;
        }

        StartCoroutine(ShowNextConversation());
    }



    public IEnumerator ShowNextConversation()
    {


        if (eventConversation < CombatManager.Instance.ScriptableEvent.conversation.Count)
        {
            UIManager.Instance.AnimateTextTypeWriter(CombatManager.Instance.ScriptableEvent.conversation[eventConversation], eventGO.transform.Find("Conversation").Find("Text").GetComponent<TMP_Text>(), 100f);
            eventConversation++;
            eventGO.transform.Find("Conversation").Find("NextArrow").gameObject.SetActive(true);
        }
        else
        {
            eventGO.transform.Find("Conversation").Find("NextArrow").gameObject.SetActive(false);
            eventButtonsShown = true;

            //show buttons
            yield return StartCoroutine(ShowEventButtons());
        }

        yield return null;
    }

    public IEnumerator ShowEventButtons()
    {

        GameObject buttonEventParent = eventGO.transform.Find("Buttons").gameObject;

        //create new event buttons
        foreach (ScriptableButtonEvent scriptableButtonEvent in CombatManager.Instance.ScriptableEvent.scriptableButtonEventList)
        {

            GameObject eventButton = Instantiate(eventButtonPrefab, buttonEventParent.transform.position, Quaternion.identity);
            eventButton.transform.SetParent(buttonEventParent.transform);

            eventButton.GetComponent<UI_EventButton>().scriptableButtonEvent = scriptableButtonEvent;

            //assign the variables
            eventButton.GetComponent<ButtonManager>().SetText(scriptableButtonEvent.eventButtonDescription);

            //call the on button create of the scriptable object
            eventButton.GetComponent<UI_EventButton>().scriptableButtonEvent.OnButtonCreate(eventButton);

        }



        yield return null;
    }

    public void ShowEventGo(ScriptableEvent scriptableEvent)
    {
        //show the event
        eventGO.SetActive(true);

        //assign the variables
        eventGO.transform.Find("Image").GetComponent<Image>().sprite = scriptableEvent.icon;
        //eventGO.transform.Find("Description").GetComponent<TMP_Text>().text = scriptableEvent.description;
        eventGO.transform.Find("Title").GetComponent<TMP_Text>().text = scriptableEvent.title;

        GameObject buttonEventParent = eventGO.transform.Find("Buttons").gameObject;

        //destroy all previous event buttons
        SystemManager.Instance.DestroyAllChildren(buttonEventParent);



        //create new event buttons
        foreach (ScriptableButtonEvent scriptableButtonEvent in scriptableEvent.scriptableButtonEventList)
        {

            GameObject eventButton = Instantiate(eventButtonPrefab, buttonEventParent.transform.position, Quaternion.identity);
            eventButton.transform.SetParent(buttonEventParent.transform);

            eventButton.GetComponent<UI_EventButton>().scriptableButtonEvent = scriptableButtonEvent;

            //assign the variables
            eventButton.transform.Find("Text").GetComponent<TMP_Text>().text = scriptableButtonEvent.eventButtonDescription;

            //call the on button create of the scriptable object
            eventButton.GetComponent<UI_EventButton>().scriptableButtonEvent.OnButtonCreate(eventButton);

        }

    }

    public IEnumerator EndEvent(string finalWording)
    {
        GameObject buttonEventParent = eventGO.transform.Find("Buttons").gameObject;
        //destroy all previous event buttons
        SystemManager.Instance.DestroyAllChildren(buttonEventParent);

        //GameObject eventButtonClose = Instantiate(closeButtonPrefab, buttonEventParent.transform.position, Quaternion.identity);
        //eventButtonClose.transform.SetParent(buttonEventParent.transform);

        if (finalWording != null)
        {
            UIManager.Instance.AnimateTextTypeWriter(finalWording, eventGO.transform.Find("Conversation").Find("Text").GetComponent<TMP_Text>(), 100f);
        }

        yield return new WaitForSeconds(0.5f);


        canExitEvent = true;

        yield return null;

    }




    public void HideEventGo()
    {
        eventGO.SetActive(false);
    }

    public void BackToAdventure()
    {
        combatEndWindow.SetActive(false);

        eventGO.SetActive(false);

        SystemManager.Instance.LoadScene("scene_Adventure", 0f, true, false);
    }


    ////animations
    //public void AnimateTextTypeWriter(string textToAnimate, TMP_Text textObject)
    //{

    //    //change inventory text
    //    string textTMP = "";

    //    DOTween.To(() => textTMP, x => textTMP = x, textToAnimate, textToAnimate.Length / 30f)
    //        .OnUpdate(() =>
    //        {
    //            textObject.text = textTMP;
    //        });
    //}

    public void AnimateTextTypeWriter(string textToAnimate, TMP_Text textObject, float speed)
    {
        // Stop existing animation on this specific text object before starting a new one
        StopTypeWriter(textObject);

        string textTMP = "";
        Tweener tweener = null;

        tweener = DOTween.To(() => textTMP, x => textTMP = x, textToAnimate, textToAnimate.Length / speed)
            .OnUpdate(() =>
            {
                textObject.text = textTMP;
            })
            .OnComplete(() =>
            {
                if (activeTweeners.ContainsKey(textObject))
                {
                    activeTweeners.Remove(textObject); // Remove only this text's tween
                }
            });

        activeTweeners[textObject] = tweener; // Store tween for this specific text object
    }

    public void StopTypeWriter(TMP_Text textObject)
    {
        if (activeTweeners.ContainsKey(textObject))
        {
            activeTweeners[textObject].Kill();
            activeTweeners.Remove(textObject);
        }
    }

    public IEnumerator DelayedTypewriter(string text, TMP_Text textObject, float speed)
    {
        yield return null; // Wait 1 frame
        UIManager.Instance.AnimateTextTypeWriter(text, textObject, speed);
    }

    public void StopAllTypeWriters()
    {
        foreach (var tweener in activeTweeners.Values)
        {
            if (tweener.IsActive())
            {
                tweener.Kill();
            }
        }
        activeTweeners.Clear(); // Clear all tweens
    }

    public void CloseShop()
    {

        //save the cardlist
        GameObject cardList = UIManager.Instance.shopUI.transform.Find("CardList").gameObject;
        List<ShopData> shopDataList = new List<ShopData>();
        List<ShopCard> shopCardList = cardList.GetComponentsInChildren<ShopCard>(true).ToList();

        foreach (ShopCard shopCard in shopCardList)
        {
            shopDataList.Add(shopCard.shopData);
        }

        //save artifacts
        GameObject artifactList = UIManager.Instance.shopUI.transform.Find("ArtifactsList").gameObject;
        List<ShopData> shopDataListArtifact = new List<ShopData>();
        List<ShopArtifact> shopArtifactList = artifactList.GetComponentsInChildren<ShopArtifact>(true).ToList();

        foreach (ShopArtifact shopArtifact in shopArtifactList)
        {
            shopDataListArtifact.Add(shopArtifact.shopData);
        }

        //save items
        GameObject itemList = UIManager.Instance.shopUI.transform.Find("ItemList").gameObject;
        List<ShopData> shopDataListItem = new List<ShopData>();
        List<ShopItem> shopItemList = itemList.GetComponentsInChildren<ShopItem>(true).ToList();

        foreach (ShopItem shopItem in shopItemList)
        {
            shopDataListItem.Add(shopItem.shopData);
        }

        //save on planet
        CombatManager.Instance.planetClicked.GetComponent<RoomScript>().shopCards = shopDataList;
        CombatManager.Instance.planetClicked.GetComponent<RoomScript>().shopArtifacts = shopDataListArtifact;
        CombatManager.Instance.planetClicked.GetComponent<RoomScript>().shopItems = shopDataListItem;

        UIManager.Instance.shopUI.SetActive(false);


    }

    public void RefreshEveryShopItem()
    {
        // Cards
        GameObject parent = UIManager.Instance.shopUI.transform.Find("CardList").gameObject;
        foreach (Transform child in parent.transform)
        {
            GameObject card = child.gameObject;
            CardAvailability(card.GetComponent<ShopCard>().shopData, card);
        }

        // Artifacts
        parent = UIManager.Instance.shopUI.transform.Find("ArtifactsList").gameObject;
        foreach (Transform child in parent.transform)
        {
            GameObject artifact = child.gameObject;
            ArtifactAvailability(artifact.GetComponent<ShopArtifact>().shopData, artifact);
        }

        // Items
        parent = UIManager.Instance.shopUI.transform.Find("ItemList").gameObject;
        foreach (Transform child in parent.transform)
        {
            GameObject item = child.gameObject;
            ItemAvailability(item.GetComponent<ShopItem>().shopData, item);
        }
    }

    public void CardAvailability(ShopData shopData, GameObject card)
    {
        GameObject cardGoldPanel = card.transform.GetChild(0).Find("UtilityFront").Find("GoldPanel").gameObject;
        cardGoldPanel.SetActive(true);

        if (!shopData.itemAvailable)
        {
            card.transform.GetChild(0).Find("UtilityFront").Find("SoldOut").gameObject.SetActive(true);
            cardGoldPanel.transform.Find("Text").GetComponent<TMP_Text>().text = "SOLD";
        }
        else
        {

            int originalPrice = card.GetComponent<ShopCard>().shopData.shopCostItem;
            int newPrice = GetRealPrice(originalPrice);

            cardGoldPanel.transform.Find("Text").GetComponent<TMP_Text>().text = SetPrices(originalPrice, newPrice);

            if (ItemManager.Instance.CanPlayerBuy(newPrice))
            {
                cardGoldPanel.transform.Find("Text").GetComponent<TMP_Text>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorWhite);
            }
            else
            {
                cardGoldPanel.transform.Find("Text").GetComponent<TMP_Text>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorRed);
            }
        }
    }

    public int GetRatioPercentage()
    {
        int ratioPercentage = 0;

        ClassItemData discountCard = ItemManager.Instance.GetItemByScriptableName("Discount Card", StaticData.artifactItemList);
        if (discountCard != null)
        {
            ratioPercentage += discountCard.scriptableItem.intValue; 
        }

        ClassItemData premiumDiscountCard = ItemManager.Instance.GetItemByScriptableName("Premium Discount Card", StaticData.artifactItemList);
        if (premiumDiscountCard != null)
        {
            ratioPercentage += premiumDiscountCard.scriptableItem.intValue; 
        }

        return -1 * ratioPercentage;
    }

    public string SetPrices(int originalPrice, int newPrice)
    {
        if (originalPrice != newPrice)
        {
            // Show "200 → 180"
            return $"{newPrice}";
        }

        // No discount, just show the single price
        return originalPrice.ToString();
    }

    public int GetRealPrice(int originalPrice)
    {
        int newPrice = originalPrice;
        float ratio = 0f;

        ClassItemData discountCard = ItemManager.Instance.GetItemByScriptableName("Discount Card", StaticData.artifactItemList);
        if (discountCard != null)
        {
            ratio += discountCard.scriptableItem.intValue / 100f; // use float division
        }

        ClassItemData premiumDiscountCard = ItemManager.Instance.GetItemByScriptableName("Premium Discount Card", StaticData.artifactItemList);
        if (premiumDiscountCard != null)
        {
            ratio += premiumDiscountCard.scriptableItem.intValue / 100f; // use float division
        }

        if (ratio > 0f)
        {
            int discount = Mathf.RoundToInt(originalPrice * ratio); // calculate discount
            newPrice = Mathf.Max(0, originalPrice - discount);     // ensure not negative
        }

        return newPrice;
    }

    public void ArtifactAvailability(ShopData shopData, GameObject artifact)
    {
        GameObject cardGoldPanel = artifact.transform.GetChild(0).Find("UtilityFront").Find("GoldPanel").gameObject;
        cardGoldPanel.SetActive(true);

        ClassItemData itemExist = ItemManager.Instance.CheckIfItemExistOnList(StaticData.artifactItemList, shopData.scriptableItem);

        if (itemExist != null)
        {
            shopData.itemAvailable = false;
        }

        if (!shopData.itemAvailable)
        {
            artifact.transform.GetChild(0).Find("UtilityFront").Find("SoldOut").gameObject.SetActive(true);
            cardGoldPanel.transform.Find("Text").GetComponent<TMP_Text>().text = "SOLD";
        }
        else
        {
            int originalPrice = artifact.GetComponent<ShopArtifact>().shopData.shopCostItem;
            int newPrice = GetRealPrice(originalPrice);

            cardGoldPanel.transform.Find("Text").GetComponent<TMP_Text>().text = SetPrices(originalPrice, newPrice);

            if (ItemManager.Instance.CanPlayerBuy(newPrice))
            {
                cardGoldPanel.transform.Find("Text").GetComponent<TMP_Text>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorWhite);
            }
            else
            {
                cardGoldPanel.transform.Find("Text").GetComponent<TMP_Text>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorRed);
            }
        }
    }


    public void ItemAvailability(ShopData shopData, GameObject item)
    {

        GameObject cardGoldPanel = item.transform.Find("GoldPanel").gameObject;
        cardGoldPanel.SetActive(true);

        if (!shopData.itemAvailable)
        {
            item.transform.Find("SoldOut").gameObject.SetActive(true);
            cardGoldPanel.transform.Find("Text").GetComponent<TMP_Text>().text = "SOLD";
        }
        else
        {
            int originalPrice = item.GetComponent<ShopItem>().shopData.shopCostItem;
            int newPrice = GetRealPrice(originalPrice);

            cardGoldPanel.transform.Find("Text").GetComponent<TMP_Text>().text = SetPrices(originalPrice, newPrice);

            if (ItemManager.Instance.CanPlayerBuy(newPrice))
            {
                cardGoldPanel.transform.Find("Text").GetComponent<TMP_Text>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorWhite);
            }
            else
            {
                cardGoldPanel.transform.Find("Text").GetComponent<TMP_Text>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorRed);
            }
        }
    }


}
