using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Michsky.MUIP;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using System.Collections;

public class TutorialManager : MonoBehaviour
{

    public List<TutorialData> tutorialDataList = new List<TutorialData>();

    public Image stepImage;
    public TMP_Text stepTitle;
    public TMP_Text stepDescription;
    public Button mainButton; // Next or Close
    public Button backButton;
    public GameObject tutorialPanel;

    private TutorialData currentTutorial;
    private int currentStepIndex;

    public static TutorialManager Instance;

    public GameObject mm_TutorialOpen_Prefab;
    public GameObject mm_TutorialClose_Prefab;
    public bool IsTutorialActive { get; private set; }

    public bool allowTutorials = true;

    void Awake()
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
        tutorialPanel.SetActive(false);

        mainButton.onClick.AddListener(OnMainButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    public void StartTutorial(TutorialData tutorial)
    {

        //check if already played

        if (!allowTutorials)
        {
            return;
        }

        if (DataPersistenceManager.Instance.tutorialsSeen.Contains(tutorial.tutorialName))
        {
            return;
        }

        DataPersistenceManager.Instance.tutorialsSeen.Add(tutorial.tutorialName);
        DataPersistenceManager.Instance.SaveDataEasy();

        tutorialPanel.SetActive(true);
        AudioManager.Instance.PlaySfx("UI_Modal");
        MMF_Player mmInstance =  mm_TutorialOpen_Prefab.GetComponent<MMF_Player>();
        mmInstance.Initialization();
        mmInstance.PlayFeedbacks();
        IsTutorialActive = true;

        currentTutorial = tutorial;
        currentStepIndex = 0;

        ShowStep();


        Time.timeScale = 0f; // Pause game
    }

    private void ShowStep()
    {
        var step = currentTutorial.steps[currentStepIndex];

        stepImage.sprite = step.image;
        stepTitle.text = step.title;
        stepDescription.text = step.description;

        // Back button visible only if not first step
        backButton.gameObject.SetActive(currentStepIndex > 0);

        // Change label between "Next" and "Close"
        if (currentStepIndex < currentTutorial.steps.Length - 1)
            mainButton.GetComponent<ButtonManager>().SetOnlyText("Next");
        else
            mainButton.GetComponent<ButtonManager>().SetOnlyText("Close");
    }

    private void OnMainButtonClicked()
    {
        if (currentStepIndex < currentTutorial.steps.Length - 1)
        {
            AudioManager.Instance.PlaySfx("UI_goNext");
            currentStepIndex++;
            ShowStep();
        }
        else
        {
            CloseTutorial();
        }
    }

    private void OnBackButtonClicked()
    {
        if (currentStepIndex > 0)
        {
            AudioManager.Instance.PlaySfx("UI_goBack");
            currentStepIndex--;
            ShowStep();
        }
    }

    private void CloseTutorial()
    {
        AudioManager.Instance.PlaySfx("UI_Cancel");
        MMF_Player mmInstance = mm_TutorialClose_Prefab.GetComponent<MMF_Player>();
        mmInstance.Initialization();
        mmInstance.PlayFeedbacks();


        Time.timeScale = 1f; // Resume game

        StartCoroutine(CloseTutorialTimer());
    }

    public IEnumerator CloseTutorialTimer()
    {
        yield return new WaitForSeconds(0.21f);
        tutorialPanel.SetActive(false);
        IsTutorialActive = false;

    }

    public IEnumerator StartTutorialIE(TutorialData tutorialData)
    {
        yield return new WaitForSeconds(0.2f);
        TutorialManager.Instance.StartTutorial(tutorialData);
        yield return new WaitUntil(() => !TutorialManager.Instance.IsTutorialActive);
    }

}
