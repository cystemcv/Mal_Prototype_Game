using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemManager : MonoBehaviour, IDataPersistence
{
    public static SystemManager Instance;

    public GameObject uiManager;
    public GameObject audioManager;
    public GameObject dataPersistenceManager;

    //system variables
    public float totalTimePlayed = 0f;

    public enum SystemModes {
    
        MAINMENU,
        PAUSED,
        GAMEPLAY,
        COMBAT
    }

    public SystemModes currentSystemMode = SystemModes.MAINMENU;

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

    // called first
    void OnEnable()
    {
        Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        Debug.Log(mode);

        //check if the scene was from a load button
        if (DataPersistenceManager.Instance.isLoadScene)
        {
            DataPersistenceManager.Instance.LoadGame(DataPersistenceManager.Instance.savefileID);
        }

        //check if the scene is main menu
        if (scene.name == "scene_MainMenu")
        {
            currentSystemMode = SystemModes.MAINMENU;
        }
        else
        {
            currentSystemMode = SystemModes.GAMEPLAY;
        }

    }

    // called when the game is terminated
    void OnDisable()
    {
        Debug.Log("OnDisable");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void EnableDisable_UIManager(bool enable)
    {

        if (enable)
        {
            //make the default Main menu
            UIManager.Instance.NavigateMenu("MAIN MENU");
            uiManager.SetActive(true);
        }
        else
        {
            uiManager.SetActive(false);
        }

    }
    public void EnableDisable_AudioManager(bool enable)
    {
        if (enable)
        {
            audioManager.SetActive(true);
        }
        else
        {
            audioManager.SetActive(false);
        }
    }

    public float GetTotalPlayTime()
    {
        return totalTimePlayed;
    }

    public void Update()
    {
        if (this.currentSystemMode == SystemModes.GAMEPLAY) {
            this.totalTimePlayed += Time.deltaTime;
        }
    }

    public void LoadData(GameData data)
    {
        this.totalTimePlayed = data.totalTimePlayed; 
    }

    public void SaveData(ref GameData data)
    {
        Debug.Log("time : " + data.totalTimePlayed.ToString());
        data.totalTimePlayed = this.totalTimePlayed;
    }

    public string ConvertTimeToReadable(float totalTime)
    {
        // Convert totalTime to hours, minutes, and seconds
        int hours = (int)(totalTime / 3600);
        int minutes = (int)((totalTime % 3600) / 60);
        int seconds = (int)(totalTime % 60);

        string readableTime = hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");

        return readableTime;
    }

    public void RuntimeInitializeOnLoadMethod()
    {
        Debug.Log("CHANGING SCENES");
    }
}
