using System;
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

    [Header("CHARACTER SPECIFIC")]
    public List<ScriptableItem> startingArtifacts = new List<ScriptableItem>();

    [Header("COMMON")]
    public string entityName;
    public GameObject entityPrefab;
    public Sprite entityImage;
    public Sprite entityIcon;
    [TextArea] public string entityDescription;
    public GameObject entityChoose;

    [Header("SUMMON")]
    public int summonTurns = 1;

    [Header("PLAYER")]
    public GameObject entitySpaceShipPrefab;
    public Sprite entitySpaceShipImage;
    public RuntimeAnimatorController entityAnimator;
    public RuntimeAnimatorController spaceShipAnimator;

    [Header("AI")]
    public Vector3 spawnIntend = new Vector3(0, 1, 0);
    public AICommand aICommandsInitialize;
    public List<AICommand> aICommands = new List<AICommand>();



    [Header("STATS")]
    //Character Stats - Obviously subject to alteration
    public int strength = 0;
    public int defence = 0;

    public int maxHealth;
    public int currHealth; // current health
    public int maxMana;
    public int currMana; // current mana
    public int maxLevel;
    public int currLevel = 1;



    //counter dmg
    public int counterDamage = 0;

    //other stats
    public int poisonDmg = 1;

    // Deep clone method for a complex object
    public ScriptableEntity Clone()
    {
        ScriptableEntity clone = ScriptableObject.Instantiate(this);
        //clone.abilities = new List<string>(this.abilities); // Deep copy of the list
        return clone;
    }


    [Serializable]
    public class AICommand
    {

        public SystemManager.AICommandType aICommandType;
        public List<ScriptableCard> aiScriptableCards;

    }
}
