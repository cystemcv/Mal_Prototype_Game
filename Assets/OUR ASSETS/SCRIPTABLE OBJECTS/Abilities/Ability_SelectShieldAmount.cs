using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Ability_SelectShieldAmount", menuName = "CardAbility/Ability_SelectShieldAmount")]
public class Ability_SelectShieldAmount : ScriptableCardAbility
{

    [Header("UNIQUE")]
    public List<int> shieldAmountList;
    public GameObject cardPrefabShield;

    public override string AbilityDescription(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity)
    {
        string keyword = base.AbilityDescription(cardScript, cardAbilityClass, entity);
        string description = "";

        description = "Add ";
        foreach (int shieldAmount in shieldAmountList)
        {
            description += shieldAmount + "/";
        }
        //remove the last comma
        description = description.Remove(description.Length - 1, 1);

        description += " shield to target";

        string final = keyword + " : " + description;

        return final;
    }



    public override void OnPlayCard(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity, SystemManager.ControlBy controlBy)
    {
        base.OnPlayCard(cardScript, cardAbilityClass, entity, controlBy);



        //display screen
        UIManager.Instance.ChooseGroupUI.SetActive(true);

        //change the mode
        SystemManager.Instance.abilityMode = SystemManager.AbilityModes.SHIELDCHOICE;

        foreach (int shieldAmount in shieldAmountList)
        {

            if (cardPrefabShield == null)
            {
                break;
            }

            GameObject tempCardPrefab = GameObject.Instantiate(cardPrefabShield, UIManager.Instance.ChooseGroupUI.transform.Find("ChooseContainer").gameObject.transform.position, Quaternion.identity);
            tempCardPrefab.transform.Find("Panel").Find("DescriptionBg").Find("DescriptionText").GetComponent<TMP_Text>().text = "Add " + shieldAmount.ToString() + " shield";
            tempCardPrefab.transform.Find("Panel").Find("CardImageText").GetComponent<TMP_Text>().text = shieldAmount.ToString();
            tempCardPrefab.transform.Find("Panel").Find("TitleBg").Find("TitleText").GetComponent<TMP_Text>().text = "Shield Adjust";
            tempCardPrefab.transform.Find("Panel").Find("TypeBg").Find("TypeText").GetComponent<TMP_Text>().text = "Shield Choice";

            tempCardPrefab.GetComponent<CardScript>().tempValue = shieldAmount;

            //set it as a child of the parent
            tempCardPrefab.transform.SetParent(UIManager.Instance.ChooseGroupUI.transform.Find("ChooseContainer").gameObject.transform);

            //make the local scale 1,1,1
            tempCardPrefab.transform.localScale = new Vector3(1, 1, 1);

            tempCardPrefab.GetComponent<CardEvents>().sortOrder = 1200;
            tempCardPrefab.GetComponent<Canvas>().sortingOrder = 1200;
        }



    }




}
