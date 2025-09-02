using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UIManager;

public class UI_Library : MonoBehaviour
{



    // Start is called before the first frame update
    void Start()
    {
        UIManager.Instance.topPanelCombat.SetActive(false);
    }

    public void BackToMainMenu()
    {
        //play audio
        AudioManager.Instance.PlaySfx("UI_goBack");

        //open the correct menu
        //SceneManager.LoadScene("scene_MainMenu");
        SystemManager.Instance.LoadScene("scene_MainMenu", 0f,0f, false, false);
    }

    public void ViewCardList()
    {
        List<CardScriptData> cardScriptDataList = new List<CardScriptData>();
        cardScriptDataList = CardListManager.Instance.GetAllCardsFromLibrary();

        OptionsSettings optionsSettings = new OptionsSettings();
        optionsSettings.cardScriptDataList = cardScriptDataList;
        optionsSettings.cardListMode = CardListMode.VIEW;
        optionsSettings.enableCloseButton = true;
        optionsSettings.enableMinSelection = 0;
        optionsSettings.enableMaxSelection = 0;
        optionsSettings.title = "Card List";
        //optionsSettings.onConfirmAction = CombatManager.Instance.CardList_AddCardsToDeck;
        optionsSettings.allowClassButtons = true;
        optionsSettings.allowDuplicates = false;
        UIManager.Instance.ShowCardList(optionsSettings);
    }

    public void ViewArtifacts()
    {
        ItemManager.Instance.OpenArtifactPanel(false);
    }


    public void ViewTutorials()
    {

    }


}
