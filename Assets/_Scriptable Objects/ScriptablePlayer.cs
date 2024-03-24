using UnityEngine;

[CreateAssetMenu(fileName = "New Player", menuName = "Player/Class")]
public abstract class ScriptablePlayer : ScriptableObject
{
    public string playerName;
    //Character Name

    public enum mainClass { Knight, Rogue, Hierophant, Chaos_Mage, Ranger }; //Actual classes to be determined
    //Player's primary class

    public int Subclass = 0;
    /*Each character class will inherit from this class, but as far as I can tell, enums defined here would all need the same values for each class that inherits. I think
     * we need to use an int and convert that in the subclass.Or maybe I am wrong. but for now... */

    //Character Stats - Obviously subject to alteration
    public int strength = 1;
    //affects damage with most weapons, weapons and armor that can be used, and possibly... carry weight (does not affect damage for things like crossbows)

    public int awareness = 1;
    // Could affect initiative checks, spotting tracps, detecting lies etc

    public int fortitude = 1;
    //Constitution/general health and well being. affects max life, suspectability to poisons and physical hardships, possibly a modifier to damage received

    public int charisma = 1;
    // General likeability. Affects prices at merchants and every non fighting npc encounter to some degree

    public int inteligence = 1;
    // Affects magic ability, possible skill and card gains, possibly magic items or cards will have a minimum int attached to them

    public int deftness = 1;
    // Dexterity. Possibly affects ability to hit. finesse weapon damage including missile weapons. Ability to dodge attacks. Ability to avoid traps

    public int luck = 1;
    // Affects anything that is not completely skill based. Almost everything to some extent. 

    public int maxHealth;
    public int currHealth; // current health
    public int maxMana;
    public int currMana; // current mana
    public int maxLevel;
    public int currLevel = 1;


}