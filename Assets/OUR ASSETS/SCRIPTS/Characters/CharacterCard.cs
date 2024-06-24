using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterCard : MonoBehaviour
{

    public Image bgImage;
    public GameObject activation;
    public Image cardImage;
    public TMP_Text cardName;
    public TMP_Text cardDescription;

    public ScriptableEntity scriptableEntity;

    [Header("ID")]
    public string characterID;

    public CharacterCard()
    {

        //add an id to this scriptableCard, this is in order to identify it by comparisons
        characterID = System.Guid.NewGuid().ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (scriptableEntity == null)
        {
            return;
        }

        bgImage = this.gameObject.transform.Find("MainBg").GetComponent<Image>();
        activation = this.gameObject.transform.Find("Activation").gameObject;
        cardImage = this.gameObject.transform.Find("CardImage").GetComponent<Image>();
        cardName = this.gameObject.transform.Find("TitleBg").Find("TitleText").GetComponent<TMP_Text>();
        cardDescription = this.gameObject.transform.Find("DescriptionBg").Find("DescriptionText").GetComponent<TMP_Text>();

        bgImage.color = CardListManager.Instance.GetClassColor(scriptableEntity.mainClass);
        activation.SetActive(false);
        cardImage.sprite = scriptableEntity.entityImage;
        cardName.text = scriptableEntity.mainClass.ToString();
        cardDescription.text = scriptableEntity.entityDescription;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnCharacterCardClick()
    {

        //check if the character is already on the list
        if (CharacterManager.Instance.scriptablePlayerList.Contains(scriptableEntity))
        {

            //remove it from the list
            CharacterManager.Instance.scriptablePlayerList.Remove(scriptableEntity);

            //deactivate card ui
            activation.SetActive(false);

            //disable the proceed button
            UIManager.Instance.DisableButton(UIManager.Instance.proceedToGame);

            return;

        }

        //check if we can add more characters otherwise return
        if (CharacterManager.Instance.scriptablePlayerList.Count + 1 > CharacterManager.Instance.CheckCharacterLimitBasedOnMode())
        {
            //passed the limit

            //then we remove the first one and add this one

            //find the object that has that scriptable character
            CharacterCard characterCardFound = null;
            foreach(Transform card in UIManager.Instance.characterSelectionContent.transform)
            {
                characterCardFound = card.GetChild(0).GetComponent<CharacterCard>();

                if (characterCardFound.scriptableEntity == CharacterManager.Instance.scriptablePlayerList[0])
                {
                    //get out of the loop we found what we need
                    break;
                }
            }
            characterCardFound.activation.SetActive(false);
            CharacterManager.Instance.scriptablePlayerList.RemoveAt(0);



            //return;
        }

        //add to the list
        CharacterManager.Instance.scriptablePlayerList.Add(scriptableEntity);

        //activate card ui
        activation.SetActive(true);

        //check if the limit is ok
        if (CharacterManager.Instance.scriptablePlayerList.Count == CharacterManager.Instance.CheckCharacterLimitBasedOnMode())
        {
            //disable the proceed button
            UIManager.Instance.EnableButton(UIManager.Instance.proceedToGame);
        }
        else
        {
            UIManager.Instance.DisableButton(UIManager.Instance.proceedToGame);
        }

    }
}
