using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableScaling", menuName = "Scaling/ScriptableScaling")]
public class ScriptableScaling : ScriptableObject
{

    public int maxSteps = 30;



    public List<ScalingLevelBuffDebuff> scalingLevelBuffDebuffs = new List<ScalingLevelBuffDebuff>();

    [Serializable]
    public class ScalingLevelBuffDebuff
    {
        public ScriptableBuffDebuff scriptableBuffDebuff = new ScriptableBuffDebuff();
        public bool targetEnemySide = true;
        public int scalingValue;

    }
}


