using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Event_Button_ForceOpen", menuName = "Events/ScriptableButtonEvent/Event_Button_ForceOpen")]
public class Event_Button_ForceOpen : ScriptableButtonEvent
{
    [TextArea(1, 20)]
    public string finalWording = "";

    MonoBehaviour runner;

    public override void OnButtonClick(GameObject eventButton)
    {
        base.OnButtonClick(eventButton);

         runner = CombatCardHandler.Instance; // Ensure this is a valid MonoBehaviour in your scene
                                                           //hit at least one time if its 0

        // Start the coroutine for each hit
        runner.StartCoroutine(EventEnd());


    
    }

    public IEnumerator EventEnd()
    {

        //MonoBehaviour runner = CombatCardHandler.Instance; // Ensure this is a valid MonoBehaviour in your scene

        yield return runner.StartCoroutine(ForceOpenEvent());

        //yield return runner.StartCoroutine(UIManager.Instance.EndEvent(finalWording));

        yield return null;
    }

    public IEnumerator ForceOpenEvent()
    {
        //fight mimic
        UIManager.Instance.HideEventGo();

        int random = UnityEngine.Random.Range(0, 3);

        if (random == 0)
        {
            finalWording = "Good!";
            yield return runner.StartCoroutine(UIManager.Instance.EndEvent(finalWording));
            //0 = Good
            yield return runner.StartCoroutine(LootShow(random));
        }
        else if (random == 1)
        {
            finalWording = "Bad!";
            yield return runner.StartCoroutine(UIManager.Instance.EndEvent(finalWording));
            //1 = Bad
            yield return runner.StartCoroutine(LootShow(random));
        }
        else
        {
            finalWording = "Fight!";
            yield return runner.StartCoroutine(UIManager.Instance.EndEvent(finalWording));
            Combat.Instance.StartCombat("EVENT");
        }


        yield return null;
    }

    public IEnumerator LootShow(int random)
    {
        GameObject playerCharacter = GameObject.FindGameObjectWithTag("Player");
        EntityClass entityClass = playerCharacter.GetComponent<EntityClass>();

        //save character
        StaticData.staticCharacter.maxHealth = entityClass.maxHealth;
        StaticData.staticCharacter.currHealth = entityClass.health;

        StaticData.lootItemList.Clear();

        Combat.Instance.CalculateLootBoxRewards(random);

        ItemManager.Instance.ShowLoot();

        yield return null;
    }

}
