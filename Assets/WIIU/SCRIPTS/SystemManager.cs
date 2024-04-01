using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
    public static SystemManager Instance;

    public GameObject uiManager;
    public GameObject audioManager;

    public enum SystemModes {
    
        PAUSED,
        PLAYING
    }

    public SystemModes currentSystemMode = SystemModes.PLAYING;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EnableDisable_UIManager(bool enable)
    {

        if (enable)
        {
            //make the default Main menu
            UIManager.Instance.NavigateMenu("MAIN MENU");
            uiManager.SetActive(true);
        }
        else
        {
            uiManager.SetActive(false);
        }

    }
    public void EnableDisable_AudioManager(bool enable)
    {
        if (enable)
        {
            audioManager.SetActive(true);
        }
        else
        {
            audioManager.SetActive(false);
        }
    }
}
