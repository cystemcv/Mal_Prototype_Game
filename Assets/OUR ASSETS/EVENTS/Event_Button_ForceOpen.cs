using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Event_Button_ForceOpen", menuName = "Events/ScriptableButtonEvent/Event_Button_ForceOpen")]
public class Event_Button_ForceOpen : ScriptableButtonEvent
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
            //0 = Good
            LootShow(random);
        }
        else if (random == 1)
        {
            //1 = Bad
            LootShow(random);
        }
        else
        {
            Combat.Instance.StartCombat("EVENT");
        }


        yield return null;
    }

    public void LootShow(int random)
    {
        GameObject playerCharacter = GameObject.FindGameObjectWithTag("Player");
        EntityClass entityClass = playerCharacter.GetComponent<EntityClass>();

        //save character
        StaticData.staticCharacter.maxHealth = entityClass.maxHealth;
        StaticData.staticCharacter.currHealth = entityClass.health;

        StaticData.lootItemList.Clear();

        Combat.Instance.CalculateLootBoxRewards(random);

        ItemManager.Instance.ShowLoot();
    }

}
