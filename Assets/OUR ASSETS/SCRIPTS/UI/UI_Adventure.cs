using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Adventure : MonoBehaviour
{



    // Start is called before the first frame update
    void Start()
    {
        UIManager.Instance.topPanelCombat.SetActive(true);
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
        //SceneManager.LoadScene("scene_GameModeMenu");
        SystemManager.Instance.LoadScene("scene_GameModeMenu",0f,0f, false, false);
    }

    //LOAD
    public void UIOpenSaveMenu()
    {

        //play audio
        AudioManager.Instance.PlaySfx("UI_goNext");

        SystemManager.Instance.saveLoadMode = SystemManager.SaveLoadModes.SAVE;

        //open the correct menu
        //SceneManager.LoadScene("scene_LoadSaveMenu");
        SystemManager.Instance.LoadScene("scene_LoadSaveMenu", 0f, 0f, false, false);
    }




    public void UIOpenLoadMenu()
    {

        //play audio
        AudioManager.Instance.PlaySfx("UI_goNext");

        SystemManager.Instance.saveLoadMode = SystemManager.SaveLoadModes.LOAD;

        //open the correct menu
        //SceneManager.LoadScene("scene_LoadSaveMenu");
        SystemManager.Instance.LoadScene("scene_LoadSaveMenu", 0f, 0f, false, false);
    }

    //OPTIONS
    public void UIOpenOptionsMenu()
    {

        //play audio
        AudioManager.Instance.PlaySfx("UI_goNext");

        SystemManager.Instance.saveLoadMode = SystemManager.SaveLoadModes.LOAD;

        //open the correct menu
        //SceneManager.LoadScene("scene_OptionsMenu");
        SystemManager.Instance.LoadScene("scene_LoadSaveMenu", 0f, 0f, false, false);
    }

    //OPTIONS
    public void UIGoToCombat()
    {

        //play audio
        AudioManager.Instance.PlaySfx("UI_goNext");

        //DeckManager.Instance.BuildStartingDeck();


        //if deck is not created then create it / this should only be available when not having deck
        DeckManager.Instance.BuildStartingDeck();
       

        //open the correct menu
        //SceneManager.LoadSceneAsync("scene_Combat", LoadSceneMode.Additive);
        //SceneManager.LoadScene("scene_Combat");
        SystemManager.Instance.LoadScene("scene_Combat", 0f, 0.2f, true, false);
    }

    //EXIT

    public void ExitGame()
    {
        Application.Quit();
    }


}
