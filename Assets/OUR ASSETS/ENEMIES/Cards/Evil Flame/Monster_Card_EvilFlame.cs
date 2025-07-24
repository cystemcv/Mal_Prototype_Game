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

    public override string OnCardDescription(CardScript cardScript, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScript, entityUsedCard);

        int calculatedDamage = (Combat.Instance == null) ? damageAmount : Combat.Instance.CalculateEntityDmg(damageAmount, entityUsedCard, realTarget);
        customDesc += multiHits + "X" + " Deal " + DeckManager.Instance.GetCalculatedValueString(damageAmount, calculatedDamage) + " to all enemies";
        customDesc += "<br>Add 1 " + BuffSystemManager.Instance.GetBuffDebuffColor(burn) + " to each enemy";

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

        List<GameObject> targets = AIManager.Instance.GetAllTargets(entityUsedCardGlobal);

        // Start the coroutine for each hit
        runner.StartCoroutine(Combat.Instance.AttackAllEnemy(this, damageAmount, entityUsedCardGlobal, targets, multiHits, 2));

        foreach (GameObject target in targets)
        {
            runner.StartCoroutine(BuffSystemManager.Instance.AddBuffDebuffIE(target, burn, buffValue));

        }
     

    }

}
