using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "keywords", menuName = "keywords/ScriptableKeywords")]
public class ScriptableKeywords : ScriptableObject // Not sure if cards will be made from here or specialized for each class. Possibly change to abstract class later
{

    public string keywordName;
    public string keywordDescription;

}