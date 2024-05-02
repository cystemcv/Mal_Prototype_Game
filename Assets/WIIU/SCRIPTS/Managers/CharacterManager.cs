using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;

    public ScriptablePlayer scriptablePlayer1;
    public ScriptablePlayer scriptablePlayer2;

    public enum MainClass { Knight, Rogue, Hierophant, Chaos_Mage, Ranger }; //Actual classes to be determined

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
}
