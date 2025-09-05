using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemChoiceClass : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{

    public ClassItemData classItem;
    public float hoverChoiceScale = 1.3f;
    public float transitionTime = 0.1f;

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

        gameObject.transform.Find("Panel").Find("UtilityBack").Find("ChooseActivation").gameObject.SetActive(true);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Scale down the card
        scaleTween = LeanTween.scale(this.gameObject, originalScale, transitionTime);
        gameObject.transform.Find("Panel").Find("UtilityBack").Find("ChooseActivation").gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        if (classItem.scriptableItem.itemCategory == SystemManager.ItemCategory.ARTIFACT)
        {
            ItemManager.Instance.AddArtifactItemInList(classItem);

            ItemManager.Instance.RefreshArtifacts();
            UIManager.Instance.DetermineCorrectUI();
        }

        UIManager.Instance.ChooseGroupUI.SetActive(false);

    }
}
