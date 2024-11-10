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

    public GameObject tooltipPanel;
    public TMP_Text tooltipText;  // Use Text instead of TextMeshPro if preferred
                                  // Padding to prevent tooltip from overlapping hovered element
    public float offsetPadding = 10f;

    private RectTransform panelRectTransform;

    //inventory
    public GameObject inventoryGO;
    public GameObject companionGO;
    public GameObject artifactGO;
    public GameObject lootGO;

    public GameObject ChooseGroupUI;


    [Header("SETTINGS")]
    //modal stuff
    [SerializeField] private ModalWindowManager modalWindowManager;
    [SerializeField] private Sprite modalIcon;

    [Header("NOTIFICATIONS")]
    public GameObject notificationParent;
    public GameObject notificationPb;



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

        UIManager.Instance.AdjustTooltipPosition( hoverPosition, hoverSize, FindParentCanvas(hoverObject.transform));
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


    //public void DefaultSettings()
    //{

    //    //all the initial default settings here
    //    PlayerPrefs.SetInt("user_save_settings", 0);

    //    //AUDIO TAB
    //    AudioTabController.Instance.musicSlider.mainSlider.value = 0.5f;
    //    AudioManager.Instance.MusicVolume(0.5f);
    //    AudioTabController.Instance.sfxSlider.mainSlider.value = 0.5f;
    //    AudioManager.Instance.SFXVolume(0.5f);

    //    //DISPLAY TAB
    //    DisplayTabController.Instance.selectScreenMode.ChangeDropdownInfo(0); ///fullscreen
    //    DisplayTabController.Instance.SelectScreenMode(false);
    //    DisplayTabController.Instance.selectScreenResolution.ChangeDropdownInfo(1); //1920x1080
    //    DisplayTabController.Instance.SelectScreenResolution(false);
    //}

    //public void SaveSettings()
    //{



    //    //save all the user settings
    //    PlayerPrefs.SetInt("user_save_settings", 1);

    //    //GRAPHICS TAB
    //    if (GraphicsTabController.Instance.changeQualitySettings == true)
    //    {

    //        GraphicsTabController.Instance.checkQualitySettingIndexNow = int.Parse(GraphicsTabController.Instance.selectQualityOption.selectedText.text.Split(":")[0].Trim());

    //        //then change the graphics
    //        QualitySettings.SetQualityLevel(GraphicsTabController.Instance.checkQualitySettingIndexNow, true);
    //        PlayerPrefs.SetInt("graphics_quality_option", GraphicsTabController.Instance.checkQualitySettingIndexNow);
    //    }

    //    //AUDIO TAB
    //    PlayerPrefs.SetFloat("audio_music_volume", AudioTabController.Instance.musicSlider.mainSlider.value);
    //    PlayerPrefs.SetFloat("audio_sfx_volume", AudioTabController.Instance.sfxSlider.mainSlider.value);

    //    //DISPLAY TAB
    //    PlayerPrefs.SetInt("display_sreen_mode", DisplayTabController.Instance.selectScreenMode.index);
    //    PlayerPrefs.SetInt("display_sreen_resolution", DisplayTabController.Instance.selectScreenResolution.index);


    //}

    //public void LoadSettings()
    //{
    //    //load all the user settings
    //    if (PlayerPrefs.GetInt("user_save_settings") == 0)
    //    {
    //        DefaultSettings();

    //        return;
    //    }

    //    //GRAPHICS TAB
    //    QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("graphics_quality_option"), true);
    //    GraphicsTabController.Instance.selectQualityOption.ChangeDropdownInfo(GraphicsTabController.Instance.selectQualityOption.items.FindIndex(item => item.itemName.Split(":")[0].Trim() == PlayerPrefs.GetInt("graphics_quality_option").ToString()));

    //    //AUDIO TAB
    //    AudioTabController.Instance.musicSlider.mainSlider.value = PlayerPrefs.GetFloat("audio_music_volume");
    //    AudioManager.Instance.MusicVolume(PlayerPrefs.GetFloat("audio_music_volume"));
    //    AudioTabController.Instance.sfxSlider.mainSlider.value = PlayerPrefs.GetFloat("audio_sfx_volume");
    //    AudioManager.Instance.SFXVolume(PlayerPrefs.GetFloat("audio_sfx_volume"));

    //    //DISPLAY TAB
    //    DisplayTabController.Instance.selectScreenMode.ChangeDropdownInfo(PlayerPrefs.GetInt("display_sreen_mode")); ///fullscreen
    //    DisplayTabController.Instance.SelectScreenMode(false);
    //    DisplayTabController.Instance.selectScreenResolution.ChangeDropdownInfo(PlayerPrefs.GetInt("display_sreen_resolution")); //1920x1080
    //    DisplayTabController.Instance.SelectScreenResolution(false);

    //}



    ////open
    //public void OpenModal(string title, string desc, UnityEngine.Events.UnityAction function)
    //{
    //    //remove all the listeners so we dont have stack of listeners
    //    modalWindowManager.onConfirm.RemoveAllListeners();
    //    modalWindowManager.onCancel.RemoveAllListeners();


    //    //play the modal sound
    //    AudioManager.Instance.PlaySfx("UI_Modal");

    //    //modal details
    //    modalWindowManager.icon = modalIcon; // Change icon
    //    modalWindowManager.titleText = title; // Change title
    //    modalWindowManager.descriptionText = desc; // Change desc
    //    modalWindowManager.UpdateUI(); // Update UI
    //    //modalWindowManager.Open(); // Open window
    //    modalWindowManager.AnimateWindow(); // Close/Open window automatically
    //    modalWindowManager.onConfirm.AddListener(function); // Add confirm events
    //    modalWindowManager.onCancel.AddListener(CloseModal); // Add cancel events

    //    //move the modal in the correct position (this is to keep it open and do not interfene with scene)
    //    modalWindowManager.gameObject.transform.parent.localPosition = new Vector3(0, 0, 0);
    //}

    //public void CloseModal()
    //{
    //    AudioManager.Instance.PlaySfx("UI_Cancel");
    //    modalWindowManager.Close(); // Close window
    //}


    //public void ModalOpenRestoreSettings()
    //{
    //    OpenModal("RESTORE", "Are you sure you want to restore settings to the default state?", RestoreUISettings);

    //}

    //public void RestoreUISettings()
    //{
    //    AudioManager.Instance.PlaySfx("UI_Confirm");
    //    modalWindowManager.Close(); // Close window
    //    DefaultSettings();
    //    //MainMenuBack("MAIN MENU");
    //}

    //public void ModalOpenSaveSettings()
    //{
    //    OpenModal("SAVE", "Are you sure you want to save settings?", SaveUISettings);
    //}

    //public void SaveUISettings()
    //{
    //    AudioManager.Instance.PlaySfx("UI_Confirm");
    //    modalWindowManager.Close(); // Close window
    //    SaveSettings();
    //   // MainMenuBack("MAIN MENU");
    //}

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

    //public void GoToMainMenu()
    //{
    //    OpenModal("BACK TO MAIN MENU", "Are you sure you want to go back to Main Menu? (Any unsaved changes will be lost!)", GoToMainMenuConfirm);
    //}

    //public void GoToMainMenuConfirm()
    //{
    //    AudioManager.Instance.PlaySfx("UI_Confirm");
    //    AudioManager.Instance.PlayMusic("UI_MainMenu");
    //    modalWindowManager.Close(); // Close window

    //    //change UI to gameplay
    //    UIManager.Instance.UICurrentMode = SystemManager.SystemModes.MAINMENU;

    //    //make UI inactive
    //    SystemManager.Instance.EnableDisable_UIManager(true);

    //    //load scene
    //    //SceneManager.LoadScene("scene_MainMenu");
    //    SystemManager.Instance.LoadScene("scene_MainMenu", 0.2f);
    //}

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




}
