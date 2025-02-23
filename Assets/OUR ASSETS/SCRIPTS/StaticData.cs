using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticData : MonoBehaviour
{

    public static List<CardScript> staticMainDeck = new List<CardScript>();

    public static ScriptableEntity staticCharacter;
    public static ScriptableCompanion staticScriptableCompanion;

    public static GameObject staticDungeonParent;
    public static bool staticDungeonParentGenerated = false;

    public static List<ClassItem> lootItemList = new List<ClassItem>();
    public static List<ClassItem> inventoryItemList = new List<ClassItem>();
    public static List<ClassItem> companionItemList = new List<ClassItem>();
    public static List<ClassItem> artifactItemList = new List<ClassItem>();


    //variables needed for artifacts]
    public static CardScript artifact_CardScript;

    private static string combatJsonData = @"
    {
        ""Cards_Played"": 0,
        ""Turns_Passed"": 0,
        ""Total_Cards_Played"": 0,
        ""Attack_Cards_Played"": 0,
        ""Magic_Cards_Played"": 0,
        ""Skill_Cards_Played"": 0,
        ""Focus_Cards_Played"": 0,
        ""Zero_Mana_Cost_Cards_Played"": 0,
        ""One_Mana_Cost_Cards_Played"": 0,
        ""Two_Mana_Cost_Cards_Played"": 0,
        ""Three_Mana_Cost_Cards_Played"": 0,
        ""Four_And_Above_Mana_Cost_Cards_Played"": 0,
    }";


    private static string runJsonData = @"
    {
        ""Total_Turns_Passed"": 0,
        ""Total_Attack_Cards_Played"": 0,
        ""Total_Magic_Cards_Played"": 0,
        ""Total_Skill_Cards_Played"": 0,
        ""Total_Focus_Cards_Played"": 0,
        ""Total_Zero_Mana_Cost_Cards_Played"": 0,
        ""Total_One_Mana_Cost_Cards_Played"": 0,
        ""Total_Two_Mana_Cost_Cards_Played"": 0,
        ""Total_Three_Mana_Cost_Cards_Played"": 0,
        ""Total_Four_And_Above_Mana_Cost_Cards_Played"": 0,
        ""Total_Enemies_Killed"": 0,
        ""Total_Elite_Enemies_Killed"": 0,
        ""Total_Planets_Explored"": 0,
        ""Total_Events_Encountered"": 0,
        ""Total_Gold_Gained"": 0,
        ""Total_Silver_Gained"": 0
    }";

    public static Dictionary<string, int> combatStats;
    public static Dictionary<string, int> runStats;

    //public StaticData()
    //{
    //    combatStats = JsonConvert.DeserializeObject<Dictionary<string, int>>(combatJsonData);
    //    runStats = JsonConvert.DeserializeObject<Dictionary<string, int>>(runJsonData);
    //}

    public static void InitializeJsons()
    {
        combatStats = JsonConvert.DeserializeObject<Dictionary<string, int>>(combatJsonData);
        runStats = JsonConvert.DeserializeObject<Dictionary<string, int>>(runJsonData);
    }

    // Get the value of a given key
    public static int GetStatValue(Dictionary<string,int> stats ,string key)
    {
        if (stats.ContainsKey(key))
        {
            return stats[key];
        }
        else
        {
            Debug.LogWarning($"Stat '{key}' does not exist.");
            return -1; // Return -1 to indicate the key does not exist
        }
    }

    public static void IncrementStat(Dictionary<string, int> stats, string key, int amount = 1)
    {
        if (stats.ContainsKey(key))
        {
            stats[key] += amount;
        }
        else
        {
            Debug.LogWarning($"Stat '{key}' does not exist.");
        }
    }



    public static void ResetRunStats(Dictionary<string, int> stats)
    {
        foreach (var key in stats.Keys)
        {
            stats[key] = 0;
        }
    }

    public static string GetJson(Dictionary<string, int> stats)
    {
        return JsonConvert.SerializeObject(stats, Formatting.Indented);
    }

    public static string FormatStatsForText(Dictionary<string, int> stats)
    {
        string formattedText = "";

        foreach (var stat in stats)
        {
            formattedText += $"{stat.Key.Replace("_", " ")}: {stat.Value} <br>";
        }

        return formattedText;
    }
}
