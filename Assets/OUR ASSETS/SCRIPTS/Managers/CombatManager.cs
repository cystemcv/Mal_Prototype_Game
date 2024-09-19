using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;


    [Header("GLOBAL VARIABLES")]
    public int manaMaxAvailable = 3;
    public int manaAvailable = 0;



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
