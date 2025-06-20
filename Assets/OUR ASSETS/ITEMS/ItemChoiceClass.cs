using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemChoiceClass : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{

    public ClassItemData classItem;
    private float hoverChoiceScale = 1.2f;
    private float transitionTime = 0.1f;

    public LTDescr scaleTween;
    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Scale up the hovered card
        scaleTween = LeanTween.scale(this.gameObject, originalScale * hoverChoiceScale, transitionTime);
        gameObject.transform.eulerAngles = new Vector3(0, 0, 0);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Scale down the card
        scaleTween = LeanTween.scale(this.gameObject, originalScale, transitionTime);
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        if (classItem.scriptableItem.itemCategory == SystemManager.ItemCategory.ARTIFACT)
        {
            ItemManager.Instance.AddArtifactItemInList(classItem);

            ItemManager.Instance.RefreshArtifacts();
        }
        else
        {
            //add to companion list
            ItemManager.Instance.AddCompanionItemInList(classItem);

            ItemManager.Instance.RefreshCompanion();
        }


        UIManager.Instance.ChooseGroupUI.SetActive(false);

    }
}
