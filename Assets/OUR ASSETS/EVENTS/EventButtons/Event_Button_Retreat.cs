using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Event_Button_Retreat", menuName = "Events/ScriptableButtonEvent/Event_Button_Retreat")]
public class Event_LoseGold : ScriptableButtonEvent
{
    [TextArea(1, 20)]
    public string descriptionAfterClick = "";
    public override void OnButtonClick()
    {
        base.OnButtonClick();

        //change event 
        UIManager.Instance.EndEvent(null,null, descriptionAfterClick);

    
    }

}
