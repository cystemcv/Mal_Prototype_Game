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

        StaticData.staticCharacter = scriptableEntity.Clone();

        //activate card ui
        activation.SetActive(true);

        //check if the limit is ok
        if (StaticData.staticCharacter != null)
        {
            //disable the proceed button
            UIManager.Instance.EnableButton(UI_CharacterSelectionMenu.Instance.proceedToGame);
        }
        else
        {
            UIManager.Instance.DisableButton(UI_CharacterSelectionMenu.Instance.proceedToGame);
        }

    }
}
