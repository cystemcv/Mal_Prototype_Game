using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MainMenuEasyScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UIManager.Instance.topPanelCombat.SetActive(false);
        UIManager.Instance.DisableAllUIScenes();
        UIManager.Instance.scenes_BG.SetActive(true);
        UIManager.Instance.scene_mainMenu.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
