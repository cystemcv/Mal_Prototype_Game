using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UISaveLoad : MonoBehaviour
{
    public static UISaveLoad Instance;

    public delegate void OnCurrentModeChange();
    public event OnCurrentModeChange onCurrentModeChange;


    public TMP_Text saveLoadTitleText;

    //butttons for save/load

    public List<GameObject> saveLoadButtons;



    //this variable has event so we use getter and setter
    public SystemManager.SaveLoadModes SaveLoadMode
    {
        get { return SystemManager.Instance.saveLoadMode; }
        set
        {
            //if (currentMode == value) return;
            SystemManager.Instance.saveLoadMode = value;
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
        SaveLoadModeChangeDetectEvent();
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
        ////check each button if there is a savefile
        //InitializeSaveLoadButtons();

        //if (SystemManager.Instance.saveLoadMode == SystemManager.SaveLoadModes.SAVE)
        //{
        //    saveLoadTitleText.text = "SAVE";
        //}
        //else
        //{
        //    saveLoadTitleText.text = "LOAD";
        //}

   
    }

    public void InitializeSaveLoadButtons()
    {
        //autosave


        foreach (GameObject saveLoadButton in saveLoadButtons)
        {

            //main group of items
            GameObject IconParent = saveLoadButton.transform.GetChild(0).gameObject;
            GameObject TextGroup = saveLoadButton.transform.GetChild(1).gameObject;
            GameObject empty = saveLoadButton.transform.GetChild(2).gameObject;
            GameObject border = saveLoadButton.transform.GetChild(3).gameObject;

            //check if auto save button
            //if yes and we are on save then disable i

            if (SystemManager.Instance.saveLoadMode == SystemManager.SaveLoadModes.SAVE && saveLoadButton.GetComponent<GameObjectID>().objectID == "AutoSave")
            {
                //disable button
                //Debug.Log("DISABLED");
                saveLoadButton.GetComponent<Button>().interactable = false;

                ChangeButtonTransparency(TextGroup, empty, IconParent, border, 70);

            }
            else if(SystemManager.Instance.saveLoadMode == SystemManager.SaveLoadModes.LOAD && saveLoadButton.GetComponent<GameObjectID>().objectID == "AutoSave")
            {
                //saveLoadButton.SetActive(true);
                //Debug.Log("NOT DISABLED");
                saveLoadButton.GetComponent<Button>().interactable = true;

                ChangeButtonTransparency(TextGroup, empty, IconParent, border, 255);
            }

            //read from file
            FileDataHandler dataHandler = new FileDataHandler(Application.persistentDataPath, DataPersistenceManager.Instance.fileName + "_" + saveLoadButton.GetComponent<GameObjectID>().objectID + "." + DataPersistenceManager.Instance.fileType);

            //get from file
            GameData gameData = dataHandler.Load();

            //check if gameData exist
            if (gameData == null)
            {
    
                TextGroup.SetActive(false);
                empty.SetActive(true);

                if(SystemManager.Instance.saveLoadMode == SystemManager.SaveLoadModes.LOAD)
                {
                    saveLoadButton.GetComponent<Button>().interactable = false;
                    ChangeButtonTransparency(TextGroup, empty, IconParent, border, 70);
                }
                else
                {
                    saveLoadButton.GetComponent<Button>().interactable = true;
                    ChangeButtonTransparency(TextGroup, empty, IconParent, border, 255);
                }
            }
            else
            {
             
                TextGroup.SetActive(true);
                empty.SetActive(false);

                //implement text to show
                string gameplayTime = SystemManager.Instance.ConvertTimeToReadable(gameData.totalTimePlayed);
        
                TextGroup.transform.Find("timerText").gameObject.GetComponent<TMP_Text>().text = gameplayTime;
                TextGroup.transform.Find("MainSaveFileText").gameObject.GetComponent<TMP_Text>().text = "Save - " + saveLoadButton.GetComponent<GameObjectID>().objectID;

            }

        }


    }


    public void ChangeButtonTransparency(GameObject TextGroup, GameObject empty, GameObject IconParent, GameObject border, byte alpha)
    {

        //change the text in main group
        foreach (TMP_Text text in TextGroup.GetComponentsInChildren<TMP_Text>())
        {
            text.color = new Color32(255, 255, 255, alpha);
        }

        //change the text in empty
        foreach (TMP_Text text in empty.GetComponentsInChildren<TMP_Text>())
        {
            text.color = new Color32(255, 255, 255, alpha);
        }

        //change icon
        foreach (Image image in IconParent.GetComponentsInChildren<Image>())
        {
            image.color = new Color32(255, 255, 255, alpha);
        }

        //change the border
        foreach (Image image in border.GetComponentsInChildren<Image>())
        {
            image.color = new Color32(255, 255, 255, alpha);
        }
    }



    public void Save()
    {


    }

    public void Load()
    {

    }
}
