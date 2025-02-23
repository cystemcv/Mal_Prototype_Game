using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class RoomScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SystemManager.PlanetTypes planetType;
    public bool roomCleared = false;

    public ScriptablePlanets scriptablePlanet;

    public void ClickedRoom()
    {

        //SystemManager.Instance.object_HighlightButton.SetActive(false);

        //get the neighbor rooms
        //CustomDungeonGenerator.Instance.OnRoomClick(this.gameObject);

        ItemManager.Instance.HideInventory();


        if (roomCleared)
        {
            return;
        }

        if (planetType == SystemManager.PlanetTypes.BATTLE)
        {

            CombatManager.Instance.scriptablePlanet = scriptablePlanet;
            CombatManager.Instance.planetClicked = this.gameObject;

            SystemManager.Instance.LoadScene("scene_Combat", 0f,true,true);
        }
        else if (planetType == SystemManager.PlanetTypes.EVENT)
        {

            //CombatManager.Instance.scriptablePlanet = scriptablePlanet;
            CombatManager.Instance.planetClicked = this.gameObject;

            int randomIndex = Random.Range(0, CustomDungeonGenerator.Instance.galaxyGenerating.scriptableEventList.Count);

            UIManager.Instance.ShowEventGo(CustomDungeonGenerator.Instance.galaxyGenerating.scriptableEventList[randomIndex]);

            //SystemManager.Instance.LoadScene("scene_Combat", 0f, true, true);
        }
        else
        {

            ItemManager.Instance.ActivateItemList(SystemManager.ActivationType.OnNonCombatRoom);

            CustomDungeonGenerator.Instance.OnRoomClick(this.gameObject);


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
