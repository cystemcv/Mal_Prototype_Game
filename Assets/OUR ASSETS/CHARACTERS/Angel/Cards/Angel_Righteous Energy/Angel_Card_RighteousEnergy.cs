using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Angel_Card_RighteousEnergy", menuName = "Card/Angel/Angel_Card_RighteousEnergy")]
public class Angel_Card_RighteousEnergy : ScriptableCard
{

    public int buffValue = 0;

    private GameObject realTarget;
    private GameObject entityUsedCardGlobal;

    public ScriptableBuffDebuff righteous;

    private int buffValueFinal = 0;


    public override string OnCardDescription(CardScript cardScript, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScript, entityUsedCard);

        customDesc += "Add " + buffValue + " <color=yellow>" + righteous.nameID + "</color> ." +
            "\nDouble this amount if there is no other <color=yellow>" + righteous.nameID + "</color>";

        return customDesc;
    }

    public override void OnPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        realTarget = CombatCardHandler.Instance.targetClicked;

        base.OnPlayCard(cardScript, entityUsedCard);

        BuffDebuffClass buffDebuff = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(realTarget, righteous.nameID);

        if (buffDebuff == null)
        {
            buffValueFinal = buffValue * 2;
        }
        else
        {
            buffValueFinal = buffValue;
        }

        //strength
        BuffSystemManager.Instance.AddBuffDebuff(realTarget, righteous, buffValueFinal, 0);
    }

    public override void OnAiPlayCard(CardScript cardScript, GameObject entityUsedCard)
    {
        realTarget = AIManager.Instance.GetRandomAlly(entityUsedCard); ;

        base.OnPlayCard(cardScript, entityUsedCard);

        BuffDebuffClass buffDebuff = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(realTarget, righteous.nameID);

        if (buffDebuff == null)
        {
            buffValueFinal = buffValue * 2;
        }

        //strength
        BuffSystemManager.Instance.AddBuffDebuff(realTarget, righteous, buffValueFinal, 0);
    }






}
