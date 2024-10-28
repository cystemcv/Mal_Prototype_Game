using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        if (onHover)
        {
            return;
        }

        if (typeOfPrefab == SystemManager.SelectionScreenPrefabType.CHARACTER)
        {

            if (scriptableEntity.entityName == StaticData.staticCharacter.entityName)
            {
                SystemManager.Instance.ChangeTargetMaterial(SystemManager.Instance.materialTargetEntity, this.gameObject);
            }
            else
            {
                SystemManager.Instance.ChangeTargetMaterial(SystemManager.Instance.materialDefaultEntity, this.gameObject);
            }

        }
        else
        {
            if (scriptableCompanion.companionName == StaticData.staticScriptableCompanion.companionName)
            {
                SystemManager.Instance.ChangeTargetMaterial(SystemManager.Instance.materialTargetEntity, this.gameObject);
            }
            else
            {
                SystemManager.Instance.ChangeTargetMaterial(SystemManager.Instance.materialDefaultEntity, this.gameObject);
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

        SystemManager.Instance.ChangeTargetMaterial(SystemManager.Instance.materialMouseOverEntity, this.gameObject);
        //SystemManager.Instance.ChangeTargetEntityColor(SystemManager.Instance.colorVeryLightBlue, this.gameObject);



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

        SystemManager.Instance.ChangeTargetMaterial(SystemManager.Instance.materialDefaultEntity, this.gameObject);
        //SystemManager.Instance.ChangeTargetEntityColor(SystemManager.Instance.colorWhite, this.gameObject);
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
    }

}
