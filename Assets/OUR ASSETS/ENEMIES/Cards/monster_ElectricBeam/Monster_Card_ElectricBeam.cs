using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster_Card_ElectricBeam", menuName = "Card/Monster/Monster_Card_ElectricBeam")]
public class Monster_Card_ElectricBeam : ScriptableCard
{

    public int damageAmount = 0;
    public int multiHits = 0;
    public int extraDmg = 0;
    public ScriptableBuffDebuff wet;

    private GameObject realTarget;
    private GameObject entityUsedCardGlobal;

    private int scalingDmg = 0;


    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        //scaling
        scalingDmg = damageAmount + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        int calculatedDamage = (Combat.Instance == null) ? scalingDmg : Combat.Instance.CalculateEntityDmg(scalingDmg, entityUsedCard, realTarget);
        customDesc += "Deal " + DeckManager.Instance.GetCalculatedValueString(scalingDmg, calculatedDamage) + " to all enemies.";
        customDesc += "(+" + extraDmg + " dmg if " + BuffSystemManager.Instance.GetBuffDebuffColor(wet) + " exist. Remove 2 stacks)";

        return customDesc;
    }

    public override void OnPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnPlayCard(cardScriptData, entityUsedCard);

        realTarget = CombatCardHandler.Instance.targetClicked;
        entityUsedCardGlobal = entityUsedCard;

        ExecuteCard(cardScriptData);

    }

    public override void OnAiPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayCard(cardScriptData, entityUsedCard);

        realTarget = AIManager.Instance.GetRandomTarget(entityUsedCard);
        entityUsedCardGlobal = entityUsedCard;

        ExecuteCard(cardScriptData);
    }

    public void ExecuteCard(CardScriptData cardScriptData)
    {

        //then loop
        if (multiHits <= 0)
        {
            multiHits = 1;
        }

        MonoBehaviour runner = CombatCardHandler.Instance;

        List<GameObject> targets = AIManager.Instance.GetAllTargets(entityUsedCardGlobal);

        //scaling
        scalingDmg = damageAmount + (scalingLevelCardValue * cardScriptData.scalingLevelValue);
        int finalDmg = scalingDmg;
        foreach (GameObject target in targets)
        {
            BuffDebuffClass buffDebuffClass = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(target, wet.nameID);
            if (!SystemManager.Instance.CheckNullMonobehavior(buffDebuffClass))
            {
                finalDmg = scalingDmg + extraDmg;
                //decrease by 1
                BuffSystemManager.Instance.DecreaseValueTargetBuffDebuff(target, wet.nameID, -2);
            }

            // Start the coroutine for each hit
            runner.StartCoroutine(Combat.Instance.AttackSingleTargetEnemy(this, finalDmg, 1, entityUsedCardGlobal, target, multiHits, this.abilityEffectLifetime));
        }

  

    }

}
