using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BuffSystemManager;

public class ScriptableCardAbility : ScriptableObject
{

    public string abilityName;
    public float waitForAbility = 0.2f;
    //public bool runToTarget = false;
    public SystemManager.TypeOfAttack typeOfAttack = SystemManager.TypeOfAttack.SIMPLE;
    public float timeToGetToTarget = 0.2f;


    public float originalCharacterPosX;

    public ScriptableBuffDebuff scriptableBuffDebuff;
    public LTDescr localMoveTween;

    public GameObject abilityEffect;
    public GameObject abilityAfterEffect;
    public float abilityAfterEffectLifetime = 0.2f;
    public SystemManager.CardCharacterAnimation cardCharacterAnimation = SystemManager.CardCharacterAnimation.MeleeAttack;
    public SystemManager.CardCharacterSound cardCharacterSound = SystemManager.CardCharacterSound.Generic;

    private Animator characterAnimator;

    public virtual string AbilityDescription(CardScript cardScript, GameObject character)
    {
        return "<color=blue>" + abilityName + "</color>";
    }



    public virtual void OnPlayCard(CardScript cardScript, GameObject character, GameObject target)
    {

        // Trigger the animation
        characterAnimator = character.transform.Find("model").GetComponent<Animator>();

        if (typeOfAttack == SystemManager.TypeOfAttack.SIMPLE)
        {
            ProceedWithAnimationAndSound(character);
        }
        else if (typeOfAttack == SystemManager.TypeOfAttack.PROJECTILE)
        {

            ProceedWithAnimationAndSound(character);

            Transform projectileSpawn = character.transform.Find("PROJECTILESPAWN");

            //spawn the projectile
            GameObject projectile = Instantiate(abilityEffect, projectileSpawn.position, Quaternion.identity);

            // Move the projectile
            localMoveTween = LeanTween.moveX(projectile, target.transform.position.x, timeToGetToTarget);

            // Invoke the method to proceed after the movement duration using InvokeHelper
            InvokeHelper.Instance.Invoke(() => OnProjectileCompleted(projectile, target), timeToGetToTarget + waitForAbility);

        }
        else if (typeOfAttack == SystemManager.TypeOfAttack.MELLEE)
        {


            // Move the character
            localMoveTween = LeanTween.moveX(character, target.transform.position.x, timeToGetToTarget);

            if (characterAnimator != null)
            {
                characterAnimator.SetTrigger("Run");
            }

            // Invoke the method to proceed after the movement duration using InvokeHelper
            InvokeHelper.Instance.Invoke(() => OnMovementComplete(character), timeToGetToTarget);

            // Then invoke this to go back
            InvokeHelper.Instance.Invoke(() => OnAnimationComplete(character), timeToGetToTarget + waitForAbility);
        }
        else
        {
            // If no movement, proceed directly to the animation and sound
            ProceedWithAnimationAndSound(character);
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

        if (characterAnimator != null)
        {
            characterAnimator.SetTrigger(cardCharacterAnimation.ToString());
        }

        Debug.Log("PLAYED CARD BY " + character.GetComponent<CharacterClass>().scriptablePlayer.mainClass);

        // Play the sound
        AudioManager.Instance.PlayCardSound(cardCharacterSound.ToString());
    }

    private void OnAnimationComplete(GameObject character)
    {
        // Proceed with animation and sound after the movement
        ProceedToGoBack(character);
    }

    private void ProceedToGoBack(GameObject character)
    {

        // Move the character back
        localMoveTween = LeanTween.moveX(character, character.GetComponent<CharacterClass>().originalCombatPos.position.x, timeToGetToTarget);

        if (characterAnimator != null)
        {
            characterAnimator.SetTrigger("RunBack");
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
