using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Common_Card_Boomerang", menuName = "Card/Common/Common_Card_Boomerang")]
public class Common_Card_Boomerang : ScriptableCard
{

    public int damageAmount = 0;
    public int multiHits = 0;
    public int selfDamageAmount = 0;
    public int selfMultiHits = 0;

    public GameObject boomerangPrefab;

    private GameObject realTarget;
    private GameObject entityUsedCardGlobal;

    public override string OnCardDescription(CardScript cardScript, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScript, entityUsedCard);

        int calculatedDamage = (Combat.Instance == null) ? damageAmount : Combat.Instance.CalculateEntityDmg(damageAmount, entityUsedCard, realTarget);
        int selfCalculatedDamage = (Combat.Instance == null) ? selfDamageAmount : Combat.Instance.CalculateEntityDmg(selfDamageAmount, entityUsedCard, realTarget);
        customDesc += multiHits + "X" +  " Deal " + DeckManager.Instance.GetCalculatedValueString(damageAmount, calculatedDamage) + " to all enemies<br>";
        customDesc += selfMultiHits + "X" + " Deal " + DeckManager.Instance.GetCalculatedValueString(selfDamageAmount, selfCalculatedDamage) + " to all allies";

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


        
        // Start the coroutine for each hit
        runner.StartCoroutine(ExecuteCoroutine());

    }

    public IEnumerator ExecuteCoroutine()
    {

        Animator entityAnimator = entityUsedCardGlobal.transform.Find("model").GetComponent<Animator>();

        if (entityAnimator != null)
        {
            entityAnimator.SetTrigger(this.entityAnimation.ToString());
        }

        List<GameObject> targets = AIManager.Instance.GetAllTargets(entityUsedCardGlobal);
        yield return DealDmg(this, damageAmount, entityUsedCardGlobal, targets, 1, 2);

        yield return new WaitForSeconds(0.5f);

        yield return DealDmg(this, damageAmount, entityUsedCardGlobal, targets, 1, 2);

        yield return new WaitForSeconds(0.5f);

        targets.Clear();
        targets = AIManager.Instance.GetAllAllies(entityUsedCardGlobal);
        yield return DealDmg(this, damageAmount, entityUsedCardGlobal, targets, 1, 2);

        yield return new WaitForSeconds(0.5f);
    }

    public IEnumerator DealDmg(ScriptableCard scriptableCard, int damageAmount, GameObject entityUsedCardGlobal, List<GameObject> realTargets, int multiHits, float multiHitDuration = 2, bool pierce = false)
    {

        foreach (GameObject realTarget in realTargets)
        {
            //if target dies during multi attack then stop
            if (realTarget == null)
            {
                break;
            }

            int calculatedDmg = (Combat.Instance == null) ? damageAmount : Combat.Instance.CalculateEntityDmg(damageAmount, entityUsedCardGlobal, realTarget);

            CombatManager.Instance.SpawnEffectPrefab(realTarget, scriptableCard);


            Combat.Instance.AdjustTargetHealth(entityUsedCardGlobal, realTarget, calculatedDmg, false, SystemManager.AdjustNumberModes.ATTACK);

            if (scriptableCard.cardSoundEffect != null)
            {
                AudioManager.Instance.cardSource.PlayOneShot(scriptableCard.cardSoundEffect);
            }
        }

        yield return null;

    }


    }
