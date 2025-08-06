using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff_Honor", menuName = "Buffs/Buff_Honor")]
public class Buff_Honor : ScriptableBuffDebuff
{

    public ScriptableBuffDebuff righteous;
    public int buffValue = 1;

    public override void OnPlayCard(CardScriptData cardScriptData, GameObject caster, GameObject target)
    {
        base.OnPlayCard(cardScriptData,caster, target);
        Activate(cardScriptData, caster);
    }



    public void Activate(CardScriptData cardScriptData,GameObject target)
    {

        if (cardScriptData.primaryManaCost < 2)
        {
            return;
        }

        BuffSystemManager.Instance.AddBuffDebuff(target, righteous, buffValue);

    }

 

}
