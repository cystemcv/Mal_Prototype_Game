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
                summon = Combat.Instance.InstantiateCharacter(summonInCard);

                summon.tag = "PlayerSummon";
            }
            else
            {
                summon = Combat.Instance.InstantiateEnemies(summonInCard);

                summon.tag = "EnemySummon";
            }

            //initialize the stats
            summon.GetComponent<EntityClass>().InititializeEntity();

            //initialize 
            summon.GetComponent<AIBrain>().GenerateIntend();

        }



    }




}
