using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff_MaxHealth", menuName = "Buffs/Buff_MaxHealth")]
public class Buff_MaxHealth : ScriptableBuffDebuff
{


    public override void OnApplyBuff(GameObject target, int value)
    {
        EntityClass entityClass = target.GetComponent<EntityClass>();

        entityClass.maxHealth += value;
        entityClass.health += value;

        entityClass.healthText.GetComponent<TMP_Text>().text = entityClass.health + "/" + entityClass.maxHealth;

        //adjust the hp bar
        UI_Combat.Instance.UpdateHealthBarSmoothly(entityClass.health, entityClass.maxHealth, entityClass.slider);
    }

    public override void OnExpireBuffDebuff(GameObject target)
    {

        BuffDebuffClass buffDebuffClass = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(target, "MaxHealth");

        target.GetComponent<EntityClass>().maxHealth += buffDebuffClass.tempValue;
        target.GetComponent<EntityClass>().health += buffDebuffClass.tempValue;
    }

    public override StatModifiedValue? OnModifyStats(GameObject caster, GameObject target, ScriptableCard scriptableCard)
    {

        BuffDebuffClass buffDebuffClass = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(caster, "MaxHealth");

        StatModifiedValue statModifiedValue = new StatModifiedValue();
        statModifiedValue.StatModifiedAttribute = SystemManager.StatModifiedAttribute.MAXHEALTH;
        statModifiedValue.statModifiedType = SystemManager.StatModifiedType.NORMAL;
        statModifiedValue.statIncreaseInt = buffDebuffClass.tempValue;
        statModifiedValue.statIncreaseFloat = 0;

        return statModifiedValue;
    }
}
