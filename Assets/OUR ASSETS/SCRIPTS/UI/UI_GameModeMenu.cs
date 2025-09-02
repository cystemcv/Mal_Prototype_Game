using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_GameModeMenu : MonoBehaviour
{

    public static UI_GameModeMenu Instance;

    public List<TutorialData> tutorialDataList = new List<TutorialData>();

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

    public void Start()
    {
        UIManager.Instance.topPanelCombat.SetActive(false);
        StartCoroutine(StartTutorial());
    }

    public IEnumerator StartTutorial()
    {
        yield return new WaitForSeconds(0.2f);
        TutorialManager.Instance.StartTutorial(tutorialDataList[0]);
        yield return new WaitUntil(() => !TutorialManager.Instance.IsTutorialActive);
    }

    public void GoToCharacterSelection_MainMode()
    {
        //play audio
        AudioManager.Instance.PlaySfx("UI_goNext");

        //change the mode
        SystemManager.Instance.gameMode = SystemManager.GameMode.MainMode;

        CombatManager.Instance.trainingMode = false;

        //open the correct menu
        // SceneManager.LoadScene("scene_CharacterSelectionMenu");
        SystemManager.Instance.LoadScene("scene_CharacterSelectionMenu", 0f, 0f, false, false);
    }



    public void GoToCharacterSelection_DuoMode()
    {
        //play audio
        AudioManager.Instance.PlaySfx("UI_goNext");

        //change the mode
        SystemManager.Instance.gameMode = SystemManager.GameMode.DuoMode;

        //open the correct menu
        //SceneManager.LoadScene("scene_CharacterSelectionMenu");
        SystemManager.Instance.LoadScene("scene_CharacterSelectionMenu", 0f, 0f, false, false);
    }

    public void BackToMainMenu()
    {
        //play audio
        AudioManager.Instance.PlaySfx("UI_goBack");

        //open the correct menu
        //SceneManager.LoadScene("scene_MainMenu");
        SystemManager.Instance.LoadScene("scene_MainMenu", 0f, 0f, false, false);
    }

    public void GoTo_TrainingMode()
    {
        //play audio
        AudioManager.Instance.PlaySfx("UI_goNext");

        ////build the deck for the scriptable deck
        //DeckManager.Instance.BuildStartingDeck();

        CombatManager.Instance.trainingMode = true;


        //open the correct menu
        // SceneManager.LoadScene("scene_CharacterSelectionMenu");
        SystemManager.Instance.LoadScene("scene_Combat", 0f, 0.2f, true, true);
    }
}
