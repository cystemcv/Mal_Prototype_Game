using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class AIBrain : MonoBehaviour
{

    public List<CardScript> cardScriptList;

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

        //save what gameobject used the card
        cardScriptList[aiLogicStep].whoUsedCard = this.gameObject;

        
        //play card
        DeckManager.Instance.PlayCardOnlyAbilities(cardScriptList[aiLogicStep]);

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

        int countPos = 0;

        //loop throught the abilities
        foreach (ScriptableCardAbility scriptableCardAbility in cardScriptList[aiLogicStep].scriptableCard.scriptableCardAbilities)
        {
            targetsList.Clear();
            //create the intend based on the ai choices
            GameObject intendObject = Instantiate(SystemManager.Instance.intendObject, intends.transform.position, Quaternion.identity);
            //parent it
            intendObject.transform.SetParent(intends.transform);

            IntendClass intendClass = intendObject.GetComponent<IntendClass>();
            intendClass.scriptableCard = cardScriptList[aiLogicStep].scriptableCard;


            //put the icon based on the intend
            Image intendImage = intendObject.transform.Find("Icon").GetComponent<Image>();
            TMP_Text intendText = intendObject.transform.Find("Text").GetComponent<TMP_Text>();

            intendText.text = "";
            //intendImage.sprite = GetIntendIcon(scriptableCardAbility);

            //put the icon based on the intend
            if (scriptableCardAbility.aIIntend == SystemManager.AIIntend.ATTACK)
            {
                intendImage.sprite = SystemManager.Instance.intend_Attack;
                intendText.text = scriptableCardAbility.GetAbilityVariable(cardScriptList[aiLogicStep]).ToString();
            }
            else if (scriptableCardAbility.aIIntend == SystemManager.AIIntend.MAGIC)
            {
                intendImage.sprite = SystemManager.Instance.intend_Magic;
                intendText.text = scriptableCardAbility.GetAbilityVariable(cardScriptList[aiLogicStep]).ToString();
            }
            else if (scriptableCardAbility.aIIntend == SystemManager.AIIntend.BUFF)
            {
                intendImage.sprite = SystemManager.Instance.intend_Buff;
                intendText.text = "<>";
               // intendText.gameObject.SetActive(false);
            }
            else if (scriptableCardAbility.aIIntend == SystemManager.AIIntend.DEBUFF)
            {
                intendImage.sprite = SystemManager.Instance.intend_Debuff;
                intendText.text = "<>";
                // intendText.gameObject.SetActive(false);
            }
            else if (scriptableCardAbility.aIIntend == SystemManager.AIIntend.CARDDECK)
            {
                intendImage.sprite = SystemManager.Instance.intend_CardDeck;
                //intendText.text = "T";
                intendText.gameObject.SetActive(false);
            }

            //get the target and the color
            if (scriptableCardAbility.aITypeOfAttack == SystemManager.AITypeOfAttack.AOE)
            {

                //check for what targets its the aoe
                if (scriptableCardAbility.aIWhoToTarget == SystemManager.AIWhoToTarget.PLAYER)
                {

                    //golden
                    intendText.color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorGolden);

                }
                else if(scriptableCardAbility.aIWhoToTarget == SystemManager.AIWhoToTarget.ENEMY)
                {

                    //dark grey
                    intendText.color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorDarkGrey);

                }
            }
            else if (scriptableCardAbility.aITypeOfAttack == SystemManager.AITypeOfAttack.SINGLETARGET)
            {
                //check for what targets its the aoe
                if (scriptableCardAbility.aIWhoToTarget == SystemManager.AIWhoToTarget.PLAYER)
                {

                    //add the target
                    GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");

                
                    //remove dead
                    foreach (GameObject target in targets)
                    {
                        if (target.GetComponent<EntityClass>().entityMode != SystemManager.EntityMode.DEAD)
                        {
                            targetsList.Add(target);
                        }
                    }

                    //targetsList = targets.ToList();

                    //get the random target
                    int indexTarget = Random.Range(0, targetsList.Count);
                    EntityClass entityClass = targetsList[indexTarget].GetComponent<EntityClass>();

                    //add them to the list
                    intendClass.target = targetsList[indexTarget];

                    //color of the character class
                    intendText.color = CardListManager.Instance.GetClassColor(entityClass.scriptableEntity.mainClass);
                }
                else if (scriptableCardAbility.aIWhoToTarget == SystemManager.AIWhoToTarget.ENEMY)
                {

                    //add the target
                    GameObject[] targets = GameObject.FindGameObjectsWithTag("Enemy");


                    //remove dead
                    foreach (GameObject target in targets)
                    {
                        if (target.GetComponent<EntityClass>().entityMode != SystemManager.EntityMode.DEAD)
                        {
                            targetsList.Add(target);
                        }
                    }

                    //targetsList = targets.ToList();

                    //get the random target
                    int indexTarget = Random.Range(0, targetsList.Count);
                    EntityClass entityClass = targetsList[indexTarget].GetComponent<EntityClass>();

                    //add them to the list
                    intendClass.target = targetsList[indexTarget];

                    //lighter grey
                    intendText.color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorLightGrey);
            
                }
            }

        }


    }

    public Sprite GetIntendIcon(ScriptableCardAbility scriptableCardAbility)
    {
        Sprite icon = null;

        //put the icon based on the intend
        if (scriptableCardAbility.aIIntend == SystemManager.AIIntend.ATTACK)
        {
            icon = SystemManager.Instance.intend_Attack;
        }
        else if (scriptableCardAbility.aIIntend == SystemManager.AIIntend.MAGIC)
        {
            icon = SystemManager.Instance.intend_Magic;
        }
        else if (scriptableCardAbility.aIIntend == SystemManager.AIIntend.BUFF)
        {
            icon = SystemManager.Instance.intend_Buff;
        }
        else if (scriptableCardAbility.aIIntend == SystemManager.AIIntend.DEBUFF)
        {
            icon = SystemManager.Instance.intend_Debuff;
        }

        return icon;
    }


}
