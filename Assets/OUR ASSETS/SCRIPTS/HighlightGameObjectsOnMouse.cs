using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HighlightGameObjectsOnMouse : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {

        SystemManager.Instance.ChangeTargetMaterial(SystemManager.Instance.materialTargetEntity, this.gameObject);
        SystemManager.Instance.ChangeTargetEntityColor(SystemManager.Instance.colorVeryLightBlue, this.gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SystemManager.Instance.ChangeTargetMaterial(SystemManager.Instance.materialDefaultEntity, this.gameObject);
        SystemManager.Instance.ChangeTargetEntityColor(SystemManager.Instance.colorWhite, this.gameObject);
    }
}
