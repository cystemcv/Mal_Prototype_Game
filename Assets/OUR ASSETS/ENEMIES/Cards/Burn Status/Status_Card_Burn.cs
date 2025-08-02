using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Status_Card_Burn", menuName = "Card/Status/Status_Card_Burn")]
public class Status_Card_Burn : ScriptableCard
{
    public ScriptableBuffDebuff burn;

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        customDesc += "Add 1 " + BuffSystemManager.Instance.GetBuffDebuffColor(burn) + " to the Hero when drawn";

        return customDesc;
    }

    public override void OnDrawCard(CardScriptData cardScriptData)
    {
        base.OnDrawCard(cardScriptData);

        MonoBehaviour runner = CombatCardHandler.Instance;
        GameObject target = GameObject.FindGameObjectWithTag("Player");
        //add burn status to player
        runner.StartCoroutine(BuffSystemManager.Instance.AddBuffDebuffIE(target, burn, 1));

    }

}
