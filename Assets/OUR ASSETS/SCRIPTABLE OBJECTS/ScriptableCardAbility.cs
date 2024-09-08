using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BuffSystemManager;

public class ScriptableCardAbility : ScriptableObject
{
    [Header("COMMON")]
    public string abilityName;
    public float waitForAbility = 0.2f;
    //public bool runToTarget = false;
    public SystemManager.TypeOfAttack typeOfAttack = SystemManager.TypeOfAttack.SIMPLE;
    public float timeToGetToTarget = 0.2f;
    public float waitAmountDelay = 0.2f; //another delay


    public float originalEntityPosX;

    public ScriptableBuffDebuff scriptableBuffDebuff;
    public LTDescr localMoveTween;

    public GameObject abilityEffect;
    public float abilityEffectLifetime = 0.2f;
    public GameObject abilityAfterEffect;
    public float abilityAfterEffectLifetime = 0.2f;
    public SystemManager.EntityAnimation entityAnimation = SystemManager.EntityAnimation.MeleeAttack;
    public SystemManager.EntitySound entitySound = SystemManager.EntitySound.Generic;

    private Animator entityAnimator;

    [Header("AI SETTINGS")]
    public SystemManager.AIIntend aIIntend;
    public SystemManager.AITypeOfAttack aITypeOfAttack;
    public SystemManager.AIWhoToTarget aIWhoToTarget;

    private float animationTime = 0f;

    public float GetFullAbilityWaitingTime(GameObject entity)
    {

        try
        {

            entityAnimator = entity.transform.Find("model").GetComponent<Animator>();
            float totalTime = GetAnimationTime(entityAnimator, entityAnimation.ToString());
            if (typeOfAttack == SystemManager.TypeOfAttack.MELLEE)
            {
                totalTime += timeToGetToTarget * 2; //because it goes to target and back
            }
            else if (typeOfAttack == SystemManager.TypeOfAttack.PROJECTILE)
            {
                totalTime += timeToGetToTarget;
            }


            return totalTime;
        }
        catch(Exception ex)
        {
            Debug.LogError("Ability waiting time error : " + ex.Message);

            return 0;
        }
    }

    public virtual string AbilityDescription(CardScript cardScript, GameObject entity)
    {
        return "<color=blue>" + abilityName + "</color>";
    }

    public virtual void OnPlayCard(CardScript cardScript, GameObject entity, GameObject target)
    {


        // Trigger the animation
        entityAnimator = entity.transform.Find("model").GetComponent<Animator>();

        //reset it
        animationTime = GetAnimationTime(entityAnimator, entityAnimation.ToString());


        if (typeOfAttack == SystemManager.TypeOfAttack.SIMPLE)
        {
            ProceedWithAnimationAndSound(entity);
        }
        else if (typeOfAttack == SystemManager.TypeOfAttack.PROJECTILE)
        {

            // Start the coroutine that waits for the animation to end before proceeding
            CoroutineHelper.Instance.StartCoroutine(SpawnProjectileAfterAnimation(entity, target));

        }
        else if (typeOfAttack == SystemManager.TypeOfAttack.MELLEE)
        {


            // Move the character
            float posOrNeg = entity.transform.position.x - target.transform.position.x;
            float distanceAwayFromTarget = 0;
  
            if (posOrNeg <= 0)
            {
                distanceAwayFromTarget = -1.5f;
            }
            else
            {
                distanceAwayFromTarget = 1.5f;
            }

            localMoveTween = LeanTween.moveX(entity, target.transform.position.x + distanceAwayFromTarget, timeToGetToTarget);

            if (entityAnimator != null)
            {
                entityAnimator.SetTrigger("Run");
            }

            // Invoke the method to proceed after the movement duration using InvokeHelper
            InvokeHelper.Instance.Invoke(() => OnMovementComplete(entity), timeToGetToTarget);

            // Then invoke this to go back
            InvokeHelper.Instance.Invoke(() => OnAnimationComplete(entity), timeToGetToTarget + animationTime);
        }
        else
        {
            // If no movement, proceed directly to the animation and sound
            ProceedWithAnimationAndSound(entity);
        }
    }

    // Coroutine to handle spawning the projectile after the animation ends
    private IEnumerator SpawnProjectileAfterAnimation(GameObject entity, GameObject target)
    {
        // Proceed with the animation and sound
        ProceedWithAnimationAndSound(entity);

        // Assuming "animationTime" is the time it takes for the animation to complete
        yield return new WaitForSeconds(animationTime);

        // After the animation ends, proceed with spawning the projectile
        Transform projectileSpawn = entity.transform.Find("PROJECTILESPAWN");

        if (projectileSpawn != null)
        {
            // Spawn the projectile
            GameObject projectile = Instantiate(abilityEffect, projectileSpawn.position, Quaternion.identity);

            // Move the projectile
            localMoveTween = LeanTween.moveX(projectile, target.transform.position.x, timeToGetToTarget);

            // Invoke the method to proceed after the movement duration using InvokeHelper
            InvokeHelper.Instance.Invoke(() => OnProjectileCompleted(projectile, target), timeToGetToTarget);
        }
        else
        {
            Debug.LogWarning("Projectile spawn point not found!");
        }
    }

    private void OnMovementComplete(GameObject character)
    {
        // Proceed with animation and sound after the movement
        ProceedWithAnimationAndSound(character);
    }

    private void OnProjectileCompleted(GameObject projectile, GameObject target)
    {
        //destroy main projectile
        Destroy(projectile);

        //spawn after effect 
        if (abilityAfterEffect != null) {
            GameObject temp_abilityAfterEffect = Instantiate(abilityAfterEffect, target.transform.position, Quaternion.identity);
            Destroy(temp_abilityAfterEffect, abilityAfterEffectLifetime);
        }
  

    }

    private void ProceedWithAnimationAndSound(GameObject character)
    {

        float m_CurrentClipLength = 0;
        string m_ClipName = "";

        if (entityAnimator != null)
        {
            entityAnimator.SetTrigger(entityAnimation.ToString());
        }

  

        // Play the sound
        AudioManager.Instance.PlayCardSound(entitySound.ToString());
    }

    public float GetAnimationTime(Animator animator , string animation)
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

    private void OnAnimationComplete(GameObject character)
    {
        // Proceed with animation and sound after the movement
        ProceedToGoBack(character);
    }

    private void ProceedToGoBack(GameObject character)
    {

        // Move the character back
        localMoveTween = LeanTween.moveX(character, character.GetComponent<EntityClass>().OriginXPos, timeToGetToTarget);

        if (entityAnimator != null)
        {
            entityAnimator.SetTrigger("RunBack");
        }
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

    public void SpawnEffectPrefab(GameObject target)
    {
        //spawn prefab
        GameObject abilityEffect = Instantiate(this.abilityEffect, target.transform.Find("model").Find("SpawnEffect").position, Quaternion.identity);
        abilityEffect.transform.SetParent(target.transform.Find("model").Find("SpawnEffect"));

        //if the target is player then we wanna rotate 
        if (target.tag == "Player")
        {
            abilityEffect.transform.Rotate(new Vector3(0, 180, 0));
        }

        Destroy(abilityEffect, this.abilityEffectLifetime);
    }

    //public string GetAbilityAIIntend()
    //{
    //    string intend = aIIntend + "-" + aITypeOfAttack + "-" + aIWhoToTarget;

    //    return intend;
    //}

    

}
