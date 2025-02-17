using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Event_Button_GainCard", menuName = "Events/ScriptableButtonEvent/Event_Button_GainCard")]
public class Event_Button_GainCard : ScriptableButtonEvent
{

    public ScriptableCard scriptableCard;
    [TextArea(1, 20)]
    public string descriptionAfterClick = "";

    public override void OnButtonClick()
    {
        base.OnButtonClick();

        

        //string description = "'My power is yours, use it wisely!' ." +
        //    "<br><color=yellow>" + scriptableCard.cardName + "</color> is added to the deck!";


        DeckManager.Instance.AddCardOnDeck(scriptableCard,0);


        //change event 
        UIManager.Instance.EndEvent(null,null, descriptionAfterClick);

        //CustomDungeonGenerator.Instance.ActivatePlanet(CombatManager.Instance.planetClicked);

    }

}
