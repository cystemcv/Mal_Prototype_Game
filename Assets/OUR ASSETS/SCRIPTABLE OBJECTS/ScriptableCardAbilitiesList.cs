using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BuffSystemManager;
using static ScriptableCard;

[CreateAssetMenu(fileName = "AbilitiesList", menuName = "AbilitiesList")]
public class ScriptableCardAbilitiesList : ScriptableObject
{

    public List<ScriptableCardAbility> allAbilities;  // List of all abilities


}
