using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Status_Card_Wet", menuName = "Card/Status/Status_Card_Wet")]
public class Status_Card_Wet : ScriptableCard
{
    public ScriptableBuffDebuff wet;
    public int amount = 2;

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

        customDesc += "Add " + amount + " " + BuffSystemManager.Instance.GetBuffDebuffColor(wet) + " to a random ally when drawn";

        return customDesc;
    }

    public override void OnDrawCard(CardScriptData cardScriptData)
    {
        base.OnDrawCard(cardScriptData);

        MonoBehaviour runner = CombatCardHandler.Instance;
        GameObject target = AIManager.Instance.GetRandomTargetBasedOnTags(SystemManager.Instance.GetPlayerTagsList());
        //add burn status to player
        runner.StartCoroutine(BuffSystemManager.Instance.AddBuffDebuffIE(target, wet, amount));

    }

}
