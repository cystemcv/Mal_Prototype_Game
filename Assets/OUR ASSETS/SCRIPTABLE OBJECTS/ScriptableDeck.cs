using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "deck", menuName = "deck/deckObject")]
public class ScriptableDeck : ScriptableObject // Not sure if cards will be made from here or specialized for each class. Possibly change to abstract class later
{

    //Deck
    public List<CardScript> mainDeck;
    public List<ScriptableCard> deck;

    public string title = "";
    public string description = "";
    public string tooltip = "";

    public bool unlocked = true;

}