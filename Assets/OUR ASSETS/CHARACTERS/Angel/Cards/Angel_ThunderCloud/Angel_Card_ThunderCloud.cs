using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Angel_Card_ThunderCloud", menuName = "Card/Angel/Angel_Card_ThunderCloud")]
public class Angel_Card_ThunderCloud : ScriptableCard
{

    public int cardDmg = 0;

    private GameObject realTarget;
    private GameObject entityUsedCardGlobal;


    public override string OnCardDescription(CardScript cardScript, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScript, entityUsedCard);
        int calculatedDmg = Combat.Instance.CalculateEntityDmg(cardDmg, entityUsedCard, null);

        customDesc += "Deal " + DeckManager.Instance.GetCalculatedValueString(cardDmg, calculatedDmg) + " to an enemy for every <color=yellow>Wet</color>. Remove each " +
            "<color=yellow>Wet</color>";
        return customDesc;
    }

    public override void OnPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        realTarget = CombatCardHandler.Instance.targetClicked;

        base.OnPlayCard(cardScript, entityUsedCard);


        ////allow to activate coroutine on scriptable object
        //MonoBehaviour runner = CombatCardHandler.Instance; // Ensure this is a valid MonoBehaviour in your scene
        //                                                   //hit at least one time if its 0

        //// Start the coroutine for each hit
        //runner.StartCoroutine(ExecuteMultiHits());


    }

    public override void OnAiPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        realTarget = AIManager.Instance.GetRandomAlly(entityUsedCard); ;

        base.OnPlayCard(cardScript, entityUsedCard);

        ////allow to activate coroutine on scriptable object
        //MonoBehaviour runner = CombatCardHandler.Instance; // Ensure this is a valid MonoBehaviour in your scene
        //                                                   //hit at least one time if its 0

        //// Start the coroutine for each hit
        //runner.StartCoroutine(ExecuteMultiHits());
    }



    //private IEnumerator ExecuteMultiHits()
    //{

    //    bool hitWetTarget = false;


    //    //check if buff exist on target
    //    BuffDebuffClass buffDebuffClass = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(realTarget, scriptableBuffDebuff.nameID);

    //    if (buffDebuffClass != null)
    //    {
    //        hitWetTarget = true;
    //    }

    //    // Use ability on each hit
    //    base.OnPlayCard(cardScript, cardAbilityClass, entityUsedCard, controlBy);

    //    base.SpawnEffectPrefab(realTarget, cardAbilityClass);

    //    int calculatedDmg = Combat.Instance.CalculateEntityDmg(DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList), entityUsedCard, realTarget);
    //    Combat.Instance.AdjustTargetHealth(entityUsedCard, realTarget, calculatedDmg, false, SystemManager.AdjustNumberModes.ATTACK);

    //    // Wait between hits
    //    yield return new WaitForSeconds(cardAbilityClass.waitForAbility);


    //    if (hitWetTarget)
    //    {

    //        //then go throught all other enemies with debuff and strike them too

    //        // Use ability on each hit
    //        base.OnPlayCard(cardScript, cardAbilityClass, entityUsedCard, controlBy);
    //        List<GameObject> targetsFound = new List<GameObject>();

    //        //get all targets
    //        if (SystemManager.Instance.GetPlayerTagsList().Contains(this.entityUsedCard.tag))
    //        {
    //            targetsFound = SystemManager.Instance.FindGameObjectsWithTags(SystemManager.Instance.GetEnemyTagsList());
    //        }
    //        else
    //        {
    //            targetsFound = SystemManager.Instance.FindGameObjectsWithTags(SystemManager.Instance.GetPlayerTagsList());
    //        }

    //        //then loop
    //        foreach (GameObject targetFound in targetsFound)
    //        {
    //            if (targetFound == null)
    //            {
    //                continue;
    //            }


    //            if (targetFound.GetComponent<EntityClass>().entityMode != SystemManager.EntityMode.DEAD)
    //            {


    //                BuffDebuffClass buffDebuff = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(targetFound, scriptableBuffDebuff.nameID);

    //                if (buffDebuff != null)
    //                {

    //                    int count = buffDebuff.tempVariable;

    //                    while (count > 0 && targetFound != null)
    //                    {

    //                        //spawn prefab
    //                        base.SpawnEffectPrefab(targetFound, cardAbilityClass);


    //                        calculatedDmg = Combat.Instance.CalculateEntityDmg(DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList), entityUsedCard, targetFound);
    //                        Combat.Instance.AdjustTargetHealth(entityUsedCard, targetFound, calculatedDmg, false, SystemManager.AdjustNumberModes.ATTACK);

    //                        count = count - 1;
    //                        //remove the debuff
    //                        BuffSystemManager.Instance.DecreaseValueTargetBuffDebuff(targetFound, scriptableBuffDebuff.nameID, -1);

    //                        // Wait between hits
    //                        yield return new WaitForSeconds(cardAbilityClass.waitForAbility);

    //                    }


    //                }


    //            }






    //        }



    //    }
    //}





}
