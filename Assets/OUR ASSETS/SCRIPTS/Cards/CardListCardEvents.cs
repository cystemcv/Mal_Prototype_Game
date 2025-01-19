using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardListCardEvents : MonoBehaviour, IPointerDownHandler
{

    public GameObject markedGO;
    public ScriptableCard scriptableCard;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        if (UIManager.Instance.enableMaxSelection <= 0)
        {
            return;
        }


        if (!markedGO.activeSelf && UIManager.Instance.selectedCardList.Count < UIManager.Instance.enableMaxSelection)
        {
            markedGO.SetActive(true);
            UIManager.Instance.selectedCardList.Add(scriptableCard);
        }
        else if(markedGO.activeSelf)
        {
            markedGO.SetActive(false);
            UIManager.Instance.selectedCardList.Remove(scriptableCard);
        }

        GameObject selectionText = UIManager.Instance.cardListGO.transform.Find("Others").Find("SelectionText").gameObject;

        selectionText.GetComponent<TMP_Text>().text = UIManager.Instance.selectedCardList.Count + "/" + UIManager.Instance.enableMaxSelection;

    }
}
