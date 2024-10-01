using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ScriptableEntity", menuName = "Entity/ScriptableEntity")]
public class ScriptableEntity : ScriptableObject
{
    [Header("CARDS")]
    public List<ScriptableCard> startingCards; //assign the cards you want the class to start building the deck

    [Header("CHARACTER SPECIFIC")]
    public SystemManager.MainClass mainClass; //Actual classes to be determined
    public float distanceFromAnotherUnit = 0f;
    public int unitsWorth = 1;

    [Header("COMMON")]
    public string entityName;
    public GameObject entityPrefab;
    public Sprite entityImage;
    public Sprite entityIcon;
    public string entityDescription;


    [Header("STATS")]
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


    //other stats
    public int poisonDmg = 1;

}