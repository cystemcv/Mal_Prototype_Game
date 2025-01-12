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

        Combat.Instance.SummonEntity(entityUsedCard, scriptableEntities);

    }

    public override void OnAiPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        base.OnAiPlayCard(cardScript, entityUsedCard);

        Combat.Instance.SummonEntity(entityUsedCard, scriptableEntities);
    }




}
