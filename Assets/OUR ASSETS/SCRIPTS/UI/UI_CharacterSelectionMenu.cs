using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class UI_CharacterSelectionMenu : MonoBehaviour
{

    public static UI_CharacterSelectionMenu Instance;

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

    [Header("CHARACTERS")]
    public GameObject characterSelectionContent;
    public GameObject characterPanelUIPrefab;
    public GameObject proceedToGame;

    // Start is called before the first frame update
    void Start()
    {

        //disable the proceed button
        UIManager.Instance.DisableButton(proceedToGame);

        //display all characters
        DisplayAllCharacters();
    }



    public void DisplayAllCharacters()
    {
        //destroy all children objects
        SystemManager.Instance.DestroyAllChildren(characterSelectionContent);

        foreach (ScriptableEntity scriptableEntity in CharacterManager.Instance.characterList)
        {

            //generate each character
            GameObject characterPanel = Instantiate(characterPanelUIPrefab, characterSelectionContent.transform.position, Quaternion.identity);
            characterPanel.transform.GetChild(0).GetComponent<CharacterCard>().scriptableEntity = scriptableEntity;

            //set it as a child of the parent
            characterPanel.transform.SetParent(characterSelectionContent.transform);

            //make the local scale 1,1,1
            characterPanel.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void BackToGameModeMenu()
    {
        //play audio
        AudioManager.Instance.PlaySfx("UI_goBack");

        //open the correct menu
        //SceneManager.LoadScene("scene_GameModeMenu");
        SystemManager.Instance.LoadScene("scene_GameModeMenu", 0.2f);
    }

    public void BackToMainMenu()
    {
        //play audio
        AudioManager.Instance.PlaySfx("UI_goBack");

        //open the correct menu
        //SceneManager.LoadScene("scene_MainMenu");
        SystemManager.Instance.LoadScene("scene_MainMenu", 0.2f);
    }

    public void ProceedToGame()
    {

        //build the deck for the scriptable deck
        DeckManager.Instance.BuildStartingDeck();

        //SceneManager.LoadScene("scene_Adventure");
        SystemManager.Instance.LoadScene("scene_Adventure", 0.2f);


    }
}
