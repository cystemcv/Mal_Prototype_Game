using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using static ScriptableCard;

public class AIBrain : MonoBehaviour
{

    public List<ScriptableCard> cardScriptList;

    public int aiLogicStep = 0;

    public GameObject targetForCard;

    public GameObject intend;


    public void ExecuteCommand()
    {

        //if there are not cards then dont do anything
        if (cardScriptList.Count <= 0)
        {
            aiLogicStep = 0;
            return;
        }


        //play card
        PlayCardOnlyAbilities(cardScriptList[aiLogicStep]);

        IncreaseAiStep();

    }

    public void IncreaseAiStep()
    {
        //go to the next step
        aiLogicStep++;

        //check if it goes over the card limit reset it
        if (aiLogicStep > cardScriptList.Count - 1)
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

        //get the list of gameobjects on the scene
        List<GameObject> gameobjectsFound = SystemManager.Instance.GetObjectsWithTagsFromGameobjectOppossite(this.gameObject);

        int indexForTargetForCard = Random.Range(0, gameobjectsFound.Count);
        //assign a target for card
        targetForCard = gameobjectsFound[indexForTargetForCard];

        ScriptableCard scriptableCard = cardScriptList[aiLogicStep];

        SpawnAIIntend(scriptableCard);

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

    public void ReAssignTargetForCard()
    {
        List<GameObject> gameobjectsFound = SystemManager.Instance.GetObjectsWithTagsFromGameobjectOppossite(this.gameObject);

        if (gameobjectsFound.Count == 0)
        {
            return;
        }

        int indexForTargetForCard = Random.Range(0, gameobjectsFound.Count);
        //assign a target for card
        targetForCard = gameobjectsFound[indexForTargetForCard];
    }

    public void PlayCardOnlyAbilities(ScriptableCard cardScript)
    {

        StartCoroutine(PlayCardCoroutine(cardScript));

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


    public string GetCardDamageInString(ScriptableCard scriptableCard, GameObject entity)
    {

        string damageText = "";
        int count = 0;

        foreach (CardAbilityClass cardAbilityClass in scriptableCard.cardAbilityClass)
        {

            if (cardAbilityClass.scriptableCardAbility.abilityType == SystemManager.AbilityType.ATTACK)
            {

                if (count != 0)
                {
                    damageText += "+";
                }

                count += 1;

                //check multi hit attack
                int multiHits = DeckManager.Instance.GetIntValueFromList(1, cardAbilityClass.abilityIntValueList);
                string multiHitString = (multiHits > 0) ? multiHits + "x" : "";
                AIBrain aIBrain = entity.GetComponent<AIBrain>();

                GameObject target = null;
                if (aIBrain != null)
                {
                    //damageText += aIBrain.targetForCard.name + " ";
                    target = aIBrain.targetForCard;
                }

                damageText += "(" + multiHitString + Combat.Instance.CalculateEntityDmg(DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList), entity, target) + ")";
            }


        }


        return damageText;
    }


}
