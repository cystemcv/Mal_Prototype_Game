using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;

    public List<ScriptablePlayer> characterList;

    public List<ScriptablePlayer> scriptablePlayerList;

    public enum GameMode { MainMode,DuoMode,AnyMode }

    public GameMode gameMode = GameMode.MainMode;

    //used by scriptable objects
    public enum MainClass { Knight, Rogue, Hierophant, Chaos_Mage, Ranger, Sniper }; //Actual classes to be determined

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


    public int CheckCharacterLimitBasedOnMode()
    {

        int limit = 0;

        if (gameMode == GameMode.MainMode) {
            limit = 1;
        }
        else if (gameMode == GameMode.DuoMode)
        {
            limit = 2;
        }
        else
        {
            limit = 1;
        }

        return limit;

    }
   

}