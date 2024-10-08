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

        //create the intend based on the ai choices
        GameObject displayCardName = this.gameObject.transform.Find("gameobjectUI").Find("DisplayCardName").gameObject;


        TMP_Text cardText = displayCardName.transform.Find("CardText").GetComponent<TMP_Text>();

   

        //get all the abilities dmg

        //if (scriptableCard.targetEntityTagList.Count == 0)
        //{
        //    targetText.text = "";
        //}
        //else
        //{
        //    targetText.text = targetForCard.name;
        //}

        //damageText.text = GetCardDamageInString(scriptableCard, this.gameObject);
        cardText.text = scriptableCard.cardName;

    }

    public void ReAssignTargetForCard()
    {
        List<GameObject> gameobjectsFound = SystemManager.Instance.GetObjectsWithTagsFromGameobjectOppossite(this.gameObject);

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

        foreach (CardAbilityClass cardAbilityClass in scriptableCard.cardAbilityClass)
        {

            if (cardAbilityClass == null)
            {
                continue;
            }

            //create new cardscript
            CardScript cardScript = new CardScript();
            cardScript.scriptableCard = scriptableCard;

            //activate effect
            cardAbilityClass.scriptableCardAbility.OnPlayCard(cardScript, cardAbilityClass, entity, SystemManager.ControlBy.AI);

            yield return new WaitForSeconds(cardAbilityClass.waitForAbility);
            //yield return new WaitForSeconds(10f);
        }

        //go back to idle animation
        if (entity != null)
        {
            Animator animator = entity.transform.Find("model").GetComponent<Animator>();

            if (animator != null)
            {

                animator.SetTrigger("Idle");
            }

        }



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
                if(aIBrain != null)
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
