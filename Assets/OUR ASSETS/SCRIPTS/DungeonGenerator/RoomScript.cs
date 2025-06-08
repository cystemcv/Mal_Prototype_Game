using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class RoomScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SystemManager.PlanetTypes planetType;
    public bool roomCleared = false;

    public ScriptablePlanets scriptablePlanet;
    private bool isHovered = false;

    //shop
    public List<ShopData> shopCards = new List<ShopData>();
    public List<ShopData> shopArtifacts = new List<ShopData>();
    public List<ShopData> shopItems = new List<ShopData>();

    //rest
    public bool usedRest = false;

    public void Update()
    {
     
    }

    public void ClickedRoom(bool ignoreSpaceShip = false)
    {

        //SystemManager.Instance.object_HighlightButton.SetActive(false);

        //get the neighbor rooms
        //CustomDungeonGenerator.Instance.OnRoomClick(this.gameObject);

        if (!ignoreSpaceShip)
        {



            if (CustomDungeonGenerator.Instance.playerSpaceShipMoving)
            {
                return;
            }

            ItemManager.Instance.HideInventory();

            CombatManager.Instance.ScriptableEvent = null;
            CombatManager.Instance.scriptablePlanet = null;

            //CustomDungeonGenerator.Instance.DrawHighlightedPathLine(null); // Clear line

            CustomDungeonGenerator.Instance.StartPathTraversal(CustomDungeonGenerator.Instance.path, this);

        }
        else
        {

            if (CustomDungeonGenerator.Instance.playerSpaceShipMoving)
            {
                return;
            }

            ItemManager.Instance.HideInventory();

            CombatManager.Instance.ScriptableEvent = null;
            CombatManager.Instance.scriptablePlanet = null;

            CustomDungeonGenerator.Instance.ClickedRoom(this);
        }




        //change this object to clicked
        //this.gameObject.GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorTransparent);

    }




    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;

        CustomDungeonGenerator.Instance.lastHoveredRoomSecondary = this.gameObject;

        if (!CustomDungeonGenerator.Instance.playerSpaceShipMoving)
        {
            CustomDungeonGenerator.Instance.lastHoveredRoom = this.gameObject;
            AudioManager.Instance.PlaySfx("UI_goNext");
        }

    }



    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        if (!CustomDungeonGenerator.Instance.playerSpaceShipMoving)
        {
            CustomDungeonGenerator.Instance.DrawHighlightedPathLine(null); // Clear line
        }

        CustomDungeonGenerator.Instance.lastHoveredRoomSecondary = null;
        //SystemManager.Instance.object_HighlightButton.SetActive(false);
    }

}
