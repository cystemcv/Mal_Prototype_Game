using UnityEngine;
using UnityEngine.EventSystems;

public class RoomScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SystemManager.RoomType roomType;
    public bool roomCleared = false;

    public void ClickedRoom()
    {

        //SystemManager.Instance.object_HighlightButton.SetActive(false);

        //get the neighbor rooms
        CustomDungeonGenerator.Instance.OnRoomClick(this.gameObject);

        if (roomType == SystemManager.RoomType.Battle) {
            Combat.Instance.StartCombat();
        }



        //change this object to clicked
        //this.gameObject.GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorTransparent);

    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        //SystemManager.Instance.object_HighlightButton.SetActive(false);

        //SystemManager.Instance.object_HighlightButton.transform.position = this.gameObject.transform.position;

        //SystemManager.Instance.object_HighlightButton.GetComponent<ImprovedRadialFillController>().InitMaterial();

        //SystemManager.Instance.object_HighlightButton.SetActive(true);

        AudioManager.Instance.PlaySfx("UI_goNext");

    }



    public void OnPointerExit(PointerEventData eventData)
    {
        //SystemManager.Instance.object_HighlightButton.SetActive(false);
    }

}