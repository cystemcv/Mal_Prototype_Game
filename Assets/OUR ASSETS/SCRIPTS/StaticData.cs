using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class StaticData : MonoBehaviour
{
    // Class to store both value and score for each stat
    public class StatEntry
    {
        public int value;  // The stat value
        public int score;  // The score weight for this stat
    }

    public static List<CardScriptData> staticMainDeck = new List<CardScriptData>();

    public static ScriptableEntity staticCharacter;
    public static ScriptableCompanion staticScriptableCompanion;

    public static GameObject staticDungeonParent;
    public static bool staticDungeonParentGenerated = false;

    public static List<ClassItemData> lootItemList = new List<ClassItemData>();
    public static List<ClassItemData> inventoryItemList = new List<ClassItemData>();
    public static List<ClassItemData> companionItemList = new List<ClassItemData>();
    public static List<ClassItemData> artifactItemList = new List<ClassItemData>();

    public static List<CraftingRecipesData> craftingRecipesDataList = new List<CraftingRecipesData>();

    // Variables needed for artifacts
    public static CardScriptData artifact_CardScript;

    // JSON strings for combat and run stats (now including value & score)
    private static string combatJsonData = @"{
        ""Cards_Played"": {""value"": 0, ""score"": 10},
        ""Turns_Passed"": {""value"": 0, ""score"": 5},
        ""Total_Cards_Played"": {""value"": 0, ""score"": 2},
        ""Attack_Cards_Played"": {""value"": 0, ""score"": 4},
        ""Magic_Cards_Played"": {""value"": 0, ""score"": 6},
        ""Skill_Cards_Played"": {""value"": 0, ""score"": 3},
        ""Focus_Cards_Played"": {""value"": 0, ""score"": 8},
        ""Zero_Mana_Cost_Cards_Played"": {""value"": 0, ""score"": 1},
        ""One_Mana_Cost_Cards_Played"": {""value"": 0, ""score"": 2},
        ""Two_Mana_Cost_Cards_Played"": {""value"": 0, ""score"": 3},
        ""Three_Mana_Cost_Cards_Played"": {""value"": 0, ""score"": 4},
        ""Four_And_Above_Mana_Cost_Cards_Played"": {""value"": 0, ""score"": 5}
    }";

    private static string runJsonData = @"{
        ""Total_Turns_Passed"": {""value"": 0, ""score"": 5},
        ""Total_Attack_Cards_Played"": {""value"": 0, ""score"": 4},
        ""Total_Magic_Cards_Played"": {""value"": 0, ""score"": 6},
        ""Total_Skill_Cards_Played"": {""value"": 0, ""score"": 3},
        ""Total_Focus_Cards_Played"": {""value"": 0, ""score"": 8},
        ""Total_Zero_Mana_Cost_Cards_Played"": {""value"": 0, ""score"": 1},
        ""Total_One_Mana_Cost_Cards_Played"": {""value"": 0, ""score"": 2},
        ""Total_Two_Mana_Cost_Cards_Played"": {""value"": 0, ""score"": 3},
        ""Total_Three_Mana_Cost_Cards_Played"": {""value"": 0, ""score"": 4},
        ""Total_Four_And_Above_Mana_Cost_Cards_Played"": {""value"": 0, ""score"": 5},
        ""Total_Enemies_Killed"": {""value"": 0, ""score"": 10},
        ""Total_Elite_Enemies_Killed"": {""value"": 0, ""score"": 15},
        ""Total_Planets_Explored"": {""value"": 0, ""score"": 7},
        ""Total_Events_Encountered"": {""value"": 0, ""score"": 5},
        ""Total_Gold_Gained"": {""value"": 0, ""score"": 1},
        ""Total_Silver_Gained"": {""value"": 0, ""score"": 1}
    }";

    public static Dictionary<string, StatEntry> combatStats;
    public static Dictionary<string, StatEntry> runStats;

    // Initialize the JSON data into dictionaries
    public static void InitializeJsons()
    {
        combatStats = JsonConvert.DeserializeObject<Dictionary<string, StatEntry>>(combatJsonData);
        runStats = JsonConvert.DeserializeObject<Dictionary<string, StatEntry>>(runJsonData);
    }

    // Get the value of a given key
    public static int GetStatValue(Dictionary<string, StatEntry> stats, string key)
    {
        if (stats.ContainsKey(key))
        {
            return stats[key].value;
        }
        else
        {
            Debug.LogWarning($"Stat '{key}' does not exist.");
            return -1; // Return -1 to indicate the key does not exist
        }
    }

    // Increment the value of a given key
    public static void IncrementStat(Dictionary<string, StatEntry> stats, string key, int amount = 1)
    {
        if (stats.ContainsKey(key))
        {
            stats[key].value += amount;
        }
        else
        {
            Debug.LogWarning($"Stat '{key}' does not exist.");
        }
    }

    // Reset all stats to zero
    public static void ResetStats(Dictionary<string, StatEntry> stats)
    {
        foreach (var key in stats.Keys)
        {
            stats[key].value = 0;
        }
    }

    // Convert dictionary to JSON string
    public static string GetJson(Dictionary<string, StatEntry> stats)
    {
        return JsonConvert.SerializeObject(stats, Formatting.Indented);
    }

    // Convert dictionary to formatted text for UI
    public static string FormatStatsForText(Dictionary<string, StatEntry> stats)
    {
        string formattedText = "";
        foreach (var stat in stats)
        {
            int totalScore = stat.Value.value * stat.Value.score;
            formattedText += $"{stat.Key.Replace("_", " ")}: {stat.Value.value} (Score: {totalScore})<br>";
        }
        return formattedText;
    }

    public static int GetTotalJsonScore(Dictionary<string, StatEntry> stats)
    {
        int totalScore = 0;
        foreach (var stat in stats.Values)
        {
            totalScore += stat.value * stat.score; // Multiply value by score and sum up
        }
        return totalScore;
    }
}
