using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UI_Menus : MonoBehaviour
{
    public static UI_Menus Instance;

    public List<GameObject> list_goMenuItem;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }


    }




}
