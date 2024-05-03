using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableCardAbility : ScriptableObject
{

    public string abilityName;
   
    public virtual string AbilityDescription(ScriptableCard scriptableCard)
    {
        return abilityName;
    }

    public virtual void OnPlayCard(ScriptableCard scriptableCard)
    {

    }

    public virtual void OnDiscardCard(ScriptableCard scriptableCard)
    {

    }

    public virtual void OnBanishedCard(ScriptableCard scriptableCard)
    {

    }

    public int GetAbilityVariable(ScriptableCard scriptableCard)
    {
        int abilityValue = 0;

        int index = scriptableCard.scriptableCardAbilities.IndexOf(this);

        if (index == 0)
        {
            abilityValue = scriptableCard.ability1Var;
        }
        else if (index == 1)
        {
            abilityValue = scriptableCard.ability2Var;
        }
        else if (index == 2)
        {
            abilityValue = scriptableCard.ability3Var;
        }
        else if (index == 3)
        {
            abilityValue = scriptableCard.ability4Var;
        }
        else if (index == 4)
        {
            abilityValue = scriptableCard.ability5Var;
        }
        else if (index == 5)
        {
            abilityValue = scriptableCard.ability6Var;
        }

        return abilityValue;
    }
}
