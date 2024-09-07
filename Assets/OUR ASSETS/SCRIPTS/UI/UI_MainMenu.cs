using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //SCREENS

    //ADVENTURE

    public void UIOpenGameModeMenu()
    {

        //play audio
        AudioManager.Instance.PlaySfx("UI_goNext");

        //disable the 3rd option for now
        //DisableButton(UIMENU.transform.Find("HOLDER").Find("GAME MODE MENU").Find("ListOfGameModes").Find("Panel").Find("btn_UNKOWN").gameObject);

        //open the correct menu
        SceneManager.LoadScene("scene_GameModeMenu");
    }

    //LOAD
    public void UIOpenSaveMenu()
    {

        //play audio
        AudioManager.Instance.PlaySfx("UI_goNext");

        SystemManager.Instance.saveLoadMode = SystemManager.SaveLoadModes.SAVE;

        //open the correct menu
        SceneManager.LoadScene("scene_LoadSaveMenu");
    }




    public void UIOpenLoadMenu()
    {

        //play audio
        AudioManager.Instance.PlaySfx("UI_goNext");

        SystemManager.Instance.saveLoadMode = SystemManager.SaveLoadModes.LOAD;

        //open the correct menu
        SceneManager.LoadScene("scene_LoadSaveMenu");
    }

    //OPTIONS
    public void UIOpenOptionsMenu()
    {

        //play audio
        AudioManager.Instance.PlaySfx("UI_goNext");

        SystemManager.Instance.saveLoadMode = SystemManager.SaveLoadModes.LOAD;

        //open the correct menu
        SceneManager.LoadScene("scene_OptionsMenu");
    }

    //EXIT

    public void ExitGame()
    {
        Application.Quit();
    }


}
