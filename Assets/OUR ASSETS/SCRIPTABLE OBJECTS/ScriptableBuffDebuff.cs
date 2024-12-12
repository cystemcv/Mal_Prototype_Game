using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BuffSystemManager;

[CreateAssetMenu(fileName = "ScriptableBuffDebuff", menuName = "BuffsDebuffs/ScriptableBuffDebuff")]
public class ScriptableBuffDebuff : ScriptableObject
{
    [Header("COMMON")]
    public string nameID;
    public Sprite icon;
    public string description;
    public bool isBuff = true;

    public bool infiniteDuration = false;


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

    public virtual bool OnEnemyTurnStart(GameObject target)
    {
        return false;
    }

    public virtual bool OnEnemyTurnEnd(GameObject target)
    {
        return false;
    }

    public virtual void OnExpireBuffDebuff(GameObject target)
    {

    }

    public virtual void OnApplyBuff(GameObject target, int value, int turnsValue)
    {

    }

}
