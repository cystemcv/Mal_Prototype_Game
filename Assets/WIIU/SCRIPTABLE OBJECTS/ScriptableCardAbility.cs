using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BuffSystemManager;

public class ScriptableCardAbility : ScriptableObject
{

    public string abilityName;
    public float waitForAbility = 0f;

    public ScriptableBuffDebuff scriptableBuffDebuff;



    public virtual string AbilityDescription(CardScript cardScript)
    {
        return "<color=blue>" + abilityName + "</color>";
    }

    public virtual void OnPlayCard(CardScript cardScript)
    {


  
    }

    public virtual void OnDiscardCard(CardScript cardScript)
    {

    }

    public virtual void OnBanishedCard(CardScript cardScript)
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

    public int GetAbilityVariable(CardScript cardScript)
    {
        int abilityValue = 0;

        int index = cardScript.scriptableCard.scriptableCardAbilities.IndexOf(this);

        if (index == 0)
        {
            abilityValue = cardScript.scriptableCard.ability1Var;
        }
        else if (index == 1)
        {
            abilityValue = cardScript.scriptableCard.ability2Var;
        }
        else if (index == 2)
        {
            abilityValue = cardScript.scriptableCard.ability3Var;
        }
        else if (index == 3)
        {
            abilityValue = cardScript.scriptableCard.ability4Var;
        }
        else if (index == 4)
        {
            abilityValue = cardScript.scriptableCard.ability5Var;
        }
        else if (index == 5)
        {
            abilityValue = cardScript.scriptableCard.ability6Var;
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
