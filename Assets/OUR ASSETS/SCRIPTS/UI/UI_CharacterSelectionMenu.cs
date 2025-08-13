using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UI_CharacterSelectionMenu : MonoBehaviour
{

    public static UI_CharacterSelectionMenu Instance;

    [Header("CHARACTERS")]


    public GameObject listOfCharactersGO;
    public GameObject listOfCompanionsGO;

    public GameObject characterTitle;
    public GameObject characterDescription;

    public GameObject companionTitle;
    public GameObject companionDescription;

    public GameObject characterPanelPrefab;


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


    // Start is called before the first frame update
    void Start()
    {
        UIManager.Instance.topPanelCombat.SetActive(false);
        //disable the proceed button
        //UIManager.Instance.DisableButton(proceedToGame);

        //display all characters
        DisplayAllCharacters();

        //display all companions
        DisplayAllCompanions();
    }



    public void DisplayAllCharacters()
    {


        //destroy all children objects
        foreach (Transform characterPanel in listOfCharactersGO.transform)
        {
            characterPanel.Find("Image").gameObject.SetActive(false);
           
        }

        int count = 0;

        foreach (ScriptableEntity scriptableEntity in CharacterManager.Instance.characterList)
        {


            //get ui object
            GameObject characterPanel = listOfCharactersGO.transform.GetChild(count).gameObject;

            characterPanel.transform.Find("Image").GetComponent<Image>().sprite = scriptableEntity.entityImage;
            characterPanel.transform.Find("Image").gameObject.SetActive(true);

            //add script
            characterPanel.AddComponent<SelectionScreenPrefab>();

            SelectionScreenPrefab selectionScreenPrefab = characterPanel.GetComponent<SelectionScreenPrefab>();

            selectionScreenPrefab.typeOfPrefab = SystemManager.SelectionScreenPrefabType.CHARACTER;
            selectionScreenPrefab.scriptableEntity = scriptableEntity;

            //first
            if (count == 0)
            {
                StaticData.staticCharacter = scriptableEntity.Clone();

                UI_CharacterSelectionMenu.Instance.characterTitle.GetComponent<TMP_Text>().text = StaticData.staticCharacter.entityName;
                UI_CharacterSelectionMenu.Instance.characterDescription.GetComponent<TMP_Text>().text = StaticData.staticCharacter.entityDescription;
            }

            count++;
        }
    }

    public void DisplayAllCompanions()
    {

        foreach (Transform characterPanel in listOfCompanionsGO.transform)
        {
            characterPanel.Find("Image").gameObject.SetActive(false);
       
        }

        int count = 0;


        foreach (ScriptableCompanion scriptableCompanion in CharacterManager.Instance.companionList)
        {
            //generate each character
            GameObject characterPanel = listOfCompanionsGO.transform.GetChild(count).gameObject;

            characterPanel.transform.Find("Image").GetComponent<Image>().sprite = scriptableCompanion.companionImage;
            characterPanel.transform.Find("Image").gameObject.SetActive(true);

            //add script
            characterPanel.AddComponent<SelectionScreenPrefab>();

            SelectionScreenPrefab selectionScreenPrefab = characterPanel.GetComponent<SelectionScreenPrefab>();

            selectionScreenPrefab.typeOfPrefab = SystemManager.SelectionScreenPrefabType.COMPANION;
            selectionScreenPrefab.scriptableCompanion = scriptableCompanion;

            //first
            if (count == 0)
            {
                StaticData.staticScriptableCompanion = scriptableCompanion.Clone();

                UI_CharacterSelectionMenu.Instance.companionTitle.GetComponent<TMP_Text>().text = StaticData.staticScriptableCompanion.companionName;
                UI_CharacterSelectionMenu.Instance.companionDescription.GetComponent<TMP_Text>().text = StaticData.staticScriptableCompanion.companionDescription;
            }

            count++;
        }
    }

    public void BackToGameModeMenu()
    {
        //play audio
        AudioManager.Instance.PlaySfx("UI_goBack");

        //open the correct menu
        //SceneManager.LoadScene("scene_GameModeMenu");
        SystemManager.Instance.LoadScene("scene_GameModeMenu", 0f, false, false);
    }

    public void BackToMainMenu()
    {
        //play audio
        AudioManager.Instance.PlaySfx("UI_goBack");

        //open the correct menu
        //SceneManager.LoadScene("scene_MainMenu");
        SystemManager.Instance.LoadScene("scene_MainMenu", 0f, false, false);
    }

    public void ProceedToGame()
    {

        //build the deck for the scriptable deck
        DeckManager.Instance.BuildStartingDeck();

        //initialize other important things
        ItemManager.Instance.GameStartItems();

        //SceneManager.LoadScene("scene_Adventure");
        SystemManager.Instance.LoadScene("scene_Adventure", 0f, false, false);


    }
}
