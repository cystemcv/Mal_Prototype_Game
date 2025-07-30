using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using static ScriptableCard;



public class AIBrain : MonoBehaviour
{

 


    //public List<ScriptableCard> cardScriptList;

    public int aiLogicStep = 0;

    public GameObject targetForCard;

    public GameObject intend;

    public ScriptableEntity scriptableEntity;
    public ScriptableCard scriptableCardToUse;

    public void ExecuteCommand()
    {

        //if there are not cards then dont do anything
        if (scriptableEntity.aICommands.Count <= 0)
        {
            aiLogicStep = 0;
            return;
        }

        //play card
        PlayCardOnlyAbilities(scriptableCardToUse);

        IncreaseAiStep();

    }

    public void ExecuteInitializeCommand()
    {

        //if there are not cards then dont do anything
        if (scriptableEntity.aICommandsInitialize == null)
        {
            return;
        }

        ScriptableCard scriptableCard = GetAICardFromCommand(scriptableEntity.aICommandsInitialize);
        //play card
        PlayCardOnlyAbilities(scriptableCard);

        IncreaseAiStep();

    }


    public void IncreaseAiStep()
    {
        //go to the next step
        aiLogicStep++;

        //check if it goes over the card limit reset it
        if (aiLogicStep > scriptableEntity.aICommands.Count - 1)
        {
            aiLogicStep = 0;
        }


    }

    public void GenerateIntend()
    {

        if (this.gameObject.GetComponent<EntityClass>().entityMode == SystemManager.EntityMode.DEAD)
        {
            return;
        }

        scriptableEntity = this.gameObject.GetComponent<EntityClass>().scriptableEntity;

        scriptableCardToUse = GetAICardFromCommand(scriptableEntity.aICommands[aiLogicStep]);

        CardScript cardScript = new CardScript();
        cardScript.scriptableCard = scriptableCardToUse;
        //assign a target for card
        scriptableCardToUse.OnAiPlayTarget(cardScript, this.gameObject);

        SpawnAIIntend(scriptableCardToUse);

    }

    public void SpawnAIIntend(ScriptableCard scriptableCard)
    {

        if (intend == null)
        {
            intend = SystemManager.Instance.SpawnPrefab(UI_Combat.Instance.displayCardName, this.gameObject, "DisplayCardName", this.gameObject.GetComponent<EntityClass>().scriptableEntity.spawnIntend);
            //set parent of 
        }

        intend.SetActive(true);
        TMP_Text cardText = intend.transform.Find("CardText").GetComponent<TMP_Text>();
        cardText.text = scriptableCard.cardName;
    }

    public ScriptableCard GetAICardFromCommand(ScriptableEntity.AICommand aICommand)
    {

        ScriptableCard scriptableCard = null;

        if (aICommand.aICommandType == SystemManager.AICommandType.MANUAL)
        {
            //then play the first card
            scriptableCard = aICommand.aiScriptableCards[0];
        }
        else if (aICommand.aICommandType == SystemManager.AICommandType.RANDOM)
        {
            //then play a random card from that set
            int randomNumber = Random.Range(0, scriptableEntity.aICommands.Count);
            scriptableCard = aICommand.aiScriptableCards[randomNumber];
        }

        return scriptableCard;
    }





    public void PlayCardOnlyAbilities(ScriptableCard scriptableCard)
    {

        StartCoroutine(PlayCardCoroutine(scriptableCard));

    }

    IEnumerator PlayCardCoroutine(ScriptableCard scriptableCard)
    {

        SystemManager.Instance.thereIsActivatedCard = true;

        GameObject entity = this.gameObject;


        if (entity == null)
        {
            yield return null;
        }



        //create new cardscript
        CardScript cardScript = new CardScript();
        cardScript.scriptableCard = scriptableCard;

        //activate effect
        cardScript.scriptableCard.OnAiPlayCard(cardScript, entity);

        yield return new WaitForSeconds(cardScript.scriptableCard.abilityEffectLifetime);
        //yield return new WaitForSeconds(10f);


        ////go back to idle animation
        //if (entity != null)
        //{
        //    Animator animator = entity.transform.Find("model").GetComponent<Animator>();

        //    if (animator != null)
        //    {

        //        animator.SetTrigger("Idle");
        //    }

        //}



        //card ended
        SystemManager.Instance.thereIsActivatedCard = false;



    }




}
