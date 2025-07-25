using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopCard : MonoBehaviour, IPointerClickHandler
{

    public ShopData shopData;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        BuyCard();
    }

    public void BuyCard()
    {
        //if enough gold
        if (ItemManager.Instance.CanPlayerBuy(shopData.shopCostItem) && shopData.itemAvailable)
        {

            //add sold on card
            this.gameObject.transform.GetChild(0).Find("UtilityFront").Find("SoldOut").gameObject.SetActive(true);
            this.gameObject.transform.GetChild(0).Find("UtilityFront").Find("GoldPanel").Find("Text").GetComponent<TMP_Text>().text = "SOLD";

            shopData.itemAvailable = false;

            //decrease the gold from player
            ItemManager.Instance.AddRemoveInventoryItemInListByScriptableName("Gold", shopData.shopCostItem * -1);

            //show the new shop gold

            //get player gold
            int playerGold = ItemManager.Instance.GetItemQuantity("Gold", StaticData.inventoryItemList);

            UIManager.Instance.shopUI.transform.Find("PlayerGold").GetComponent<TMP_Text>().text = playerGold.ToString();

            //add card on deck
            DeckManager.Instance.AddCardOnDeck(shopData.scriptableCard,0);

        }
        else
        {
            //player cannot buy
        }

    }
}
