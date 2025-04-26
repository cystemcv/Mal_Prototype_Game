using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UI_TopPanel : MonoBehaviour
{

    public GameObject topPanel;
    public bool openPanel = false;

    public void TogglePanel()
    {

        Animator animator = topPanel.GetComponent<Animator>();

        openPanel = !openPanel; 

        if (openPanel)
        {
            animator.SetTrigger("Open");
        }
        else
        {
            animator.SetTrigger("Close");
        }

    }
}
