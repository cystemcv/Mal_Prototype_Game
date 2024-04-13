using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TestScript : MonoBehaviour, IDataPersistence
{
    public bool usePersistenceData = false;

    public TMP_Text text;
    public int deathCount;

    public void Start()
    {
        this.deathCount = 0;
    }

    public void LoadData(GameData data)
    {
        if (!usePersistenceData)
            return;

        this.deathCount = data.deathCount;
        text.text = this.deathCount.ToString();
    }

    public void SaveData(ref GameData data)
    {
        if (!usePersistenceData)
            return;

        Debug.Log("data : " + data.deathCount);
        data.deathCount = this.deathCount;
    }

    public void incrementText()
    {
        this.deathCount = this.deathCount + 1;
        text.text = this.deathCount.ToString();
    }

    public void SaveGameTest()
    {
        //DataPersistenceManager.Instance.SaveGame();
    }

    public void LoadGameTest()
    {
        //DataPersistenceManager.Instance.LoadGame();
    }

}
