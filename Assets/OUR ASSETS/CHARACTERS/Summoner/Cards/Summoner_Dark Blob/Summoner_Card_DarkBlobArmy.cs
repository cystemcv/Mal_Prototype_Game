using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Summoner_Card_DarkBlobArmy", menuName = "Card/Summoner/Summoner_Card_DarkBlobArmy")]
public class Summoner_Card_DarkBlobArmy : ScriptableCard
{


    private GameObject realTarget;

    public List<ScriptableEntity> scriptableEntities;

    public override string OnCardDescription(CardScript cardScript, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScript, entityUsedCard);

        customDesc += "Summon " + scriptableEntities.Count + "X " + scriptableEntities[0].entityName;

        return customDesc;
    }

    public override void OnPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        base.OnPlayCard(cardScript, entityUsedCard);

        MonoBehaviour runner = CombatCardHandler.Instance; // Ensure this is a valid MonoBehaviour in your scene
                                                           //hit at least one time if its 0

        runner.StartCoroutine(Combat.Instance.SummonEntity(entityUsedCard, Combat.Instance.ScriptableToEntityClass(scriptableEntities)));

    }

    public override void OnAiPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        base.OnAiPlayCard(cardScript, entityUsedCard);

        MonoBehaviour runner = CombatCardHandler.Instance; // Ensure this is a valid MonoBehaviour in your scene
                                                           //hit at least one time if its 0

        runner.StartCoroutine(Combat.Instance.SummonEntity(entityUsedCard, Combat.Instance.ScriptableToEntityClass(scriptableEntities)));
    }




}
