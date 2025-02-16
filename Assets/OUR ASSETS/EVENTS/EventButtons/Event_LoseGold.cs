using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Event_LoseGold", menuName = "Events/ScriptableButtonEvent/Event_LoseGold")]
public class Event_LoseGold : ScriptableButtonEvent
{

    public override void OnButtonClick()
    {
        base.OnButtonClick();


        string description = "You lost your money...you are really pathetic!";

        //change event 
        UIManager.Instance.EndEvent(null,null,description);

        CustomDungeonGenerator.Instance.ActivatePlanet(CombatManager.Instance.planetClicked);

    }

}
