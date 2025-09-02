using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{



    // Start is called before the first frame update


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

        //open the correct menu
        UIManager.Instance.DisableAllUIScenes();
        UIManager.Instance.scenes_BG.SetActive(true);
        UIManager.Instance.scene_gameModes.SetActive(true);
    }

    //LOAD
    public void UIOpenSaveMenu()
    {

        //play audio
        AudioManager.Instance.PlaySfx("UI_goNext");

        SystemManager.Instance.saveLoadMode = SystemManager.SaveLoadModes.SAVE;

        //open the correct menu
        //SceneManager.LoadScene("scene_LoadSaveMenu");
        SystemManager.Instance.LoadScene("scene_LoadSaveMenu", 0f,0f, false, false);
    }




    public void UIOpenLoadMenu()
    {

        //play audio
        AudioManager.Instance.PlaySfx("UI_goNext");

        SystemManager.Instance.saveLoadMode = SystemManager.SaveLoadModes.LOAD;

        //open the correct menu
        //SceneManager.LoadScene("scene_LoadSaveMenu");
        SystemManager.Instance.LoadScene("scene_LoadSaveMenu", 0f,0f, false, false);
    }

    //OPTIONS
    public void UIOpenOptionsMenu()
    {

        //play audio
        AudioManager.Instance.PlaySfx("UI_goNext");

        SystemManager.Instance.saveLoadMode = SystemManager.SaveLoadModes.LOAD;

        //open the correct menu
        UIManager.Instance.DisableAllUIScenes();
        UIManager.Instance.scenes_BG.SetActive(true);
        UIManager.Instance.scene_options.SetActive(true);
    }

    public void UIOpenLibrary()
    {
        AudioManager.Instance.PlaySfx("UI_goNext");
        UIManager.Instance.DisableAllUIScenes();
        UIManager.Instance.scenes_BG.SetActive(true);
        UIManager.Instance.scene_library.SetActive(true);
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
        SystemManager.Instance.LoadScene("scene_Combat", 0f,0.2f, true, false);
    }

    //EXIT

    public void ExitGame()
    {
        Application.Quit();
    }


}
