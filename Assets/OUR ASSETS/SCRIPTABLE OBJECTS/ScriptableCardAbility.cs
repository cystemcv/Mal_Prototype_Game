using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using static BuffSystemManager;
using static ScriptableCard;


public class ScriptableCardAbility : ScriptableObject
{
    [Title("SETTINGS")]
    public SystemManager.AbilityType abilityType = SystemManager.AbilityType.OTHER;

    [Title("MAIN")]
    public string abilityName;

    [TextArea(5, 10)] // This creates a multi-line text area with a minimum height of 5 lines and a maximum height of 10 lines.
    [LabelText("Abil.Desc")] // Custom label for the field
    [GUIColor(0.9f, 0.9f, 1f)]
    public string abilityDevDescription;

    [Title("MISC")]
    public float originalEntityPosX;
    public ScriptableBuffDebuff scriptableBuffDebuff;
    public bool infiniteDuration = false;


    public LTDescr localMoveTween;




    //private float animationTime = 0f;

    private Animator entityAnimator;
    //public float GetFullAbilityWaitingTime(GameObject entity)
    //{

    //    try
    //    {

    //        entityAnimator = entity.transform.Find("model").GetComponent<Animator>();
    //        float totalTime = GetAnimationTime(entityAnimator, entityAnimation.ToString());

    //        return totalTime;
    //    }
    //    catch(Exception ex)
    //    {
    //        Debug.LogError("Ability waiting time error : " + ex.Message);

    //        return 0;
    //    }
    //}

    public virtual string AbilityDescription(CardScriptData cardScriptData, CardAbilityClass cardAbilityClass, GameObject entity)
    {
        return (abilityName.Trim() != "") ? "<color=blue>" + abilityName + "</color> : " : "";
    }

    public virtual void OnPlayCard(CardScriptData cardScriptData, CardAbilityClass cardAbilityClass, GameObject entityUsedCard, SystemManager.ControlBy controlBy)
    {


        // Trigger the animation
        entityAnimator = entityUsedCard.transform.Find("model").GetComponent<Animator>();

        ProceedWithAnimationAndSound(entityUsedCard, cardAbilityClass);

    }






    private void ProceedWithAnimationAndSound(GameObject character, CardAbilityClass cardAbilityClass)
    {

        float m_CurrentClipLength = 0;
        string m_ClipName = "";

        if (entityAnimator != null)
        {
            entityAnimator.SetTrigger(cardAbilityClass.entityAnimation.ToString());
        }



        // Play the sound
        AudioManager.Instance.PlayCardSound(cardAbilityClass.entitySound.ToString());
    }

    public float GetAnimationTime(Animator animator, string animation)
    {


        // Get the RuntimeAnimatorController from the Animator
        RuntimeAnimatorController controller = animator.runtimeAnimatorController;

        // Loop through all the animation clips in the controller
        foreach (AnimationClip clip in controller.animationClips)
        {
            // Find the clip by name
            if (clip.name == animation)
            {
                return clip.length;
            }
        }

        //if not found
        return 0;
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



    public ScriptableBuffDebuff GetBuffDebuff()
    {

        return scriptableBuffDebuff;
    }

    public ScriptableCardAbility GetThisAbility()
    {

        return this;
    }

    public void SpawnEffectPrefab(GameObject target, CardAbilityClass cardAbilityClass)
    {
        //missing prefab vfx
        if (cardAbilityClass.abilityEffect == null || target == null)
        {
            return;
        }

        //spawn prefab
        GameObject abilityEffect = Instantiate(cardAbilityClass.abilityEffect, target.transform.Find("model").Find("SpawnEffect").position, Quaternion.identity);
        //abilityEffect.transform.SetParent(target.transform.Find("model").Find("SpawnEffect"));

        abilityEffect.transform.position = new Vector3(abilityEffect.transform.position.x, 
            abilityEffect.transform.position.y + cardAbilityClass.abilityEffectYaxis,
            abilityEffect.transform.position.z);

        //if the target is player then we wanna rotate 
        if (target.tag == "Player")
        {
            abilityEffect.transform.Rotate(new Vector3(0, 180, 0));
        }

        Destroy(abilityEffect, cardAbilityClass.abilityEffectLifetime);
    }

    //public string GetAbilityAIIntend()
    //{
    //    string intend = aIIntend + "-" + aITypeOfAttack + "-" + aIWhoToTarget;

    //    return intend;
    //}



}
