using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardListManager : MonoBehaviour
{
    public static CardListManager Instance;

    public Sprite commonBg;
    public Sprite rareBg;
    public Sprite epicBg;
    public Sprite legendaryBg;
    public Sprite curseBg;

    public GameObject cardPrefab;


    //classes card pool
    [FoldoutGroup("CARD POOLS")]
    public List<CardPoolList> cardPoolLists;

    [Serializable]
    public class CardPoolList
    {
        [Title("CONDITION"), GUIColor("orange")]
        public List<ScriptableCard> scriptableCards;
        [Title("CLASS")]
        public SystemManager.MainClass mainClass = SystemManager.MainClass.COMMON;
    }


    //card abilities prefabs
    public GameObject cardPrefabShield;


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

    public Color GetClassColor(SystemManager.MainClass mainClass)
    {
        Color color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorWhite);

        if (mainClass == SystemManager.MainClass.MONSTER)
        {
            color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorRed); 
        }
        else if (mainClass == SystemManager.MainClass.ANGEL)
        {
            color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorBlue);
        }

        return color;
    }

//    public Color32 GetClassColor(ScriptableCard scriptableCard)
//    {

//        Color32 color32 = whiteColor;

//        if (scriptableCard.mainClass == CharacterManager.MainClass.Knight)
//        {
//            color32 = redColor;
//        }
//        else if (scriptableCard.mainClass == CharacterManager.MainClass.Rogue)
//        {
//color32 = 
//        }

//        Color colorFromHex;
//        ColorUtility.TryParseHtmlString("#FFEC19", out colorFromHex);

//        return color32;

//    }


}
