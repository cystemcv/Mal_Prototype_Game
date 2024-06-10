using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public enum UIScreens { MainMenu, ModeSelectionMenu, CharacterSelectionMenu, SaveMenu, LoadMenu, LibraryMenu, OptionsMenu }

    public UIScreens currentUIScreen = UIScreens.MainMenu;


    public Image logo;
    public TMP_Text headerText;
    public TMP_Text footerText;
    public GameObject bgCanvas;
    public GameObject uiParticles;

    //main ui components
    public GameObject UIMENU;
    public GameObject UICOMBAT;

    [Header("UIMENU")]
    //main gameobjects of the UI
    public List<GameObject> list_goMenuItem;




    //UI background
    public Image solidColorBackground;



    //maintext
    public GameObject txtMainMenu;

    //background
    public Image uiAnimatedBg;

    //player time text
    public TMP_Text playerTimeText;


    [Header("UICOMBAT")]
    public GameObject handObjectParent;
    public GameObject deckUIObject;
    public GameObject deckText;
    public GameObject discardUISpawn;
    public GameObject discardText;
    public GameObject banishedText;
    public GameObject manaText;
    public GameObject chooseACardScreen;


    [Header("SETTINGS")]
    //modal stuff
    [SerializeField] private ModalWindowManager modalWindowManager;
    [SerializeField] private Sprite modalIcon;

    [Header("NOTIFICATIONS")]
    public GameObject notificationParent;
    public GameObject notificationPb;

    [Header("CHARACTERS")]
    public GameObject characterSelectionContent;
    public GameObject characterPanelUIPrefab;
    public GameObject proceedToGame;

    public delegate void OnCurrentModeChange();
    public event OnCurrentModeChange onCurrentModeChange;

    //this variable has event so we use getter and setter
    public SystemManager.SystemModes UICurrentMode
    {
        get { return SystemManager.Instance.currentSystemMode; }
        set
        {
            if (SystemManager.Instance.currentSystemMode == value) return;
            SystemManager.Instance.currentSystemMode = value;
            if (onCurrentModeChange != null)
                onCurrentModeChange();
        }
    }


    public delegate void OnCurrentUIScreenChange();
    public event OnCurrentUIScreenChange onCurrentUIScreenChange;

    //this variable has event so we use getter and setter
    public UIScreens CurrentUIScreen
    {
        get { return currentUIScreen; }
        set
        {
            if (currentUIScreen == value) return;
            currentUIScreen = value;
            if (onCurrentUIScreenChange != null)
                onCurrentUIScreenChange();
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

        InitializeMainMenu();
    }

    //subscribe events
    private void OnEnable()
    {
        onCurrentModeChange += UIModeChangeDetectEvent;
        onCurrentUIScreenChange += UIScreenChangeDetectEvent;
    }

    private void OnDisable()
    {
        onCurrentModeChange -= UIModeChangeDetectEvent;
        onCurrentUIScreenChange -= UIScreenChangeDetectEvent;
    }

    // Start is called before the first frame update
    void Start()
    {

        CurrentUIScreen = UIScreens.MainMenu;

        //check if there is a saved user setting
        if (PlayerPrefs.GetInt("user_save_settings") == 1)
        {
            //if true then load
            LoadSettings();
        }
        else
        {
            //else initialize
            DefaultSettings();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (SystemManager.Instance.currentSystemMode == SystemManager.SystemModes.GAMEPLAY)
        {
            playerTimeText.text = SystemManager.Instance.ConvertTimeToReadable(SystemManager.Instance.totalTimePlayed);
        }
        else if (SystemManager.Instance.currentSystemMode == SystemManager.SystemModes.COMBAT)
        {
            playerTimeText.text = SystemManager.Instance.ConvertTimeToReadable(SystemManager.Instance.totalTimePlayed);
            deckText.transform.GetChild(1).GetComponent<TMP_Text>().text = DeckManager.Instance.combatDeck.Count.ToString();
            discardText.transform.GetChild(1).GetComponent<TMP_Text>().text = DeckManager.Instance.discardedPile.Count.ToString();
            banishedText.transform.GetChild(0).GetComponent<TMP_Text>().text = DeckManager.Instance.banishedPile.Count.ToString();
        }
        else
        {
            playerTimeText.text = "";
        }
    }

    public void UIModeChangeDetectEvent()
    {

        if (UICurrentMode == SystemManager.SystemModes.MAINMENU)
        {
            InitializeMainMenu();
        }
        else if (UICurrentMode == SystemManager.SystemModes.GAMEPLAY)
        {
            InitializeMainMenuGameplay();
        }
        else if (UICurrentMode == SystemManager.SystemModes.COMBAT)
        {
            InitializeMainMenuGameplay();

        }
        else
        {
            InitializeMainMenu();
        }

    }

    public void UIScreenChangeDetectEvent()
    {

        Debug.Log("test");

    }

    public void InitializeMainMenu()
    {
        //btnNewGame.SetActive(true);
        //btnContinue.SetActive(true);
        //btnSave.SetActive(false);
        //btnLoad.SetActive(true);
        //btnOptions.SetActive(true);
        //btnClose.SetActive(false);
        //btnMainMenu.SetActive(false);
        //btnExit.SetActive(true);

        //uiAnimatedBg.color = new Color32(0, 95, 166, 255);
        //txtMainMenu.SetActive(true);

        //solidColorBackground.color = new Color32(68,53,135,255);
    }

    public void InitializeMainMenuGameplay()
    {
        //btnNewGame.SetActive(false);
        //btnContinue.SetActive(false);
        //btnSave.SetActive(true);
        //btnLoad.SetActive(true);
        //btnOptions.SetActive(true);
        //btnClose.SetActive(true);
        //btnMainMenu.SetActive(true);
        //btnExit.SetActive(true);

        //uiAnimatedBg.color = new Color32(90,10,0,255);
        //txtMainMenu.SetActive(false);

        //solidColorBackground.color = new Color32(68, 53, 135, 0);
    }


    public void MakeAllMenuInactive()
    {
        foreach (GameObject goMenuItem in list_goMenuItem)
        {
            goMenuItem.SetActive(false);
        }


    }

    public void MainMenuNext(string menuName)
    {

        //play audio
        AudioManager.Instance.PlaySfx("UI_goNext");

        //UISaveLoad.Instance.CurrentMode = UISaveLoad.saveLoadMode.SAVE;

        //open the correct menu
        NavigateMenu(menuName);
    }

    public void MainMenuBack(string menuName)
    {
        //play audio
        AudioManager.Instance.PlaySfx("UI_goBack");

        //open the correct menu
        NavigateMenu(menuName);
    }

    public void GoToGameModeSelection(string mode)
    {
        //play audio
        //play audio
        if (mode == "BACK")
        {
            AudioManager.Instance.PlaySfx("UI_goBack");
        }
        else
        {
            AudioManager.Instance.PlaySfx("UI_goNext");
        }


        //change visibity
        logo.gameObject.SetActive(false);
        footerText.gameObject.SetActive(false);


        //change header text
        headerText.gameObject.SetActive(true);
        headerText.text = "CHOOSE A MODE!";

        //disable the 3rd option for now
        DisableButton(UIMENU.transform.Find("HOLDER").Find("GAME MODE MENU").Find("ListOfGameModes").Find("Panel").Find("btn_UNKOWN").gameObject);

        //open the correct menu
        NavigateMenu("GAME MODE MENU");
    }

    public void GoToCharacterSelection_MainMode(string mode)
    {
        //play audio
        //play audio
        if (mode == "BACK")
        {
            AudioManager.Instance.PlaySfx("UI_goBack");
        }
        else
        {
            AudioManager.Instance.PlaySfx("UI_goNext");
        }


        //change visibity
        logo.gameObject.SetActive(false);
        footerText.gameObject.SetActive(false);


        //change header text
        headerText.gameObject.SetActive(true);
        headerText.text = "CHOOSE A CHARACTER!";

        //change the mode
        CharacterManager.Instance.gameMode = CharacterManager.GameMode.MainMode;

        //clear the list
        CharacterManager.Instance.scriptablePlayerList.Clear();

        //disable the proceed button
        UIManager.Instance.DisableButton(UIManager.Instance.proceedToGame);

        //display all characters
        DisplayAllCharacters();

        //open the correct menu
        NavigateMenu("CHARACTER SELECTION MENU");
    }

    public void GoToCharacterSelection_DuoMode(string mode)
    {
        //play audio
        //play audio
        if (mode == "BACK")
        {
            AudioManager.Instance.PlaySfx("UI_goBack");
        }
        else
        {
            AudioManager.Instance.PlaySfx("UI_goNext");
        }


        //change visibity
        logo.gameObject.SetActive(false);
        footerText.gameObject.SetActive(false);


        //change header text
        headerText.gameObject.SetActive(true);
        headerText.text = "CHOOSE 2 CHARACTERS!";

        //change the mode
        CharacterManager.Instance.gameMode = CharacterManager.GameMode.DuoMode;

        //clear the list
        CharacterManager.Instance.scriptablePlayerList.Clear();

        //disable the proceed button
        UIManager.Instance.DisableButton(UIManager.Instance.proceedToGame);

        //display all characters
        DisplayAllCharacters();

        //open the correct menu
        NavigateMenu("CHARACTER SELECTION MENU");
    }

    public void GoToMainMenu(string mode)
    {
        //play audio
        if (mode == "BACK")
        {
            AudioManager.Instance.PlaySfx("UI_goBack");
        }
        else
        {
            AudioManager.Instance.PlaySfx("UI_goNext");
        }
  

        //change visibity
        logo.gameObject.SetActive(true);
        footerText.gameObject.SetActive(true);


        //change header text
        headerText.gameObject.SetActive(false);
        headerText.text = "";

        //open the correct menu
        NavigateMenu("MAIN MENU");
    }

    public void NavigateMenu(string menuName)
    {

        //make everything inactive
        MakeAllMenuInactive();

        //make the menu we need active
        GameObject goMenuItem = list_goMenuItem.Find(item => item.name == menuName);
        goMenuItem.SetActive(true);
    }

    public void DefaultSettings()
    {

        //all the initial default settings here
        PlayerPrefs.SetInt("user_save_settings", 0);

        //AUDIO TAB
        AudioTabController.Instance.musicSlider.mainSlider.value = 0.5f;
        AudioManager.Instance.MusicVolume(0.5f);
        AudioTabController.Instance.sfxSlider.mainSlider.value = 0.5f;
        AudioManager.Instance.SFXVolume(0.5f);

        //DISPLAY TAB
        DisplayTabController.Instance.selectScreenMode.ChangeDropdownInfo(0); ///fullscreen
        DisplayTabController.Instance.SelectScreenMode(false);
        DisplayTabController.Instance.selectScreenResolution.ChangeDropdownInfo(1); //1920x1080
        DisplayTabController.Instance.SelectScreenResolution(false);
    }

    public void SaveSettings()
    {



        //save all the user settings
        PlayerPrefs.SetInt("user_save_settings", 1);

        //GRAPHICS TAB
        if (GraphicsTabController.Instance.changeQualitySettings == true)
        {

            GraphicsTabController.Instance.checkQualitySettingIndexNow = int.Parse(GraphicsTabController.Instance.selectQualityOption.selectedText.text.Split(":")[0].Trim());

            //then change the graphics
            QualitySettings.SetQualityLevel(GraphicsTabController.Instance.checkQualitySettingIndexNow, true);
            PlayerPrefs.SetInt("graphics_quality_option", GraphicsTabController.Instance.checkQualitySettingIndexNow);
        }

        //AUDIO TAB
        PlayerPrefs.SetFloat("audio_music_volume", AudioTabController.Instance.musicSlider.mainSlider.value);
        PlayerPrefs.SetFloat("audio_sfx_volume", AudioTabController.Instance.sfxSlider.mainSlider.value);

        //DISPLAY TAB
        PlayerPrefs.SetInt("display_sreen_mode", DisplayTabController.Instance.selectScreenMode.index);
        PlayerPrefs.SetInt("display_sreen_resolution", DisplayTabController.Instance.selectScreenResolution.index);


    }

    public void LoadSettings()
    {
        //load all the user settings
        if (PlayerPrefs.GetInt("user_save_settings") == 0)
        {
            DefaultSettings();

            return;
        }

        //GRAPHICS TAB
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("graphics_quality_option"), true);
        GraphicsTabController.Instance.selectQualityOption.ChangeDropdownInfo(GraphicsTabController.Instance.selectQualityOption.items.FindIndex(item => item.itemName.Split(":")[0].Trim() == PlayerPrefs.GetInt("graphics_quality_option").ToString()));

        //AUDIO TAB
        AudioTabController.Instance.musicSlider.mainSlider.value = PlayerPrefs.GetFloat("audio_music_volume");
        AudioManager.Instance.MusicVolume(PlayerPrefs.GetFloat("audio_music_volume"));
        AudioTabController.Instance.sfxSlider.mainSlider.value = PlayerPrefs.GetFloat("audio_sfx_volume");
        AudioManager.Instance.SFXVolume(PlayerPrefs.GetFloat("audio_sfx_volume"));

        //DISPLAY TAB
        DisplayTabController.Instance.selectScreenMode.ChangeDropdownInfo(PlayerPrefs.GetInt("display_sreen_mode")); ///fullscreen
        DisplayTabController.Instance.SelectScreenMode(false);
        DisplayTabController.Instance.selectScreenResolution.ChangeDropdownInfo(PlayerPrefs.GetInt("display_sreen_resolution")); //1920x1080
        DisplayTabController.Instance.SelectScreenResolution(false);

    }



    //open
    public void OpenModal(string title, string desc, UnityEngine.Events.UnityAction function)
    {
        //remove all the listeners so we dont have stack of listeners
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
        modalWindowManager.onConfirm.AddListener(function); // Add confirm events
        modalWindowManager.onCancel.AddListener(CloseModal); // Add cancel events

        //move the modal in the correct position (this is to keep it open and do not interfene with scene)
        modalWindowManager.gameObject.transform.parent.localPosition = new Vector3(0, 0, 0);
    }

    public void CloseModal()
    {
        AudioManager.Instance.PlaySfx("UI_Cancel");
        modalWindowManager.Close(); // Close window
    }


    public void ModalOpenRestoreSettings()
    {
        OpenModal("RESTORE", "Are you sure you want to restore settings to the default state?", RestoreUISettings);

    }

    public void RestoreUISettings()
    {
        AudioManager.Instance.PlaySfx("UI_Confirm");
        modalWindowManager.Close(); // Close window
        DefaultSettings();
        MainMenuBack("MAIN MENU");
    }

    public void ModalOpenSaveSettings()
    {
        OpenModal("SAVE", "Are you sure you want to save settings?", SaveUISettings);
    }

    public void SaveUISettings()
    {
        AudioManager.Instance.PlaySfx("UI_Confirm");
        modalWindowManager.Close(); // Close window
        SaveSettings();
        MainMenuBack("MAIN MENU");
    }

    public void GoToScene(string sceneName)
    {
        AudioManager.Instance.PlaySfx("UI_Confirm");
        AudioManager.Instance.PlayMusic("Gameplay_1");

        //change UI to gameplay
        UIManager.Instance.UICurrentMode = SystemManager.SystemModes.GAMEPLAY;

        //make UI inactive
        SystemManager.Instance.EnableDisable_UIManager(false);

        //load scene
        SceneManager.LoadScene(sceneName);


    }

    public void GoToMainMenu()
    {
        OpenModal("BACK TO MAIN MENU", "Are you sure you want to go back to Main Menu? (Any unsaved changes will be lost!)", GoToMainMenuConfirm);
    }

    public void GoToMainMenuConfirm()
    {
        AudioManager.Instance.PlaySfx("UI_Confirm");
        AudioManager.Instance.PlayMusic("UI_MainMenu");
        modalWindowManager.Close(); // Close window

        //change UI to gameplay
        UIManager.Instance.UICurrentMode = SystemManager.SystemModes.MAINMENU;

        //make UI inactive
        SystemManager.Instance.EnableDisable_UIManager(true);

        //load scene
        SceneManager.LoadScene("scene_MainMenu");
    }

    public void CloseUIWindow()
    {
        AudioManager.Instance.PlaySfx("UI_goBack");

        SystemManager.Instance.EnableDisable_UIManager(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void UIOpenSaveMenu()
    {

        //play audio
        AudioManager.Instance.PlaySfx("UI_goNext");

        UISaveLoad.Instance.CurrentMode = UISaveLoad.saveLoadMode.SAVE;

        //open the correct menu
        NavigateMenu("SAVELOAD MENU");
    }

    public void UIOpenLoadMenu()
    {

        //play audio
        AudioManager.Instance.PlaySfx("UI_goNext");

        UISaveLoad.Instance.CurrentMode = UISaveLoad.saveLoadMode.LOAD;

        //open the correct menu
        NavigateMenu("SAVELOAD MENU");
    }

    //notifications
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

    public void DisplayAllCharacters()
    {
        //destroy all children objects
        SystemManager.Instance.DestroyAllChildren(characterSelectionContent);

        foreach (ScriptablePlayer scriptablePlayer in CharacterManager.Instance.characterList)
        {

            //generate each character
            GameObject characterPanel = Instantiate(characterPanelUIPrefab, characterSelectionContent.transform.position, Quaternion.identity);
            characterPanel.transform.GetChild(0).GetComponent<CharacterCard>().scriptablePlayer = scriptablePlayer;
 
            //set it as a child of the parent
            characterPanel.transform.SetParent(characterSelectionContent.transform);

            //make the local scale 1,1,1
            characterPanel.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void GoToMainMode()
    {
        //change the mode
        CharacterManager.Instance.gameMode = CharacterManager.GameMode.MainMode;

        //display all characters
        DisplayAllCharacters();

        //navigate to the menu
        MainMenuNext("CHARACTER SELECTION MENU");

    }

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

            if(childText != null)
            {
                Color32 originalColor = childText.color;

                //change the color to transparetn
                childText.color = new Color32(originalColor.r,originalColor.g, originalColor.b, 30);
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

    public void ProceedToGame()
    {

        //destroy any previous characters
        SystemManager.Instance.DestroyAllChildren(CombatManager.Instance.combatScene.transform.Find("Characters").gameObject);

        int positionSpawn = 0;
        //generate the selected characters that will be used throught the game
        foreach (ScriptablePlayer scriptablePlayer in CharacterManager.Instance.scriptablePlayerList)
        {
            //instantiate our character or characters
            CombatManager.Instance.InstantiateCharacter(scriptablePlayer, positionSpawn);

            //increase to the next position
            positionSpawn++;
        }


        //build the deck
        DeckManager.Instance.BuildStartingDeck();

        //then start combat
        CombatManager.Instance.StartCombat();

    }

    

}
