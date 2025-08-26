using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster_Card_WaterAbsorption", menuName = "Card/Monster/Monster_Card_WaterAbsorption")]
public class Monster_Card_WaterAbsorption : ScriptableCard
{

    public ScriptableBuffDebuff wet;

    private int scalingAmount = 1;
    //private float scalingAmountFinal = 1;

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        //scaling
       float scalingAmountFinal = scalingAmount + ((scalingLevelCardValue * cardScriptData.scalingLevelValue) / 2f);

        customDesc = "Absorb all " + wet.nameID + " in combat. Heal all allies by the absorbed amount x" + scalingAmountFinal;

        return customDesc;
    }

    public override void OnPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnPlayCard(cardScriptData, entityUsedCard);
        PlayCard(cardScriptData, entityUsedCard);
    }

    public override void OnAiPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayCard(cardScriptData, entityUsedCard);
        PlayCard(cardScriptData, entityUsedCard);
    }

    public void PlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        MonoBehaviour runner = CombatCardHandler.Instance;

        List<GameObject> allEntities = SystemManager.Instance.GetAllEntities();
        List<BuffDebuffClass> buffDebuffClassList = BuffSystemManager.Instance.GetBuffDebuffClassFromTargets(allEntities, wet.nameID);

        int countWet = 0;

        foreach (GameObject entity in allEntities)
        {
            //get the buff
            BuffDebuffClass buffDebuffClass = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(entity, wet.nameID);
            if (!SystemManager.Instance.CheckNullMonobehavior(buffDebuffClass))
            {
                countWet += buffDebuffClass.tempValue;
                //remove the buff
                BuffSystemManager.Instance.DecreaseValueTargetBuffDebuff(entity, wet.nameID, -1 * buffDebuffClass.tempValue);
            }

        }

        //heal all allies
        List<GameObject> allyList = SystemManager.Instance.GetObjectsWithTagsFromGameobjectSameSide(entityUsedCard);

        //scaling
        float scalingAmountFinal = scalingAmount + ((scalingLevelCardValue * cardScriptData.scalingLevelValue) / 2f);
        float calcAmount = countWet * scalingAmountFinal;
        countWet = (int)Mathf.Round(calcAmount);

        foreach (GameObject ally in allyList)
        {
            runner.StartCoroutine(Combat.Instance.AdjustTargetHealth(entityUsedCard, ally, countWet, false, SystemManager.AdjustNumberModes.HEAL));
        }

    }

}
