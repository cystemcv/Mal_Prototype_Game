using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GameModeMenu : MonoBehaviour
{

    public void GoToCharacterSelection_MainMode()
    {
        //play audio
        AudioManager.Instance.PlaySfx("UI_goNext");

        //change the mode
        SystemManager.Instance.gameMode = SystemManager.GameMode.MainMode;

        //open the correct menu
        UI_Menus.Instance.NavigateMenu("CHARACTER SELECTION MENU");
    }



    public void GoToCharacterSelection_DuoMode()
    {
        //play audio
        AudioManager.Instance.PlaySfx("UI_goNext");

        //change the mode
        SystemManager.Instance.gameMode = SystemManager.GameMode.DuoMode;

        //open the correct menu
        UI_Menus.Instance.NavigateMenu("CHARACTER SELECTION MENU");
    }

    public void BackToMainMenu()
    {
        //play audio
        AudioManager.Instance.PlaySfx("UI_goBack");

        //open the correct menu
        UI_Menus.Instance.NavigateMenu("MAIN MENU");
    }

}
