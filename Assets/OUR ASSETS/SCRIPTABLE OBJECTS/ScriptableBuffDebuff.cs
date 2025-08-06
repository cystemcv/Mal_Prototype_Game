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

    public struct StatModifiedValue
    {
        public SystemManager.StatModifiedAttribute StatModifiedAttribute;
        public SystemManager.StatModifiedType statModifiedType;
        public int statIncreaseInt;
        public float statIncreaseFloat;
    }

    public virtual void OnPlayCard(CardScriptData cardScriptData, GameObject caster, GameObject target)
    {

    }
    public virtual void OnDiscardCard(CardScriptData cardScriptData)
    {

    }

    public virtual void OnBanishedCard(CardScriptData cardScriptData)
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

    public virtual void OnApplyBuff(GameObject target, int value)
    {

    }

    public virtual void OnGettingHit(GameObject caster , GameObject target, int value, int turnsValue)
    {

    }

    public virtual StatModifiedValue? OnModifyStats(GameObject caster, GameObject target, ScriptableCard scriptableCard)
    {

        return null;

    }

    public virtual StatModifiedValue? OnModifyStats_Target(GameObject caster, GameObject target, ScriptableCard scriptableCard)
    {

        return null;

    }



}
