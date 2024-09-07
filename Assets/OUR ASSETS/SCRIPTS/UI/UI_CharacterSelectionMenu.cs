using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class UI_CharacterSelectionMenu : MonoBehaviour
{

    [Header("CHARACTERS")]
    public GameObject characterSelectionContent;
    public GameObject characterPanelUIPrefab;
    public GameObject proceedToGame;

    // Start is called before the first frame update
    void Start()
    {
        //clear the list
        CharacterManager.Instance.scriptablePlayerList.Clear();

        //disable the proceed button
        UIManager.Instance.DisableButton(UIManager.Instance.proceedToGame);

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
        SceneManager.LoadScene("scene_GameModeMenu");
    }

    public void BackToMainMenu()
    {
        //play audio
        AudioManager.Instance.PlaySfx("UI_goBack");

        //open the correct menu
        SceneManager.LoadScene("scene_MainMenu");
    }

    public void ProceedToGame()
    {

        //build the deck
        DeckManager.Instance.BuildStartingDeck();

        SceneManager.LoadScene("scene_Adventure");


    }
}
