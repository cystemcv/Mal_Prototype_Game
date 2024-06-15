using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;

    public List<ScriptablePlayer> characterList;

    public List<ScriptablePlayer> scriptablePlayerList;





    //used by scriptable objects


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

        if (SystemManager.Instance.gameMode == SystemManager.GameMode.MainMode) {
            limit = 1;
        }
        else if (SystemManager.Instance.gameMode == SystemManager.GameMode.DuoMode)
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
