using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "CardCondition_CheckHowManySummons", menuName = "CardCondition/CardCondition_CheckHowManySummons")]
public class CardCondition_CheckHowManySummons : ScriptableCardCondition
{

    public override string ConditionDescription(CardScript cardScript, CardConditionClass cardConditionClass, GameObject entity)
    {

        string message = "<color=yellow>Requires at least " + cardConditionClass.abilityIntValueList[0] + " summons!</color>";

        return message;

    }

    public override bool OnPlayCard(CardScript cardScript, CardConditionClass cardConditionClass, GameObject entityUsedCard, SystemManager.ControlBy controlBy)
    {
        List<string> tag = new List<string>();
        tag.Add("PlayerSummon");

        List<GameObject> entities =  SystemManager.Instance.FindGameObjectsWithTags(tag);

        if (entities.Count >= cardConditionClass.abilityIntValueList[0])
        {
            return false;
        }
        else
        {
            return true;
        }


       
    }

}
