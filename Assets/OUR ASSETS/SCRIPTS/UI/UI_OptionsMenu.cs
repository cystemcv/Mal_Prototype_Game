using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UI_OptionsMenu : MonoBehaviour
{
    public static UI_OptionsMenu Instance;

    public List<GameObject> list_goMenuItem;

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


    void Start()
    {
        //UIManager.Instance.topPanelCombat.SetActive(false);
        LoadSettings();
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

        TutorialManager.Instance.allowTutorials = true;
        MiscTabController.Instance.enableTutoralSwitch.isOn = TutorialManager.Instance.allowTutorials; // Changing state manually
        MiscTabController.Instance.enableTutoralSwitch.UpdateUI(); // Updating UI manually

        SystemManager.Instance.options_allow_lens_distortion = true;
        MiscTabController.Instance.enableLensDistortion.isOn = SystemManager.Instance.options_allow_lens_distortion; // Changing state manually
        MiscTabController.Instance.enableLensDistortion.UpdateUI(); // Updating UI manually

        SystemManager.Instance.options_allow_chromatic_aberration = true;
        MiscTabController.Instance.enableChromaticAberration.isOn = SystemManager.Instance.options_allow_chromatic_aberration; // Changing state manually
        MiscTabController.Instance.enableChromaticAberration.UpdateUI(); // Updating UI manually
    }




    public void SaveSettings()
    {



        //save all the user settings
        PlayerPrefs.SetInt("user_save_settings", 1);

        //GRAPHICS TAB
        if (GraphicsTabController.Instance.changeQualitySettings == true)
        {

            //then change the graphics
            SystemManager.Instance.SetDropdownByName(GraphicsTabController.Instance.selectQualityOption, GraphicsTabController.Instance.selectQualityOption.selectedText.text);
            PlayerPrefs.SetString("graphics_quality_option", GraphicsTabController.Instance.selectQualityOption.selectedText.text);
        }

        //AUDIO TAB
        PlayerPrefs.SetFloat("audio_music_volume", AudioTabController.Instance.musicSlider.mainSlider.value);
        PlayerPrefs.SetFloat("audio_sfx_volume", AudioTabController.Instance.sfxSlider.mainSlider.value);

        //DISPLAY TAB
        PlayerPrefs.SetString("display_sreen_mode", DisplayTabController.Instance.selectScreenMode.selectedText.text);
        PlayerPrefs.SetString("display_sreen_resolution", DisplayTabController.Instance.selectScreenResolution.selectedText.text);

        PlayerPrefs.SetString("allow_tutorials", TutorialManager.Instance.allowTutorials.ToString());
        PlayerPrefs.SetString("options_allow_lens_distortion", SystemManager.Instance.options_allow_lens_distortion.ToString());
        PlayerPrefs.SetString("options_allow_chromatic_aberration", SystemManager.Instance.options_allow_chromatic_aberration.ToString());
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

        SystemManager.Instance.SetDropdownByName(GraphicsTabController.Instance.selectQualityOption, PlayerPrefs.GetString("graphics_quality_option").ToString());

        //AUDIO TAB
        AudioTabController.Instance.musicSlider.mainSlider.value = PlayerPrefs.GetFloat("audio_music_volume");
        AudioManager.Instance.MusicVolume(PlayerPrefs.GetFloat("audio_music_volume"));
        AudioTabController.Instance.sfxSlider.mainSlider.value = PlayerPrefs.GetFloat("audio_sfx_volume");
        AudioManager.Instance.SFXVolume(PlayerPrefs.GetFloat("audio_sfx_volume"));

        //DISPLAY TAB
        SystemManager.Instance.SetDropdownByName(DisplayTabController.Instance.selectScreenMode, PlayerPrefs.GetString("display_sreen_mode").ToString());
        DisplayTabController.Instance.SelectScreenMode(false);
        SystemManager.Instance.SetDropdownByName(DisplayTabController.Instance.selectScreenResolution, PlayerPrefs.GetString("display_sreen_resolution").ToString());
        DisplayTabController.Instance.SelectScreenResolution(false);

        string storedValue = PlayerPrefs.GetString("allow_tutorials", "true");
        bool allowTutorials;
        bool.TryParse(storedValue, out allowTutorials);
        TutorialManager.Instance.allowTutorials = allowTutorials;
        MiscTabController.Instance.enableTutoralSwitch.isOn = TutorialManager.Instance.allowTutorials; // Changing state manually
        MiscTabController.Instance.enableTutoralSwitch.UpdateUI(); // Updating UI manually


        //lens distortion
        string storedValue_LensDistortion = PlayerPrefs.GetString("options_allow_lens_distortion", "true");
        bool allowLensDistortion;
        bool.TryParse(storedValue_LensDistortion, out allowLensDistortion);
        SystemManager.Instance.options_allow_lens_distortion = allowLensDistortion;
        MiscTabController.Instance.enableLensDistortion.isOn = allowLensDistortion; // Changing state manually
        MiscTabController.Instance.enableLensDistortion.UpdateUI(); // Updating UI manually

        //chromatic abberation
        string storedValue_CA = PlayerPrefs.GetString("options_allow_chromatic_aberration", "true");
        bool options_allow_chromatic_aberration;
        bool.TryParse(storedValue_LensDistortion, out options_allow_chromatic_aberration);
        SystemManager.Instance.options_allow_chromatic_aberration = options_allow_chromatic_aberration;
        MiscTabController.Instance.enableChromaticAberration.isOn = options_allow_chromatic_aberration; // Changing state manually
        MiscTabController.Instance.enableChromaticAberration.UpdateUI(); // Updating UI manually


        Debug.Log(PlayerPrefs.GetString("display_sreen_mode").ToString());
    }




    public void ModalOpenRestoreSettings()
    {
        UIManager.Instance.OpenModal("RESTORE", "Are you sure you want to restore settings to the default state?", UI_OptionsMenu.Instance.RestoreUISettings);

    }

    public void RestoreUISettings()
    {
        AudioManager.Instance.PlaySfx("UI_Confirm");
        //modalWindowManager.Close(); // Close window
        DefaultSettings();
        //MainMenuBack("MAIN MENU");
    }

    public void ModalOpenSaveSettings()
    {
        UIManager.Instance.OpenModal("SAVE", "Are you sure you want to save settings?", UI_OptionsMenu.Instance.SaveUISettings);
    }

    public void SaveUISettings()
    {

        SaveSettings();
        // MainMenuBack("MAIN MENU");
    }

    public void BackToMainMenu()
    {
        //play audio
        AudioManager.Instance.PlaySfx("UI_goBack");

        if (UIManager.Instance.quickMenuOpen == true)
        {
            UIManager.Instance.DisableAllUIScenes();
            UIManager.Instance.OpenQuickMenu();
            UIManager.Instance.topPanelCombat.SetActive(true);
        }
        else
        {
            //open the correct menu
            UIManager.Instance.DisableAllUIScenes();
            UIManager.Instance.scenes_BG.SetActive(true);
            UIManager.Instance.scene_mainMenu.SetActive(true);
        }

    }





}
