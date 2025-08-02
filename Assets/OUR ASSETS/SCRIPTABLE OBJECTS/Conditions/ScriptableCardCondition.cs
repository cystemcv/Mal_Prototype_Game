using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

public class ScriptableCardCondition : ScriptableObject
{

    public virtual string ConditionDescription(CardScriptData cardScriptData, CardConditionClass cardConditionClass, GameObject entity)
    {
        return  "";
    }


    public virtual bool OnPlayCard(CardScriptData cardScriptData, CardConditionClass cardConditionClass, GameObject entityUsedCard, SystemManager.ControlBy controlBy)
    {

        return true;
    }

}
