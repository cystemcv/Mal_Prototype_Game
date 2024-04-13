using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;



public class UISaveLoad : MonoBehaviour
{
    public static UISaveLoad Instance;

    public delegate void OnCurrentModeChange();
    public event OnCurrentModeChange onCurrentModeChange;


    public TMP_Text saveLoadTitleText;

    //butttons for save/load

    public List<GameObject> saveLoadButtons;

    public enum saveLoadMode
    {
        SAVE,
        LOAD
    }

    //initialize current mode
    public saveLoadMode currentMode = saveLoadMode.SAVE;

    //this variable has event so we use getter and setter
    public saveLoadMode CurrentMode
    {
        get { return currentMode; }
        set
        {
            //if (currentMode == value) return;
            currentMode = value;
            if (onCurrentModeChange != null)
                onCurrentModeChange();
        }
    }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    
    }

    public void Start()
    {
        //check each button if there is a savefile
        InitializeSaveLoadButtons();
    }

    //subscribe events
    private void OnEnable()
    {
        onCurrentModeChange += SaveLoadModeChangeDetectEvent;

    }

    private void OnDisable()
    {
        onCurrentModeChange -= SaveLoadModeChangeDetectEvent;

     
    }

    public void SaveLoadModeChangeDetectEvent()
    {

        if (currentMode == UISaveLoad.saveLoadMode.SAVE)
        {
            saveLoadTitleText.text = "SAVE";
        }
        else
        {
            saveLoadTitleText.text = "LOAD";
        }

        Debug.Log("Mode is " + currentMode.ToString()); ;
    }

    public void InitializeSaveLoadButtons()
    {
        //autosave


        foreach (GameObject saveLoadButton in saveLoadButtons)
        {
            //read from file
            FileDataHandler dataHandler = new FileDataHandler(Application.persistentDataPath, DataPersistenceManager.Instance.fileName + "_" + saveLoadButton.GetComponent<GameObjectID>().objectID + "." + DataPersistenceManager.Instance.fileType);

            //get from file
            GameData gameData = dataHandler.Load();

            //check if gameData exist
            if (gameData == null)
            {
                //main group of items
                GameObject TextGroup = saveLoadButton.transform.GetChild(1).gameObject;
                TextGroup.SetActive(false);
                //empty item
                GameObject empty = saveLoadButton.transform.GetChild(2).gameObject;
                empty.SetActive(true);
            }
            else
            {
                //main group of items
                GameObject TextGroup = saveLoadButton.transform.GetChild(1).gameObject;
                TextGroup.SetActive(true);
                //empty item
                GameObject empty = saveLoadButton.transform.GetChild(2).gameObject;
                empty.SetActive(false);

                Debug.Log("TEST : " + gameData.totalTimePlayed);

                //implement text to show
                string gameplayTime = SystemManager.Instance.ConvertTimeToReadable(gameData.totalTimePlayed);
        
                TextGroup.transform.Find("timerText").gameObject.GetComponent<TMP_Text>().text = gameplayTime;
                TextGroup.transform.Find("MainSaveFileText").gameObject.GetComponent<TMP_Text>().text = "Save - " + saveLoadButton.GetComponent<GameObjectID>().objectID;

            }

        }


    }





    public void Save()
    {


    }

    public void Load()
    {

    }
}
