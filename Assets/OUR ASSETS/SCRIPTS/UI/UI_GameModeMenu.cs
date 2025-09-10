using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_GameModeMenu : MonoBehaviour
{

    public static UI_GameModeMenu Instance;

    public List<TutorialData> tutorialDataList = new List<TutorialData>();
    public ScriptablePlanets trainingPlanet;

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
       // UIManager.Instance.topPanelCombat.SetActive(false);
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
        UIManager.Instance.DisableAllUIScenes();
        UIManager.Instance.scenes_BG.SetActive(true);
        UIManager.Instance.scene_characterSelection.SetActive(true);
    }



    public void GoToCharacterSelection_DuoMode()
    {
        //play audio
        AudioManager.Instance.PlaySfx("UI_goNext");

        //change the mode
        SystemManager.Instance.gameMode = SystemManager.GameMode.DuoMode;

        //open the correct menu
        UIManager.Instance.DisableAllUIScenes();
        UIManager.Instance.scenes_BG.SetActive(true);
        UIManager.Instance.scene_characterSelection.SetActive(true);
    }

    public void BackToMainMenu()
    {
        //play audio
        AudioManager.Instance.PlaySfx("UI_goBack");

        //open the correct menu
        UIManager.Instance.DisableAllUIScenes();
        UIManager.Instance.scenes_BG.SetActive(true);
        UIManager.Instance.scene_mainMenu.SetActive(true);
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
        UIManager.Instance.DisableAllUIScenes();
        CombatManager.Instance.scriptablePlanet = trainingPlanet;
        SystemManager.Instance.LoadScene("scene_Combat", 0f, 0.2f, true, false);
    }
}
