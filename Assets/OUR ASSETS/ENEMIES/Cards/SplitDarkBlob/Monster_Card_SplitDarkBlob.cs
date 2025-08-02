using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster_Card_SplitDarkBlob", menuName = "Card/Monster/Monster_Card_SplitDarkBlob")]
public class Monster_Card_SplitDarkBlob : ScriptableCard
{


    private GameObject realTarget;

    public List<ScriptableEntity> scriptableEntities;

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        customDesc += "Half HP to Summon " + scriptableEntities.Count + "X " + scriptableEntities[0].entityName;

        return customDesc;
    }

    public override void OnPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnPlayCard(cardScriptData, entityUsedCard);


        EntityClass entityUsedCardClass = entityUsedCard.GetComponent<EntityClass>();
        //half the hp of the current entity then give the hp to the other summons
        int hp = entityUsedCardClass.health / 2;
        entityUsedCardClass.health = hp;

        //update text on hp
        entityUsedCardClass.healthText.GetComponent<TMP_Text>().text = hp + "/" + entityUsedCardClass.maxHealth;

        //adjust the hp bar
        UI_Combat.Instance.UpdateHealthBarSmoothly(entityUsedCardClass.health, entityUsedCardClass.maxHealth, entityUsedCardClass.slider);




        MonoBehaviour runner = CombatCardHandler.Instance; // Ensure this is a valid MonoBehaviour in your scene
                                                           //hit at least one time if its 0



        runner.StartCoroutine(Combat.Instance.SummonEntity(entityUsedCard, Combat.Instance.ScriptableToEntityClass(scriptableEntities)));
    }

    public override void OnAiPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayCard(cardScriptData, entityUsedCard);

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

        MonoBehaviour runner = CombatCardHandler.Instance; // Ensure this is a valid MonoBehaviour in your scene
                                                           //hit at least one time if its 0

        runner.StartCoroutine(Combat.Instance.SummonEntity(entityUsedCard, Combat.Instance.ScriptableToEntityClass(scriptableEntities)));



    }



}
