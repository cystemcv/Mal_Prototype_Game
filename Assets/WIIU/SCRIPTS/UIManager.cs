using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    //main gameobjects of the UI
    public List<GameObject> list_goMenuItem;

    //modal stuff
    [SerializeField] private ModalWindowManager modalWindowManager;
    [SerializeField] private Sprite modalIcon;

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

    // Start is called before the first frame update
    void Start()
    {

        Debug.Log("test : " + PlayerPrefs.GetInt("user_save_settings"));

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

    private void NavigateMenu(string menuName)
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


        Debug.Log("DEFAULT : " + AudioTabController.Instance.musicSlider.mainSlider.value);

        //AUDIO TAB
        AudioTabController.Instance.musicSlider.mainSlider.value = 0.5f;
        AudioManager.Instance.MusicVolume(0.5f);
        AudioTabController.Instance.sfxSlider.mainSlider.value = 0.5f;
        AudioManager.Instance.SFXVolume(0.5f);
    }

    public void SaveSettings()
    {

        //save all the user settings
        PlayerPrefs.SetInt("user_save_settings", 1);

        Debug.Log("SAVE : " + AudioTabController.Instance.musicSlider.mainSlider.value);

        //AUDIO TAB
        AudioTabController.Instance.musicSlider.mainSlider.value = AudioTabController.Instance.musicSlider.mainSlider.value;
        PlayerPrefs.SetFloat("audio_music_volume", AudioTabController.Instance.musicSlider.mainSlider.value);
        AudioTabController.Instance.sfxSlider.mainSlider.value = AudioTabController.Instance.sfxSlider.mainSlider.value;
        PlayerPrefs.SetFloat("audio_sfx_volume", AudioTabController.Instance.sfxSlider.mainSlider.value);

   
    }

    public void LoadSettings()
    {
        //load all the user settings

        //AUDIO TAB
        AudioTabController.Instance.musicSlider.mainSlider.value = PlayerPrefs.GetFloat("audio_music_volume");
        AudioManager.Instance.MusicVolume(PlayerPrefs.GetFloat("audio_music_volume"));
        AudioTabController.Instance.sfxSlider.mainSlider.value = PlayerPrefs.GetFloat("audio_sfx_volume");
        AudioManager.Instance.SFXVolume(PlayerPrefs.GetFloat("audio_sfx_volume"));
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

}
