using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Event_Button_GainCard", menuName = "Events/ScriptableButtonEvent/Event_Button_GainCard")]
public class Event_Button_GainCard : ScriptableButtonEvent
{

    public ScriptableCard scriptableCard;
    [TextArea(1, 20)]
    public string finalWording = "";

    public override void OnButtonClick(GameObject eventButton)
    {
        base.OnButtonClick(eventButton);

        

        //string description = "'My power is yours, use it wisely!' ." +
        //    "<br><color=yellow>" + scriptableCard.cardName + "</color> is added to the deck!";


        DeckManager.Instance.AddCardOnDeck(scriptableCard,0);


        MonoBehaviour runner = CombatCardHandler.Instance; // Ensure this is a valid MonoBehaviour in your scene
                                                           //hit at least one time if its 0

        // Start the coroutine for each hit
        runner.StartCoroutine(EventEnd());

        //CustomDungeonGenerator.Instance.ActivatePlanet(CombatManager.Instance.planetClicked);

    }

    public IEnumerator EventEnd()
    {

        MonoBehaviour runner = CombatCardHandler.Instance; // Ensure this is a valid MonoBehaviour in your scene

        yield return runner.StartCoroutine(UIManager.Instance.EndEvent(finalWording));

        yield return null;
    }


}
