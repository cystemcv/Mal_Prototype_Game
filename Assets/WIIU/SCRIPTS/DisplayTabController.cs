using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.MUIP;

public class DisplayTabController : MonoBehaviour
{

    public static DisplayTabController Instance;

    public CustomDropdown selectScreenMode;
    public CustomDropdown selectScreenResolution;

    public FullScreenMode screenMode;

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


    public void SelectScreenResolution(bool playSound = true)
    {
        if (playSound)
        {
            AudioManager.Instance.PlaySfx("UI_Confirm");
        }

        string[] split = selectScreenResolution.selectedText.text.Split("-");
        string resolution = split[1].Trim();

        string[] splitWH = resolution.Split("x");
        string width = splitWH[0];
        string height = splitWH[1];

        Screen.SetResolution(int.Parse(width), int.Parse(height), screenMode);
        Debug.Log("screen resolution");
    }

    public void SelectScreenMode(bool playSound = true)
    {
        if (playSound)
        {
            AudioManager.Instance.PlaySfx("UI_Confirm");
        }
    

        if (selectScreenMode.selectedText.text == "Full Screen Window")
        {
            screenMode = FullScreenMode.FullScreenWindow;
        }
        else if (selectScreenMode.selectedText.text == "Exclusive Full Screen")
        {
            screenMode = FullScreenMode.ExclusiveFullScreen;
        }
        else if (selectScreenMode.selectedText.text == "Maximized Window")
        {
            screenMode = FullScreenMode.MaximizedWindow;
        }
        else if (selectScreenMode.selectedText.text == "Windowed")
        {
            screenMode = FullScreenMode.Windowed;
        }

        Debug.Log("screen mode");

        Screen.fullScreenMode = screenMode;
    }
}
