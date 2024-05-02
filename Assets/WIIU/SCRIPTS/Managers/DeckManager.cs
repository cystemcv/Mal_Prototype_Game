using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance;

    //player 1 deck
    public List<ScriptableCard> deck1;

    //player 2 deck
    public List<ScriptableCard> deck2;

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

    public void AddCardOnDeck(ScriptableCard card, int character)
    {
        if (character == 1)
        {
            deck1.Add(card);
        }
        else
        {
            deck2.Add(card);
        }


    }

    public void RemoveCardFromDeck(ScriptableCard card, int character)
    {
        if (character == 1)
        {
            deck1.Remove(card);
        }
        else
        {
            deck2.Remove(card);
        }
    }

    public void InitializeCardOnPrefab(ScriptableCard card, GameObject parent)
    {

        //instantiate the prefab 

        //set it as a child of the parent

        //use the scriptable object to fill the art, text (title,desc,mana cost,etc)

    }

}
