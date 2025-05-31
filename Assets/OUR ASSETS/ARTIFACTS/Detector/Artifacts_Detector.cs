using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Artifacts_Detector", menuName = "Item/Artifacts/Artifacts_Detector")]
public class Artifacts_Detector : ScriptableItem
{
    //[Title("UNIQUE ITEM ABILITY")]
    //public int multiHits = 1;

    public bool activated = false;

    public override void Activate(ClassItemData classItem, CardScript cardScript)
    {


        if (activated)
        {
            return;
        }

        int hiddenRooms = 0;
        int rewardRooms = 0;

        foreach (GameObject room in CustomDungeonGenerator.Instance.rooms)
        {
            RoomScript roomScript = room.GetComponent<RoomScript>();

            if (roomScript == null)
            {
                continue;
            }

            if (roomScript.planetType == SystemManager.PlanetTypes.HIDDEN)
            {
                hiddenRooms++;
            }
            else if (roomScript.planetType == SystemManager.PlanetTypes.REWARD)
            {
                rewardRooms++;
            }

        }

        ItemManager.Instance.AddItemOnActivateOrder(this, this.itemName + " Activated! \nHidden Planets :" + hiddenRooms + "\nReward Planets:" + rewardRooms, false);

        activated = true;

    }

    public override void Initialiaze(ClassItemData classItem, CardScript cardScript)
    {
        activated = false;
    }


    public override void GameStart()
    {
        activated = false;
    }



}
