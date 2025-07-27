using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster_Card_TwinLasers", menuName = "Card/Monster/Monster_Card_TwinLasers")]
public class Monster_Card_TwinLasers : ScriptableCard
{

    public int damageAmount = 0;
    public int multiHits = 0;


    private GameObject realTarget;
    private GameObject entityUsedCardGlobal;

    public override string OnCardDescription(CardScript cardScript, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScript, entityUsedCard);

        int calculatedDamage = (Combat.Instance == null) ? damageAmount : Combat.Instance.CalculateEntityDmg(damageAmount, entityUsedCard, realTarget);
        customDesc += "Deal " + DeckManager.Instance.GetCalculatedValueString(damageAmount, calculatedDamage) + " to targets";
   
        return customDesc;
    }

    public override void OnPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        //base.OnPlayCard(cardScript, entityUsedCard);

        realTarget = CombatCardHandler.Instance.targetClicked;
        entityUsedCardGlobal = entityUsedCard;

        ExecuteCard();

    }

    public override void OnAiPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        //base.OnAiPlayCard(cardScript, entityUsedCard);

        realTarget = AIManager.Instance.GetRandomTarget(entityUsedCard);
        CombatCardHandler.Instance.posClickedTargeting = Combat.Instance.CheckCardTargets(realTarget, cardScript.scriptableCard);
        entityUsedCardGlobal = entityUsedCard;

        ExecuteCard();
    }

    public void ExecuteCard()
    {

        //then loop
        if (multiHits <= 0)
        {
            multiHits = 1;
        }

        MonoBehaviour runner = CombatCardHandler.Instance;

        List<CombatPosition> targets = CombatCardHandler.Instance.posClickedTargeting;

        // Start the coroutine for each hit
      

        foreach (CombatPosition target in targets)
        {
            if(target.entityOccupiedPos != null)
            {
                runner.StartCoroutine(Combat.Instance.AttackSingleTargetEnemy(this, damageAmount, 1, entityUsedCardGlobal, target.entityOccupiedPos, multiHits, 2));
                //runner.StartCoroutine(BuffSystemManager.Instance.AddBuffDebuffIE(target, burn, buffValue));
            }
        }
     

    }

}
