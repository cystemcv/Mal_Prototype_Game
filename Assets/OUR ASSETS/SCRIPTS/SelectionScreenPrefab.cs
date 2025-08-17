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



            if (scriptableEntity.entityName == StaticData.staticCharacter.entityName)
            {
                this.gameObject.transform.Find("Outline").GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorYellow);
            }
            else
            {
                this.gameObject.transform.Find("Outline").GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorWhite);
            }



    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        onHover = true;

        UI_CharacterSelectionMenu.Instance.OnHoverCharacterPanel(scriptableEntity);

        AudioManager.Instance.PlaySfx("UI_Confirm");
        this.gameObject.transform.Find("Outline").GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorYellow);
    }

    public void OnPointerExit(PointerEventData eventData)
    {

        onHover = false;

        UI_CharacterSelectionMenu.Instance.OnHoverCharacterPanel(StaticData.staticCharacter);

        this.gameObject.transform.Find("Outline").GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorWhite);
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
