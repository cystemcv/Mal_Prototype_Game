using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler 
{
    private string dataDirectoryPath = "";
    private string dataFileName = "";

    public FileDataHandler(string dataDirectoryPath, string dataFileName)
    {
        this.dataDirectoryPath = dataDirectoryPath;
        this.dataFileName = dataFileName;
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(this.dataDirectoryPath, this.dataFileName);
        GameData loadedData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                //deserialize from json to c#
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception ex)
            {
                Debug.Log("Error trying to load from file : " + fullPath + "\n" + ex.Message);
            }
        }

        return loadedData;
    }

    public void DeleteFile()
    {
        string fullPath = Path.Combine(this.dataDirectoryPath, this.dataFileName);

        if (File.Exists(fullPath))
        {
            try
            {
                File.Delete(fullPath);
            }
            catch (Exception ex)
            {
                Debug.Log("Error trying to delete file : " + fullPath + "\n" + ex.Message);
            }
        }
    }

    public void Save(GameData data)
    {
        string fullPath = Path.Combine(this.dataDirectoryPath, this.dataFileName);
        try
        {
            //create the directory
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // data C# to json
            string dataToStore = JsonUtility.ToJson(data, true);

            //json to file
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error saving data to the file : " + fullPath + "\n" + ex.Message);
        }
    }
}
