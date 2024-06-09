using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectID: MonoBehaviour
{

    public string objectType = "";
    public string objectID = "";


    public void SaveOrLoadButtonClick()
    {
        //save
        if (UISaveLoad.Instance.currentMode == UISaveLoad.saveLoadMode.SAVE)
        {
            GameObjectID gameObjectID = this.gameObject.GetComponent<GameObjectID>();
            DataPersistenceManager.Instance.SaveGame(gameObjectID.objectID);
            UISaveLoad.Instance.InitializeSaveLoadButtons();
        }
        else
        {
            //load
            GameObjectID gameObjectID = this.gameObject.GetComponent<GameObjectID>();
            DataPersistenceManager.Instance.savefileID = gameObjectID.objectID;
            DataPersistenceManager.Instance.isLoadScene = true;
            UIManager.Instance.GoToScene("SampleScene");



            //GameObjectID gameObjectID = this.gameObject.GetComponent<GameObjectID>();
            //DataPersistenceManager.Instance.LoadGame(gameObjectID);
      
        }
    }

    public void DeleteSaveFileClick()
    {
 
            GameObjectID gameObjectID = this.gameObject.GetComponent<GameObjectID>();
            DataPersistenceManager.Instance.DeleteSaveFile(gameObjectID.objectID);
            UISaveLoad.Instance.InitializeSaveLoadButtons();
    }
}
