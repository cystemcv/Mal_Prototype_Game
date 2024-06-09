using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData 
{
    //system
    public float totalTimePlayed;
    public int runsCompleted;


    public int deathCount;

    public GameData()
    {
        this.deathCount = 0;
        this.totalTimePlayed = 0f;
    }
}
