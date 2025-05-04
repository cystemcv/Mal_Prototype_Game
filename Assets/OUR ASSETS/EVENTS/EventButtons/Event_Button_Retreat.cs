using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Event_Button_Retreat", menuName = "Events/ScriptableButtonEvent/Event_Button_Retreat")]
public class Event_Button_Retreat : ScriptableButtonEvent
{
    [TextArea(1, 20)]
    public string finalWording = "";
    public override void OnButtonClick()
    {
        base.OnButtonClick();

        MonoBehaviour runner = CombatCardHandler.Instance; // Ensure this is a valid MonoBehaviour in your scene
                                                           //hit at least one time if its 0

        // Start the coroutine for each hit
        runner.StartCoroutine(EventEnd());


    
    }

    public IEnumerator EventEnd()
    {

        MonoBehaviour runner = CombatCardHandler.Instance; // Ensure this is a valid MonoBehaviour in your scene

        yield return runner.StartCoroutine(UIManager.Instance.EndEvent(finalWording));

        yield return null;
    }

}
