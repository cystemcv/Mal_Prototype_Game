using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_EventButton : MonoBehaviour
{

    public ScriptableButtonEvent scriptableButtonEvent;


    public void ClickedEventButton()
    {
        

        scriptableButtonEvent.OnButtonClick(this.gameObject);

    }

    public void CloseEvent()
    {

        UIManager.Instance.HideEventGo();

        CustomDungeonGenerator.Instance.ActivatePlanet(CombatManager.Instance.planetClicked);

    }

}
