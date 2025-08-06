using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Angel_Card_RighteousStrike", menuName = "Card/Angel/Angel_Card_RighteousStrike")]
public class Angel_Card_RighteousStrike : ScriptableCard
{
    private GameObject realTarget;
    public ScriptableBuffDebuff righteous;

    public int originalDmg = 5;

    private int scalingDmg = 0;

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        BuffDebuffClass buffDebuffClass = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(entityUsedCard, righteous.nameID);

        int dmg =  0;

        //scaling
        scalingDmg = originalDmg + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        if (buffDebuffClass != null)
        {
            dmg = buffDebuffClass.tempValue + scalingDmg;
        }
        else
        {
            dmg = scalingDmg;
        }

        int calculatedDamage = (Combat.Instance == null) ? dmg : Combat.Instance.CalculateEntityDmg(dmg, entityUsedCard, realTarget);

        customDesc += "Deal " + DeckManager.Instance.GetCalculatedValueString(scalingDmg, calculatedDamage) + "\nDmg Increase from " + BuffSystemManager.Instance.GetBuffDebuffColor(righteous) + " stacks";

        return customDesc;
    }

    public override void OnPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnPlayCard(cardScriptData, entityUsedCard);
        realTarget = CombatCardHandler.Instance.targetClicked;
        Activate(cardScriptData, entityUsedCard);
    }

    public override void OnAiPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayCard(cardScriptData, entityUsedCard);
        realTarget = entityUsedCard.GetComponent<AIBrain>().targetForCard;
        Activate(cardScriptData, entityUsedCard);
    }

    public void Activate(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
  
        MonoBehaviour runner = CombatCardHandler.Instance;


        BuffDebuffClass buffDebuffClass = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(entityUsedCard, righteous.nameID);

        int dmg = 0;

        //scaling
        scalingDmg = originalDmg + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        if (buffDebuffClass != null)
        {
            dmg = buffDebuffClass.tempValue + scalingDmg;
        }
        else
        {
            dmg = scalingDmg;
        }

        int calculatedDamage = (Combat.Instance == null) ? dmg : Combat.Instance.CalculateEntityDmg(dmg, entityUsedCard, realTarget);

        runner.StartCoroutine(Combat.Instance.AttackSingleTargetEnemy(this, calculatedDamage, 1, entityUsedCard, realTarget, 1, 1));
    }

    public override void OnAiPlayTarget(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayTarget(cardScriptData, entityUsedCard);
        realTarget = AIManager.Instance.GetRandomTarget(entityUsedCard);
        entityUsedCard.GetComponent<AIBrain>().targetForCard = realTarget;
    }

}
