using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SystemManager;

[CreateAssetMenu(fileName = "Event_Button_Craft", menuName = "Events/ScriptableButtonEvent/Event_Button_Craft")]
public class Event_Button_Craft : ScriptableButtonEvent
{
    [TextArea(1, 20)]
    public string finalWording = "";

   

    public override void OnButtonClick(GameObject eventButton)
    {
        base.OnButtonClick(eventButton);

        MonoBehaviour runner = CombatCardHandler.Instance; // Ensure this is a valid MonoBehaviour in your scene
                                                           //hit at least one time if its 0

        // Start the coroutine for each hit
        runner.StartCoroutine(EventEnd());



    }

    public IEnumerator EventEnd()
    {

        MonoBehaviour runner = CombatCardHandler.Instance; // Ensure this is a valid MonoBehaviour in your scene

        //yield return runner.StartCoroutine(SystemManager.Instance.DestroyAllChildrenIE(UIManager.Instance.shopUI.transform.Find("CardList").gameObject));

        yield return runner.StartCoroutine(ForceOpenEvent());

        yield return null;
    }

    public IEnumerator ForceOpenEvent()
    {
        //open shop ui
        CraftingManager.Instance.OpenCraftingUI();

        yield return null;
    }



}
