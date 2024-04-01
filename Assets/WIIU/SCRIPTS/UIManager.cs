using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    //main gameobjects of the UI
    public List<GameObject> list_goMenuItem;

    //main menu buttons
    public GameObject btnNewGame;
    public GameObject btnContinue;
    public GameObject btnLoad;
    public GameObject btnOptions;
    public GameObject btnClose;
    public GameObject btnMainMenu;
    public GameObject btnExit;

    //maintext
    public GameObject txtMainMenu;

    //background
    public Image uiAnimatedBg;


    //modal stuff
    [SerializeField] private ModalWindowManager modalWindowManager;
    [SerializeField] private Sprite modalIcon;

    public enum UIModes
    {
        DISABLED,
        MAINMENU,
        GAMEPLAY
    }

    public UIModes uiCurrentMode = UIModes.MAINMENU;

    public delegate void OnCurrentModeChange();
    public event OnCurrentModeChange onCurrentModeChange;

    //this variable has event so we use getter and setter
    public UIModes UICurrentMode
    {
        get { return uiCurrentMode; }
        set
        {
            if (uiCurrentMode == value) return;
            uiCurrentMode = value;
            if (onCurrentModeChange != null)
                onCurrentModeChange();
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
    }

    private void OnDisable()
    {
        onCurrentModeChange -= UIModeChangeDetectEvent;
    }

    // Start is called before the first frame update
    void Start()
    {


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
        
    }

    public void UIModeChangeDetectEvent()
    {

        if (UICurrentMode == UIModes.MAINMENU)
        {
            InitializeMainMenu();
        }
        else if (UICurrentMode == UIModes.GAMEPLAY)
        {
            btnNewGame.SetActive(false);
            btnContinue.SetActive(false);
            btnLoad.SetActive(true);
            btnOptions.SetActive(true);
            btnClose.SetActive(true);
            btnMainMenu.SetActive(true);
            btnExit.SetActive(true);

            uiAnimatedBg.color = new Color32(90,10,0,255);
            txtMainMenu.SetActive(false);
        }
        else
        {
            InitializeMainMenu();
        }

    }

    public void InitializeMainMenu()
    {
        btnNewGame.SetActive(true);
        btnContinue.SetActive(true);
        btnLoad.SetActive(true);
        btnOptions.SetActive(true);
        btnClose.SetActive(false);
        btnMainMenu.SetActive(false);
        btnExit.SetActive(true);

        uiAnimatedBg.color = new Color32(0, 95, 166, 255);
        txtMainMenu.SetActive(true);
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

        SaveLoad.Instance.CurrentMode = SaveLoad.saveLoadMode.SAVE;

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
            Debug.Log("changed");
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
            Debug.Log("No user save settings found but tried to load");
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
        UIManager.Instance.UICurrentMode = UIManager.UIModes.GAMEPLAY;

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
        UIManager.Instance.UICurrentMode = UIManager.UIModes.MAINMENU;

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

}
