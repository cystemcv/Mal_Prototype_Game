using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardListManager : MonoBehaviour
{
    public static CardListManager Instance;

    public GameObject cardPrefab;

    public List<ScriptableCard> cardPool;

    //classes card pool
    public List<ScriptableCard> Colorless_CardPool;
    public List<ScriptableCard> Knight_CardPool;
    public List<ScriptableCard> Rogue_CardPool;
    public List<ScriptableCard> Hierophant_CardPool;
    public List<ScriptableCard> Chaos_Mage_CardPool;
    public List<ScriptableCard> Ranger_CardPool;

    public enum AddCardTo { Hand, discardPile, combatDeck, mainDeck }
    public AddCardTo addCardTo;

    public enum CardType { Attack, Magic, Skill , Focus , Status, Curse,  }


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

    public Color GetClassColor(ScriptableCard scriptableCard)
    {
        Color color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorWhite);

        if (scriptableCard.mainClass == CharacterManager.MainClass.Knight)
        {
            color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorRed); 
        }
        else if (scriptableCard.mainClass == CharacterManager.MainClass.Rogue)
        {
            color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorYellow);
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
