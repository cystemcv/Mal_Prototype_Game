using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Ability_Summon", menuName = "CardAbility/Ability_Summon")]
public class Ability_Summon : ScriptableCardAbility
{

    public override string AbilityDescription(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity)
    {
        string keyword = base.AbilityDescription(cardScript, cardAbilityClass, entity);
        string description = "";

        description += "Summon ";

        foreach (ScriptableEntity summonInCard in cardAbilityClass.summonList)
        {
            description += summonInCard.entityName;
        }

        string final = keyword + " : " + description;

        return final;
    }



    public override void OnPlayCard(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entityUsedCard, SystemManager.ControlBy controlBy)
    {

        base.OnPlayCard(cardScript, cardAbilityClass, entityUsedCard, controlBy);

        foreach (ScriptableEntity summonInCard in cardAbilityClass.summonList)
        {


            GameObject summon;
            //get all targets
            if (SystemManager.Instance.GetPlayerTagsList().Contains(entityUsedCard.tag))
            {

                GameObject[] summons = GameObject.FindGameObjectsWithTag("PlayerSummon");

                //check if it reach the limit
                if (summons.Length >= Combat.Instance.maxPlayerSummons)
                {
                    return;
                }
                else
                {
                    summon = Combat.Instance.InstantiateCharacter(summonInCard);

                    summon.tag = "PlayerSummon";

                    //allow to activate coroutine on scriptable object
                    MonoBehaviour runner = CombatCardHandler.Instance; // Ensure this is a valid MonoBehaviour in your scene
                                                                       //hit at least one time if its 0

                    // Start the coroutine for each hit
                    runner.StartCoroutine(summon.GetComponent<EntityClass>().InititializeEntity());


                    //initialize 
                    summon.GetComponent<AIBrain>().GenerateIntend();
                }


            }
            else
            {

                GameObject[] summons = GameObject.FindGameObjectsWithTag("EnemySummon");

                //check if it reach the limit
                if (summons.Length >= Combat.Instance.maxEnemySummons)
                {
                    return;
                }
                else
                {
                    summon = Combat.Instance.InstantiateEnemies(summonInCard);

                    summon.tag = "EnemySummon";

                    //initialize the stats
                    //allow to activate coroutine on scriptable object
                    MonoBehaviour runner = CombatCardHandler.Instance; // Ensure this is a valid MonoBehaviour in your scene
                                                                       //hit at least one time if its 0

                    // Start the coroutine for each hit
                    runner.StartCoroutine(summon.GetComponent<EntityClass>().InititializeEntity());


                    //initialize 
                    summon.GetComponent<AIBrain>().GenerateIntend();
                }

     
            }



        }



    }




}
