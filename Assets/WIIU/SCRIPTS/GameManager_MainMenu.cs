using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager_MainMenu : MonoBehaviour
{

    //main gameobjects of the UI
    public List<GameObject> list_goMenuItem;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MakeAllMenuInactive()
    {
        foreach (GameObject goMenuItem in list_goMenuItem)
        {
            goMenuItem.SetActive(false);
        }
    }

    public void MainMenuNext(string menuName)
    {
        //play audio
        AudioManager.Instance.PlaySfx("UI_goNext");

        //open the correct menu
        NavigateMenu(menuName);
    }

    public void MainMenuBack(string menuName)
    {
        //play audio
        AudioManager.Instance.PlaySfx("UI_goBack");

        //open the correct menu
        NavigateMenu(menuName);
    }

    private void NavigateMenu(string menuName)
    {

        //make everything inactive
        MakeAllMenuInactive();

        //make the menu we need active
        GameObject goMenuItem = list_goMenuItem.Find(item => item.name == menuName);
        goMenuItem.SetActive(true);
    }

}
