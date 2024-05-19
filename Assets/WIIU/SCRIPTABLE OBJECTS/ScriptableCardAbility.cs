using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BuffSystemManager;

public class ScriptableCardAbility : ScriptableObject
{

    public string abilityName;
    public float waitForAbility = 0f;

    public ScriptableBuffDebuff scriptableBuffDebuff;



    public virtual string AbilityDescription(ScriptableCard scriptableCard)
    {
        return "<color=blue>" + abilityName + "</color>";
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

    public virtual bool OnCharacterTurnStart(GameObject target)
    {
        return false;
    }

    public virtual bool OnCharacterTurnEnd(GameObject target)
    {
        return false;
    }

    public virtual bool OnEnemyTurnStart( GameObject target)
    {
        return false;
    }

    public virtual bool OnEnemyTurnEnd(GameObject target)
    {
        return false;
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

    public ScriptableBuffDebuff GetBuffDebuff()
    {

        return scriptableBuffDebuff;
    }

    public ScriptableCardAbility GetThisAbility()
    {

        return this;
    }
}
