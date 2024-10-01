using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Ability_DamageRandomTarget", menuName = "CardAbility/Ability_DamageRandomTarget")]
public class Ability_DamageRandomTarget : ScriptableCardAbility
{

    [Header("UNIQUE")]
    private GameObject targetFound;


    public override string AbilityDescription(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity)
    {
        int cardDmg = DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList);
        int calculatedDmg = Combat.Instance.CalculateEntityDmg(DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList), entity, null);

        string keyword = base.AbilityDescription(cardScript, cardAbilityClass, entity);
        string description = "Deal " + cardDmg + DeckManager.Instance.GetBonusAttackAsDescription(cardDmg, calculatedDmg) + " to a random enemy";
        string final = keyword + " : " + description;

        return final;
    }




    public override void OnPlayCard(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity, SystemManager.ControlBy controlBy)
    {
        //get all targets
        GameObject[] targetsFound;


        if (entity.tag == "Player")
        {
            targetsFound = GameObject.FindGameObjectsWithTag("Enemy");
        }
        else
        {
            targetsFound = GameObject.FindGameObjectsWithTag("Player");
        }

        List<GameObject> targetsList = new List<GameObject>();

        //remove dead
        foreach (GameObject targetF in targetsFound)
        {
            if (targetF.GetComponent<EntityClass>().entityMode != SystemManager.EntityMode.DEAD)
            {
                targetsList.Add(targetF);
            }
        }

        if (targetsList.Count == 0)
        {
            return;
        }


        int randomNmbr = Random.Range(0, targetsList.Count);

        targetFound = targetsList[randomNmbr];

        base.OnPlayCard(cardScript, cardAbilityClass, entity, controlBy);

        ProceedToAbility(cardScript, cardAbilityClass, entity);




    }


    private void ProceedToAbility(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity)
    {

        //spawn prefab
        base.SpawnEffectPrefab(targetFound, cardAbilityClass);

        int calculatedDmg = Combat.Instance.CalculateEntityDmg(DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList), entity, targetFound);
        Combat.Instance.AdjustTargetHealth(targetFound, calculatedDmg, false, SystemManager.AdjustNumberModes.ATTACK);

    }


}
