using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea]
    public string tooltipContent;

    public void OnPointerEnter(PointerEventData eventData)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 hoverPosition = eventData.position; // Position of the hovered object
        Vector2 hoverSize = rectTransform.rect.size; // Size of the hovered object

        Vector2 mousePosition = Input.mousePosition;
        UIManager.Instance.ShowTooltip(tooltipContent, hoverPosition, hoverSize, this.gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.HideTooltip();
    }
}
