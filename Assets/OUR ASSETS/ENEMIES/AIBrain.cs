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
    public ScriptableEntity.AICommandCards aICommandCardToUse;
    public ScriptableCard scriptableCardToUse;

    public int getCardLevel = 0;
    public int entityLevel = 0;

    public bool justSpawned = false;

    public void ExecuteCommand()
    {

        //if there are not cards then dont do anything
        if (scriptableEntity.aICommands.Count <= 0)
        {
            aiLogicStep = 0;
            return;
        }

        if (scriptableCardToUse == null)
        {
            return;
        }

        //for any reason the target dies the assign a new target
        if (aICommandCardToUse.aiScriptableCard.targetEntityTagList.Count != 0 && targetForCard == null)
        {
            GenerateIntend();
        }

        //play card
        PlayCardOnlyAbilities(scriptableCardToUse);

        IncreaseAiStep();

    }

    //public void ExecuteInitializeCommand()
    //{

    //    //if there are not cards then dont do anything
    //    if (scriptableEntity.aICommandsInitialize == null)
    //    {
    //        return;
    //    }

    //    aICommandCardToUse = GetAICardFromCommand(scriptableEntity.aICommandsInitialize);
    //    ScriptableCard scriptableCard = aICommandCardToUse.aiScriptableCard;

    //    //play card
    //    PlayCardOnlyAbilities(scriptableCard);

    //    IncreaseAiStep();

    //}


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
        CardScriptData cardScriptData = new CardScriptData();

        if (scriptableEntity.aICommands.Count != 0)
        {
            aICommandCardToUse = GetAICardFromCommand(scriptableEntity.aICommands[aiLogicStep]);
            scriptableCardToUse = aICommandCardToUse.aiScriptableCard;

     
            cardScriptData.scriptableCard = scriptableCardToUse;
            getCardLevel = Random.Range(aICommandCardToUse.modifiedCardValueMin + entityLevel, aICommandCardToUse.modifiedCardValueMax + entityLevel);
            cardScriptData.scalingLevelValue = getCardLevel;

            //assign a target for card
            scriptableCardToUse.OnAiPlayTarget(cardScriptData, this.gameObject);
        }
        else
        {
            scriptableCardToUse = null;
        }

        SpawnAIIntend(scriptableCardToUse, cardScriptData);

    }

    public void SpawnAIIntend(ScriptableCard scriptableCard, CardScriptData cardScriptData)
    {

        if (intend == null)
        {
            intend = SystemManager.Instance.SpawnPrefab(UI_Combat.Instance.displayCardName, this.gameObject, "DisplayCardName", this.gameObject.GetComponent<EntityClass>().scriptableEntity.spawnIntend);
            //set parent of 
        }

        intend.SetActive(true);
        TMP_Text cardText = intend.transform.Find("CardText").GetComponent<TMP_Text>();

        if (scriptableCardToUse == null)
        {
            cardText.text = "";
        }
        else
        {
            cardText.text = scriptableCard.cardName;
            cardText.text += DeckManager.Instance.ShowCardLevel(cardScriptData);
        }




    }

    public ScriptableEntity.AICommandCards GetAICardFromCommand(ScriptableEntity.AICommand aICommand)
    {

        ScriptableEntity.AICommandCards aICommandCard = null;

        if (aICommand.aICommandType == SystemManager.AICommandType.MANUAL)
        {
            //then play the first card
            aICommandCard = aICommand.aiScriptableCards[0];
        }
        else if (aICommand.aICommandType == SystemManager.AICommandType.RANDOM)
        {
            //then play a random card from that set
            int randomNumber = Random.Range(0, aICommand.aiScriptableCards.Count);
            aICommandCard = aICommand.aiScriptableCards[randomNumber];
        }

        return aICommandCard;
    }





    public void PlayCardOnlyAbilities(ScriptableCard scriptableCard)
    {

        StartCoroutine(PlayCardCoroutine(scriptableCard));

    }

    IEnumerator PlayCardCoroutine(ScriptableCard scriptableCard)
    {

        //SystemManager.Instance.thereIsActivatedCard = true;

        GameObject entity = this.gameObject;


        if (entity == null)
        {
            yield return null;
        }



        //create new cardscript
        CardScriptData cardScriptData = new CardScriptData();
        cardScriptData.scriptableCard = scriptableCard;
        cardScriptData.scalingLevelValue = getCardLevel;

        //activate effect
        cardScriptData.scriptableCard.OnAiPlayCard(cardScriptData, entity);

        yield return new WaitForSeconds(cardScriptData.scriptableCard.abilityEffectLifetime);
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
        //SystemManager.Instance.thereIsActivatedCard = false;



    }




}
