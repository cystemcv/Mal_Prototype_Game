using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Card", menuName = "Card/Master")]
public class ScriptableCard : ScriptableObject // Not sure if cards will be made from here or specialized for each class. Possibly change to abstract class later
{


    [Header("MAIN")]
    public string cardName;
    public SystemManager.MainClass mainClass;
    public int primaryManaCost; // Amount of mana of the primary type to spend to play the card
    public int secondaryManaCost; // Amount of mana of the secondary type to spend to play the card

    public SystemManager.CardType cardType;
   
    public enum rarity { Common, Rare, Exalted, Legendary}; // Rarity of the card. Affects Strength of card, rules for cost, and likelihood of obtaining the card
    public rarity ourRarity; // Make enum show in inspector
    public Sprite cardArt; // Art to be displayed and attached to the card
    public string cardDesc; // Description of what the card does
    public string cardFlavor; // Flavor text- maybe not needed

    [Header("ABILITIES")]
    public int ability1Var = 0;
    public int ability2Var = 0;
    public int ability3Var = 0;
    public int ability4Var = 0;
    public int ability5Var = 0;
    public int ability6Var = 0;
    public List<ScriptableCardAbility> scriptableCardAbilities;

    [Header("SETTINGS")]
    public bool canTarget = false; //if it requires a target
    public bool targetEnemy = true; //by default is on,otherwise target character
    public bool playerMode1 = false; //available on 1 character mode
    public bool playerMode2 = false;//available on 2 character mode
    public bool playerMode3 = false;//available on 3 character mode
    public bool unlockable = false;//available from start = false, unlockable = true


}