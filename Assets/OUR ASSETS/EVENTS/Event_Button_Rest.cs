using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SystemManager;

[CreateAssetMenu(fileName = "Event_Button_Rest", menuName = "Events/ScriptableButtonEvent/Event_Button_Rest")]
public class Event_Button_Rest : ScriptableButtonEvent
{
    [TextArea(1, 20)]
    public string finalWording = "";

   
    public override void OnButtonCreate(GameObject eventButton)
    {

        if (CombatManager.Instance.planetClicked.GetComponent<RoomScript>().usedRest)
        {
            eventButton.GetComponent<ButtonManager>().Interactable(false);
        }

    }

    public override void OnButtonClick(GameObject eventButton)
    {




        base.OnButtonClick(eventButton);



        MonoBehaviour runner = CombatCardHandler.Instance; // Ensure this is a valid MonoBehaviour in your scene
                                                           //hit at least one time if its 0

        if (CombatManager.Instance.planetClicked.GetComponent<RoomScript>().usedRest)
        {
            runner.StartCoroutine(UI_Combat.Instance.OnNotification("ALREADY USED!", 1));
            return;
        }

        // Start the coroutine for each hit
        runner.StartCoroutine(EventEnd(eventButton));



    }

    public IEnumerator EventEnd(GameObject eventButton)
    {

        MonoBehaviour runner = CombatCardHandler.Instance; // Ensure this is a valid MonoBehaviour in your scene

        GameObject character = GameObject.FindGameObjectWithTag("Player");

        float ratio = CharacterManager.Instance.restHealing / 100;
        int hpToHeal = (int)(character.GetComponent<EntityClass>().scriptableEntity.maxHealth * ratio);

        yield return runner.StartCoroutine(Combat.Instance.AdjustTargetHealth(null, character, hpToHeal, false, SystemManager.AdjustNumberModes.HEAL));

        CombatManager.Instance.planetClicked.GetComponent<RoomScript>().usedRest = true;

        eventButton.GetComponent<ButtonManager>().Interactable(false);

        yield return null;
    }






}
