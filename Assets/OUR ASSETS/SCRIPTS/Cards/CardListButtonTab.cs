using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardListButtonTab : MonoBehaviour
{

    public bool isActive = false;

    public SystemManager.MainClass mainClass;

    public void FilterListForClass()
    {
        UIManager.Instance.FillCardListDataByClass(mainClass);
        UIManager.Instance.ResetCardList();
        UIManager.Instance.AssignCardsOnCardList();
    }

}
