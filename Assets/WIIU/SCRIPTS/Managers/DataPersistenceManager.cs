using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{

    [Header("File Paths")]
    [SerializeField] public string fileName;
    [SerializeField] public string fileType;

    public static DataPersistenceManager Instance;
    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;


    public string savefileID = "";
    public bool isLoadScene = false;

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
            Debug.Log("No data found, initializing!");
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
            Debug.Log("dataPersistenceObject : ");
            dataPersistenceObject.SaveData(ref gameData);
        }

        this.dataHandler.Save(gameData);
    }

    public List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        //find all the objects that implement the interface
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
