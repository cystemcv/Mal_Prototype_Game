using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Card", menuName = "Card/Master")]
public class ScriptableCard : ScriptableObject // Not sure if cards will be made from here or specialized for each class. Possibly change to abstract class later
{
    public string cardName;
    //Card Name

    public enum mainCostType { Colorless, Knight, Rogue, Hierophant, Chaos_Mage, Ranger}; // Color/Class of main mana type
    //Primary mana type
    public mainCostType ourMainCostType; // Make enum show in inspector

    public int primaryManaCost; // Amount of mana of the primary type to spend to play the card

    public enum subCostType { Colorless, Knight, Rogue, Hierophant, Chaos_Mage, Ranger}; // Color/Class of secondary mana type
    //Secondary mana type
    public subCostType ourSubCostType; // Make enum show in inspector

    public int secondaryManaCost; // Amount of mana of the secondary type to spend to play the card

    public enum rarity { Common, Rare, Exalted, Legendary}; // Rarity of the card. Affects Strength of card, rules for cost, and likelihood of obtaining the card
    public rarity ourRarity; // Make enum show in inspector

    public enum effectType { Attack, Defend, Trinket, Buff, Debuff, Multiple, Other}; // Type of card. Subject to change. Probably will also affect the color of 
    // something on the card as a visual cue to card purpose
    public effectType ourEffectType;

    public Image cardArt; // Art to be displayed and attached to the card

    public string cardDesc; // Description of what the card does

    public string cardFlavor; // Flavor text- maybe not needed

}