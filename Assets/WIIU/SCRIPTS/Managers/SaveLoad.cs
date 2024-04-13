using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SaveLoad : MonoBehaviour
{
    public static SaveLoad Instance;

    public delegate void OnCurrentModeChange();
    public event OnCurrentModeChange onCurrentModeChange;

    public enum saveLoadMode
    {
        SAVE,
        LOAD
    }

    //initialize current mode
    private saveLoadMode currentMode = saveLoadMode.SAVE;

    //this variable has event so we use getter and setter
    public saveLoadMode CurrentMode
    {
        get { return currentMode; }
        set
        {
            if (currentMode == value) return;
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
        Debug.Log("Mode is " + currentMode.ToString()); ;
    }

    public void Save()
    {


    }

    public void Load()
    {

    }
}
