using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableScaling", menuName = "Scaling/ScriptableScaling")]
public class ScriptableScaling : ScriptableObject
{

    public int maxSteps = 30;
    public int maxScalingLevel = 10;

    [RequiredListLength(10, 10)]
    public List<int> scalingHealth = new List<int>();
    [RequiredListLength(10, 10)]
    public List<int> scalingAttack = new List<int>();
    [RequiredListLength(10, 10)]
    public List<int> scalingDefence = new List<int>();
}
