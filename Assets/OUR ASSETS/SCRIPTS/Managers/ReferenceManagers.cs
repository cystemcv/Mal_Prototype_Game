using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceManagers : MonoBehaviour
{

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

}
