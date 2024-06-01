using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardListManager : MonoBehaviour
{
    public static CardListManager Instance;

    public GameObject cardPrefab;

    //classes card pool
    public List<ScriptableCard> Colorless_CardPool;
    public List<ScriptableCard> Knight_CardPool;
    public List<ScriptableCard> Rogue_CardPool;
    public List<ScriptableCard> Hierophant_CardPool;
    public List<ScriptableCard> Chaos_Mage_CardPool;
    public List<ScriptableCard> Ranger_CardPool;

    //colors
    public Color32 redColor = new Color32(255, 0, 0, 255);
    public Color32 blueColor = new Color32(0, 121, 255, 255);
    public Color32 whiteColor = new Color32(255, 255, 255, 255);

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
