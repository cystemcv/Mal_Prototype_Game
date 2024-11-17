using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

public class ScriptableCardCondition : ScriptableObject
{

    public virtual string ConditionDescription(CardScript cardScript, CardConditionClass cardConditionClass, GameObject entity)
    {
        return  "";
    }


    public virtual bool OnPlayCard(CardScript cardScript, CardConditionClass cardConditionClass, GameObject entityUsedCard, SystemManager.ControlBy controlBy)
    {

        return true;
    }

}
