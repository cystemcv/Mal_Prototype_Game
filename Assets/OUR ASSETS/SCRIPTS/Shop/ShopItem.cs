using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopItem : MonoBehaviour, IPointerClickHandler
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
        BuyItem();
    }

    public void BuyItem()
    {
        int originalPrice = shopData.shopCostItem;
        int newPrice = UIManager.Instance.GetRealPrice(originalPrice);

        //if enough gold
        if (ItemManager.Instance.CanPlayerBuy(newPrice) && shopData.itemAvailable)
        {

            //add sold on card
            this.gameObject.transform.Find("SoldOut").gameObject.SetActive(true);
            this.gameObject.transform.Find("GoldPanel").Find("Text").GetComponent<TMP_Text>().text = "SOLD";

            shopData.itemAvailable = false;

            //decrease the gold from player
            ItemManager.Instance.AddRemoveInventoryItemInListByScriptableName("Gold", newPrice * -1);

            //show the new shop gold

            //get player gold
            int playerGold = ItemManager.Instance.GetItemQuantity("Gold", StaticData.inventoryItemList);

            UIManager.Instance.shopUI.transform.Find("PlayerGold").GetComponent<TMP_Text>().text = playerGold.ToString();

            //add card on deck
            ClassItemData classItemData = new ClassItemData(shopData.scriptableItem, shopData.shopQuantityItem);

            ItemManager.Instance.AddRemoveInventoryItemInList(classItemData);

            UIManager.Instance.RefreshEveryShopItem();



        }
        else
        {
            //player cannot buy
        }

    }

}
