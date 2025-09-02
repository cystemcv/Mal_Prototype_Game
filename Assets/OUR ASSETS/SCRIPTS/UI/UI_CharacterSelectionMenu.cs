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


    [Header("HEROES")]
    public GameObject characterPanelPrefabParent;
    public GameObject characterPanelPrefab;

    [Header("HEROES PANEL")]
    public GameObject soIcon;
    public GameObject soSpaceShipIcon;
    public GameObject soClass;
    public GameObject soHP;
    public GameObject soShield;
    public GameObject soArmor;
    public GameObject soArtifactIcon;
    public GameObject soArtifactTitle;
    public GameObject soArtifactDescription;

    [Header("STARTING DECKS")]
    public GameObject startingDeckPanelPrefabParent;
    public GameObject startingDeckPanelPrefab;

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

    public void OnHoverCharacterPanel(ScriptableEntity scriptableEntity)
    {

        soIcon.GetComponent<Image>().sprite = scriptableEntity.entityImage;
        soSpaceShipIcon.GetComponent<Image>().sprite = scriptableEntity.entitySpaceShipImage;
        soClass.GetComponent<TMP_Text>().text = scriptableEntity.mainClass.ToString();
        soHP.transform.Find("TEXT").GetComponent<TMP_Text>().text = scriptableEntity.currHealth.ToString();
        soShield.transform.Find("TEXT").GetComponent<TMP_Text>().text = scriptableEntity.shield.ToString();
        soArmor.transform.Find("TEXT").GetComponent<TMP_Text>().text = scriptableEntity.armor.ToString();

        if (scriptableEntity.startingArtifacts.Count > 0)
        {
            soArtifactIcon.GetComponent<Image>().sprite = scriptableEntity.startingArtifacts[0].Icon;
            soArtifactIcon.transform.Find("Title").GetComponent<TMP_Text>().text = scriptableEntity.startingArtifacts[0].itemName;
            soArtifactIcon.transform.Find("Description").GetComponent<TMP_Text>().text = scriptableEntity.startingArtifacts[0].itemDescription;
        }

        InitializeStartingDecks(scriptableEntity);
    }


    // Start is called before the first frame update
    void Start()
    {
        UIManager.Instance.topPanelCombat.SetActive(false);
        //disable the proceed button
        //UIManager.Instance.DisableButton(proceedToGame);

        //display all characters
        DisplayAllCharacters();

        ////display all companions
        //DisplayAllCompanions();
    }



    public void DisplayAllCharacters()
    {


        //destroy all children objects
        SystemManager.Instance.DestroyAllChildren(characterPanelPrefabParent);

        int count = 0;

        foreach (ScriptableEntity scriptableEntity in CharacterManager.Instance.characterList)
        {

            GameObject characterPanel = Instantiate(characterPanelPrefab, characterPanelPrefabParent.transform);

            //get ui object
            //GameObject characterPanel = listOfCharactersGO.transform.GetChild(count).gameObject;

            characterPanel.transform.Find("Image").GetComponent<Image>().sprite = scriptableEntity.entityImage;
            characterPanel.transform.Find("Image").gameObject.SetActive(true);

            //add script
            //characterPanel.AddComponent<SelectionScreenPrefab>();

            SelectionScreenPrefab selectionScreenPrefab = characterPanel.GetComponent<SelectionScreenPrefab>();

            selectionScreenPrefab.typeOfPrefab = SystemManager.SelectionScreenPrefabType.CHARACTER;
            selectionScreenPrefab.scriptableEntity = scriptableEntity;

            //first
            if (count == 0)
            {
                StaticData.staticCharacter = scriptableEntity.Clone();

                OnHoverCharacterPanel(StaticData.staticCharacter);
            }

            count++;
        }




    }

    public void InitializeStartingDecks(ScriptableEntity scriptableEntity)
    {

        SystemManager.Instance.DestroyAllChildren(startingDeckPanelPrefabParent);

        foreach (ScriptableDeck scriptableDeck in scriptableEntity.startingDecks)
        {
            GameObject scriptableDeckPanel = Instantiate(startingDeckPanelPrefab, startingDeckPanelPrefabParent.transform);

            scriptableDeckPanel.transform.Find("TEXT").GetComponent<TMP_Text>().text = scriptableDeck.title;
            scriptableDeckPanel.GetComponent<SelectionScreenDeckPrefab>().scriptableDeck = scriptableDeck;

        }
    }

    //public void DisplayAllCompanions()
    //{

    //    foreach (Transform characterPanel in listOfCompanionsGO.transform)
    //    {
    //        characterPanel.Find("Image").gameObject.SetActive(false);
       
    //    }

    //    int count = 0;


    //    foreach (ScriptableCompanion scriptableCompanion in CharacterManager.Instance.companionList)
    //    {
    //        //generate each character
    //        GameObject characterPanel = listOfCompanionsGO.transform.GetChild(count).gameObject;

    //        characterPanel.transform.Find("Image").GetComponent<Image>().sprite = scriptableCompanion.companionImage;
    //        characterPanel.transform.Find("Image").gameObject.SetActive(true);

    //        //add script
    //        characterPanel.AddComponent<SelectionScreenPrefab>();

    //        SelectionScreenPrefab selectionScreenPrefab = characterPanel.GetComponent<SelectionScreenPrefab>();

    //        selectionScreenPrefab.typeOfPrefab = SystemManager.SelectionScreenPrefabType.COMPANION;
    //        selectionScreenPrefab.scriptableCompanion = scriptableCompanion;

    //        //first
    //        if (count == 0)
    //        {
    //            StaticData.staticScriptableCompanion = scriptableCompanion.Clone();

    //            UI_CharacterSelectionMenu.Instance.companionTitle.GetComponent<TMP_Text>().text = StaticData.staticScriptableCompanion.companionName;
    //            UI_CharacterSelectionMenu.Instance.companionDescription.GetComponent<TMP_Text>().text = StaticData.staticScriptableCompanion.companionDescription;
    //        }

    //        count++;
    //    }
    //}

    public void BackToGameModeMenu()
    {
        //play audio
        AudioManager.Instance.PlaySfx("UI_goBack");

        //open the correct menu
        UIManager.Instance.DisableAllUIScenes();
        UIManager.Instance.scenes_BG.SetActive(true);
        UIManager.Instance.scene_gameModes.SetActive(true);
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

    public void ProceedToGame()
    {

        //reset all values that need to be reset
        SystemManager.Instance.ResetGame();

        //new game
        CustomDungeonGenerator.Instance.dungeonIsGenerating = false;
        StaticData.staticDungeonParentGenerated = false;

        //build the deck for the scriptable deck
        DeckManager.Instance.BuildStartingDeck();

        CharacterManager.Instance.InitializeCharacterArtifacts();

        //initialize other important things
        ItemManager.Instance.GameStartItems();

        //SceneManager.LoadScene("scene_Adventure");
        UIManager.Instance.DisableAllUIScenes();
        SystemManager.Instance.LoadScene("scene_Adventure", 0f,0.2f, true, false);


    }
}
