using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster_Card_SplitDarkBlob", menuName = "Card/Monster/Monster_Card_SplitDarkBlob")]
public class Monster_Card_SplitDarkBlob : ScriptableCard
{


    private GameObject realTarget;

    public List<ScriptableEntity> scriptableEntities;

    public override string OnCardDescription(CardScript cardScript, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScript, entityUsedCard);

        customDesc += "Half HP to Summon " + scriptableEntities.Count + "X " + scriptableEntities[0].entityName;

        return customDesc;
    }

    public override void OnPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        base.OnPlayCard(cardScript, entityUsedCard);


        EntityClass entityUsedCardClass = entityUsedCard.GetComponent<EntityClass>();
        //half the hp of the current entity then give the hp to the other summons
        int hp = entityUsedCardClass.health / 2;
        entityUsedCardClass.health = hp;

        //update text on hp
        entityUsedCardClass.healthText.GetComponent<TMP_Text>().text = hp + "/" + entityUsedCardClass.maxHealth;

        //adjust the hp bar
        UI_Combat.Instance.UpdateHealthBarSmoothly(entityUsedCardClass.health, entityUsedCardClass.maxHealth, entityUsedCardClass.slider);

        EntityCustomClass entityCustomClass = new EntityCustomClass();
        entityCustomClass.currHealth = hp;
        entityCustomClass.maxHealth = hp;

        Combat.Instance.SummonEntity(entityUsedCard, scriptableEntities, entityCustomClass);
    }

    public override void OnAiPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        base.OnAiPlayCard(cardScript, entityUsedCard);

        EntityClass entityUsedCardClass = entityUsedCard.GetComponent<EntityClass>();
        //half the hp of the current entity then give the hp to the other summons
        int hp = entityUsedCardClass.health / 2;
        entityUsedCardClass.health = hp;

        //update text on hp
        entityUsedCardClass.healthText.GetComponent<TMP_Text>().text = hp + "/" + entityUsedCardClass.maxHealth;

        //adjust the hp bar
        UI_Combat.Instance.UpdateHealthBarSmoothly(entityUsedCardClass.health, entityUsedCardClass.maxHealth, entityUsedCardClass.slider);

        EntityCustomClass entityCustomClass = new EntityCustomClass();
        entityCustomClass.currHealth = hp;
        entityCustomClass.maxHealth = hp;

        Combat.Instance.SummonEntity(entityUsedCard, scriptableEntities, entityCustomClass);



    }



}
