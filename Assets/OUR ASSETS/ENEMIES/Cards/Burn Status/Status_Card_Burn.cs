using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Status_Card_Burn", menuName = "Card/Status/Status_Card_Burn")]
public class Status_Card_Burn : ScriptableCard
{
    public ScriptableBuffDebuff burn;

    public override string OnCardDescription(CardScript cardScript, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScript, entityUsedCard);

        customDesc += "Add 1 Burn to the Hero when drawn";

        return customDesc;
    }

    public override void OnDrawCard(CardScript cardScript)
    {
        base.OnDrawCard(cardScript);

        MonoBehaviour runner = CombatCardHandler.Instance;
        GameObject target = GameObject.FindGameObjectWithTag("Player");
        //add burn status to player
        runner.StartCoroutine(BuffSystemManager.Instance.AddBuffDebuffIE(target, burn, 1, 0));

    }

}
