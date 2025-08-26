using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster_Card_EvilFlame", menuName = "Card/Monster/Monster_Card_EvilFlame")]
public class Monster_Card_EvilFlame : ScriptableCard
{

    public int damageAmount = 0;
    public int multiHits = 0;
    public int buffValue = 0;

    public ScriptableBuffDebuff burn;

    private GameObject realTarget;
    private GameObject entityUsedCardGlobal;

    private int scalingDmg = 0;

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        //scaling
        scalingDmg = damageAmount + (scalingLevelCardValue * cardScriptData.scalingLevelValue);

        int calculatedDamage = (Combat.Instance == null) ? scalingDmg : Combat.Instance.CalculateEntityDmg(scalingDmg, entityUsedCard, realTarget);
        customDesc += multiHits + "X" + " Deal " + DeckManager.Instance.GetCalculatedValueString(scalingDmg, calculatedDamage) + " to all enemies";
        customDesc += "<br>Add 1 " + BuffSystemManager.Instance.GetBuffDebuffColor(burn) + " to each enemy";

        return customDesc;
    }

    public override void OnPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnPlayCard(cardScriptData, entityUsedCard);

        entityUsedCardGlobal = entityUsedCard;

        ExecuteCard(cardScriptData);

    }

    public override void OnAiPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayCard(cardScriptData, entityUsedCard);

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

        // Start the coroutine for each hit
        runner.StartCoroutine(Combat.Instance.AttackAllEnemy(this, scalingDmg, entityUsedCardGlobal, targets, multiHits, 2));

        foreach (GameObject target in targets)
        {
            runner.StartCoroutine(BuffSystemManager.Instance.AddBuffDebuffIE(target, burn, buffValue));

        }
     

    }

}
