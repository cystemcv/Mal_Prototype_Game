using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_GameModeMenu : MonoBehaviour
{

    public static UI_GameModeMenu Instance;

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

    public void GoToCharacterSelection_MainMode()
    {
        //play audio
        AudioManager.Instance.PlaySfx("UI_goNext");

        //change the mode
        SystemManager.Instance.gameMode = SystemManager.GameMode.MainMode;

        //open the correct menu
       // SceneManager.LoadScene("scene_CharacterSelectionMenu");
        SystemManager.Instance.LoadScene("scene_CharacterSelectionMenu", 0f);
    }



    public void GoToCharacterSelection_DuoMode()
    {
        //play audio
        AudioManager.Instance.PlaySfx("UI_goNext");

        //change the mode
        SystemManager.Instance.gameMode = SystemManager.GameMode.DuoMode;

        //open the correct menu
        //SceneManager.LoadScene("scene_CharacterSelectionMenu");
        SystemManager.Instance.LoadScene("scene_CharacterSelectionMenu", 0f);
    }

    public void BackToMainMenu()
    {
        //play audio
        AudioManager.Instance.PlaySfx("UI_goBack");

        //open the correct menu
        //SceneManager.LoadScene("scene_MainMenu");
        SystemManager.Instance.LoadScene("scene_MainMenu", 0f);
    }

}
