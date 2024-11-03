using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceManagers : MonoBehaviour
{

    private bool inventoryToggle = false;


    //all methods that should reference managers should go here!

    public void OpenUIMenu()
    {

        AudioManager.Instance.PlaySfx("UI_goNext");

        SystemManager.Instance.EnableDisable_UIManager(true);
    }

    public void CloseUIMenu()
    {
        AudioManager.Instance.PlaySfx("UI_goBack");

        SystemManager.Instance.EnableDisable_UIManager(false);
    }

    public void ToggleInventoryParent()
    {

        inventoryToggle = !inventoryToggle; // Toggle the value between true and false
        Debug.Log("Inventory Toggle State: " + inventoryToggle); // Log the state for debugging


        if (inventoryToggle)
        {
            //show
            ShowInventoryReference();
        }
        else
        {
            //hide
            HideInventoryReference();
        }


    }

    public void ShowInventoryReference()
    {
        ItemManager.Instance.ShowInventoryParent();
    }

    public void HideInventoryReference()
    {
        ItemManager.Instance.HideInventoryParent();
    }

}
