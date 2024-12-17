using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SelectionScreenPrefab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    public ScriptableEntity scriptableEntity;
    public ScriptableCompanion scriptableCompanion;

    public SystemManager.SelectionScreenPrefabType typeOfPrefab = SystemManager.SelectionScreenPrefabType.CHARACTER;

    public bool onHover = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


        if (typeOfPrefab == SystemManager.SelectionScreenPrefabType.CHARACTER)
        {

            if (scriptableEntity.entityName == StaticData.staticCharacter.entityName)
            {
                this.gameObject.transform.Find("Background").GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorYellow);
            }
            else
            {
                this.gameObject.transform.Find("Background").GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorWhite);
            }

        }
        else
        {
            if (scriptableCompanion.companionName == StaticData.staticScriptableCompanion.companionName)
            {
                this.gameObject.transform.Find("Background").GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorYellow);
            }
            else
            {
                this.gameObject.transform.Find("Background").GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorWhite);
            }
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        onHover = true;



        if (typeOfPrefab == SystemManager.SelectionScreenPrefabType.CHARACTER)
        {
            UI_CharacterSelectionMenu.Instance.characterTitle.GetComponent<TMP_Text>().text = scriptableEntity.entityName;
            UI_CharacterSelectionMenu.Instance.characterDescription.GetComponent<TMP_Text>().text = scriptableEntity.entityDescription;
        }
        else
        {
            UI_CharacterSelectionMenu.Instance.companionTitle.GetComponent<TMP_Text>().text = scriptableCompanion.companionName;
            UI_CharacterSelectionMenu.Instance.companionDescription.GetComponent<TMP_Text>().text = scriptableCompanion.companionDescription;
        }



        AudioManager.Instance.PlaySfx("UI_Confirm");
        this.gameObject.transform.Find("Highlight").GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorYellow);
    }

    public void OnPointerExit(PointerEventData eventData)
    {

        onHover = false;

        if (typeOfPrefab == SystemManager.SelectionScreenPrefabType.CHARACTER)
        {
            UI_CharacterSelectionMenu.Instance.characterTitle.GetComponent<TMP_Text>().text = StaticData.staticCharacter.entityName;
            UI_CharacterSelectionMenu.Instance.characterDescription.GetComponent<TMP_Text>().text = StaticData.staticCharacter.entityDescription;
        }
        else
        {
            UI_CharacterSelectionMenu.Instance.companionTitle.GetComponent<TMP_Text>().text = StaticData.staticScriptableCompanion.companionName;
            UI_CharacterSelectionMenu.Instance.companionDescription.GetComponent<TMP_Text>().text = StaticData.staticScriptableCompanion.companionDescription;
        }

        this.gameObject.transform.Find("Highlight").GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorWhite);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (typeOfPrefab == SystemManager.SelectionScreenPrefabType.CHARACTER)
        {
              StaticData.staticCharacter = scriptableEntity.Clone();
        }
        else
        {
            StaticData.staticScriptableCompanion = scriptableCompanion.Clone();
        }


        AudioManager.Instance.PlaySfx("UI_goNext");
    }

}
