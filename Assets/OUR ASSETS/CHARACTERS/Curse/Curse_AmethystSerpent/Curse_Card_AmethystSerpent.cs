using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Curse_Card_AmethystSerpent", menuName = "Card/Curse/Curse_Card_AmethystSerpent")]
public class Curse_Card_AmethystSerpent : ScriptableCard
{

    public int damageAmount = 0;
    public int healAmount = 0;

    private GameObject realTarget;
    private GameObject entityUsedCardGlobal;

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        int calculatedDamage = (Combat.Instance == null) ? damageAmount : Combat.Instance.CalculateEntityDmg(damageAmount, entityUsedCard, realTarget);
        customDesc += "Deal " + damageAmount + " to Everyone on Odd Turn Number!<br>";
        customDesc += "Heal " + healAmount + " to Everyone on Even Turn Number!<br>";
        customDesc += "<color=yellow>" + scriptableKeywords[0].keywordName + "</color>";

        return customDesc;
    }

    public override void OnPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {

    }

    public override void OnAiPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {

    }

    public override void OnDrawCard(CardScriptData cardScriptData)
    {
    
        ExecuteCard();
    }

    public void ExecuteCard()
    {

        MonoBehaviour runner = CombatCardHandler.Instance;

        if (this.cardSoundEffect != null)
        {
            AudioManager.Instance.cardSource.PlayOneShot(this.cardSoundEffect);
        }

        if (IsOdd(Combat.Instance.turns))
        {
            List<GameObject> allEntities = SystemManager.Instance.FindGameObjectsWithTags(SystemManager.Instance.GetAllTagsList());

            foreach (GameObject entity in allEntities)
            {
                SystemManager.Instance.SpawnEffectPrefab(this.abilityEffect,entity,0.6f);
                runner.StartCoroutine(Combat.Instance.AdjustTargetHealth(entityUsedCardGlobal, entity, damageAmount, false, SystemManager.AdjustNumberModes.ATTACK));
            }

        }
        else
        {

            List<GameObject> allEntities = SystemManager.Instance.FindGameObjectsWithTags(SystemManager.Instance.GetAllTagsList());

            foreach (GameObject entity in allEntities)
            {
                runner.StartCoroutine(Combat.Instance.AdjustTargetHealth(entityUsedCardGlobal, entity, damageAmount, false, SystemManager.AdjustNumberModes.HEAL));
            }
        }
    }

    bool IsOdd(int number)
    {
        return number % 2 != 0;
    }



}
