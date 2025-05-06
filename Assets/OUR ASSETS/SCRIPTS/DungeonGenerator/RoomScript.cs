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



    public void ClickedRoom()
    {

        //SystemManager.Instance.object_HighlightButton.SetActive(false);

        //get the neighbor rooms
        //CustomDungeonGenerator.Instance.OnRoomClick(this.gameObject);

        ItemManager.Instance.HideInventory();

        CombatManager.Instance.ScriptableEvent = null;
        CombatManager.Instance.scriptablePlanet = null;

        if (roomCleared)
        {
            return;
        }

        if (planetType == SystemManager.PlanetTypes.BATTLE)
        {

            CombatManager.Instance.scriptablePlanet = scriptablePlanet;
            CombatManager.Instance.planetClicked = this.gameObject;

            SystemManager.Instance.LoadScene("scene_Combat", 0.5f,true,false);
        }
        else if (planetType == SystemManager.PlanetTypes.EVENT)
        {

  

            int randomIndex = Random.Range(0, CustomDungeonGenerator.Instance.galaxyGenerating.scriptableEventList.Count);
            ScriptableEvent scriptableEvent = CustomDungeonGenerator.Instance.galaxyGenerating.scriptableEventList[randomIndex];

            CombatManager.Instance.scriptablePlanet = scriptableEvent.scriptablePlanet;
            CombatManager.Instance.ScriptableEvent = scriptableEvent;
            CombatManager.Instance.planetClicked = this.gameObject;

            SystemManager.Instance.LoadScene("scene_Combat", 0.5f, true, false);

            //UIManager.Instance.ShowEventGo(CustomDungeonGenerator.Instance.galaxyGenerating.scriptableEventList[randomIndex]);

            //SystemManager.Instance.LoadScene("scene_Combat", 0f, true, true);
        }
        else
        {

            ItemManager.Instance.ActivateItemList(SystemManager.ActivationType.OnNonCombatRoom,null);

            CustomDungeonGenerator.Instance.OnRoomClick(this.gameObject);


        }



        //change this object to clicked
        //this.gameObject.GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorTransparent);

    }


    private void Update()
    {
        if (isHovered && !CustomDungeonGenerator.Instance.playerSpaceShipMoving)
        {
            List<GameObject> path = CustomDungeonGenerator.Instance.GetShortestVisiblePath(CustomDungeonGenerator.Instance.playerSpaceShip.GetComponent<DungeonShipController>().planetLanded, this.gameObject);
            Debug.Log("Path count: " + (path == null ? "null" : path.Count.ToString()));
            CustomDungeonGenerator.Instance.DrawHighlightedPathLine(path);

            CustomDungeonGenerator.Instance.StartPathTraversal(path);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;

        List<GameObject> path = CustomDungeonGenerator.Instance.GetShortestVisiblePath( CustomDungeonGenerator.Instance.playerSpaceShip.GetComponent<DungeonShipController>().planetLanded , this.gameObject);
        Debug.Log("Path count: " + (path == null ? "null" : path.Count.ToString()));
        CustomDungeonGenerator.Instance.DrawHighlightedPathLine(path);

        CustomDungeonGenerator.Instance.StartPathTraversal(path);


        AudioManager.Instance.PlaySfx("UI_goNext");

    }



    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        CustomDungeonGenerator.Instance.DrawHighlightedPathLine(null); // Clear line
        //SystemManager.Instance.object_HighlightButton.SetActive(false);
    }

}
