using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardListManager : MonoBehaviour
{
    public static CardListManager Instance;

    //classes card pool
    public List<ScriptableCard> Colorless_CardPool;
    public List<ScriptableCard> Knight_CardPool;
    public List<ScriptableCard> Rogue_CardPool;
    public List<ScriptableCard> Hierophant_CardPool;
    public List<ScriptableCard> Chaos_Mage_CardPool;
    public List<ScriptableCard> Ranger_CardPool;

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
