using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{

    //everything saved and load should pass from here

    [Header("File Paths")]
    [SerializeField] public string fileName;
    [SerializeField] public string fileType;

    public static DataPersistenceManager Instance;
    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;


    public string savefileID = "";
    public bool isLoadScene = false;

    public List<string> tutorialsSeen = new List<string>();

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

        this.gameData = new GameData();
    }

    private void Start()
    {
        //DeleteDataEasy();
        LoadDataEasy();
    }

    public void SaveDataEasy()
    {
        ES3.Save("tutorialsSeen", tutorialsSeen);
    }

    public void LoadDataEasy()
    {
        if (ES3.KeyExists("tutorialsSeen"))
        {
            tutorialsSeen = ES3.Load<List<string>>("tutorialsSeen");
            Debug.Log("List loaded: " + string.Join(", ", tutorialsSeen));
        }
        else
        {
            Debug.Log("No saved list found.");
        }
    }

    public void DeleteDataEasy()
    {
        ES3.DeleteKey("tutorialsSeen");
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame(string objectID)
    {
        //read from file
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, this.fileName + "_" + objectID + "." + this.fileType);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();

        //get from file
        this.gameData = dataHandler.Load();

        //check if gameData exist
        if(this.gameData == null)
        {
        
            NewGame();
        }

        foreach(IDataPersistence dataPersistenceObject in this.dataPersistenceObjects)
        {
            dataPersistenceObject.LoadData(gameData);
        }
    }

    public void SaveGame(string objectID)
    {
        
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, this.fileName + "_" + objectID + "." + this.fileType);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();

        foreach (IDataPersistence dataPersistenceObject in this.dataPersistenceObjects)
        {
      
            dataPersistenceObject.SaveData(ref gameData);
        }

        this.dataHandler.Save(gameData);
    }

    public void DeleteSaveFile(string objectID)
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, this.fileName + "_" + objectID + "." + this.fileType);

        this.dataHandler.DeleteFile();
    }

    public List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        //find all the objects that implement the interface
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
