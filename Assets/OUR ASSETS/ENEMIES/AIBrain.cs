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

    public List<GameObject> targetsList;

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

        //get the intends gameobject which is gonna be the parent of the icons
        GameObject intends = this.gameObject.transform.Find("gameobjectUI").Find("intendList").Find("intends").gameObject;

        ScriptableCard scriptableCard = cardScriptList[aiLogicStep];

        targetsList.Clear();
        //create the intend based on the ai choices
        GameObject intendObject = Instantiate(SystemManager.Instance.intendObject, intends.transform.position, Quaternion.identity);
        //parent it
        intendObject.transform.SetParent(intends.transform);

        TMP_Text damageText = intendObject.transform.Find("DamageText").GetComponent<TMP_Text>();
        TMP_Text cardText = intendObject.transform.Find("CardText").GetComponent<TMP_Text>();

        //get all the abilities dmg


        damageText.text = GetCardDamageInString(scriptableCard, this.gameObject, null);
        cardText.text = scriptableCard.cardName;

    }

    public void PlayCardOnlyAbilities(ScriptableCard cardScript)
    {

        StartCoroutine(PlayCardCoroutine(cardScript));

    }

    IEnumerator PlayCardCoroutine(ScriptableCard scriptableCard)
    {

        SystemManager.Instance.thereIsActivatedCard = true;

        GameObject entity = this.gameObject;

        //get the player
        GameObject target = GameObject.FindGameObjectWithTag("Player");

        if (entity == null)
        {
            yield return null;
        }

        foreach (CardAbilityClass cardAbilityClass in scriptableCard.cardAbilityClass)
        {

            if(cardAbilityClass == null)
            {
                continue;
            }

            //create new cardscript
            CardScript cardScript = new CardScript();
            cardScript.scriptableCard = scriptableCard;

            //activate effect
            cardAbilityClass.scriptableCardAbility.OnPlayCard(cardScript, cardAbilityClass, entity, target);

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


    public string GetCardDamageInString(ScriptableCard scriptableCard, GameObject entity, GameObject target)
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

                damageText += "(" + multiHitString + Combat.Instance.CalculateEntityDmg(DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList), entity, target) + ")";
            }
       

        }


        return damageText;
    }


}
