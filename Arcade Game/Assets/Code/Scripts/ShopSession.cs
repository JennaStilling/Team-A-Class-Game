using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSession : MonoBehaviour
{
    ShopLineItem[] shopItems;
    private int tickets;

    GameObject panel;
    GameObject shopLineItemUI;

    GameObject[] ShopLineItemsUI;

    TextMeshProUGUI _ticketText;

    public ShopSession(ShopLineItem[] shopItems, GameObject panel, GameObject shopLineItemUI, TextMeshProUGUI ticketText) 
    {
        this.shopItems = shopItems;
        this.panel = panel;
        this.shopLineItemUI = shopLineItemUI;
        this._ticketText = ticketText;
    }
    
    public void StartSession(int startingTickets) 
    {
        ShopLineItemsUI = new GameObject[shopItems.Length];

        tickets = startingTickets;

        int offset = 0;
        foreach (ShopLineItem lineItem in shopItems)
        {
            int unboundOffset = offset;

            //make ui element for line item
            GameObject lineItemO = Instantiate(shopLineItemUI, panel.transform);

            lineItemO.transform.Find("CostText").GetComponent<TextMeshProUGUI>().text = lineItem.ShopItemProp.CostProp.ToString();
            lineItemO.transform.Find("NameText").GetComponent<TextMeshProUGUI>().text = lineItem.ShopItemProp.ItemNameProp;
            lineItemO.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>().text = lineItem.ShopItemProp.ItemDescriptionProp;
            lineItemO.transform.Find("StockText").GetComponent<TextMeshProUGUI>().text = lineItem.AmountLeftProp.ToString();
            //lineItem.transform.Find("ItemImage").GetComponent<Image>().sprite = shopItems[i].getImage();

            Debug.Log(offset);

            lineItemO.transform.Find("BuyButton").gameObject.GetComponent<Button>().onClick.AddListener(() => PurchaseItem(unboundOffset));

            lineItemO.transform.Translate(Vector3.down * 100 * offset);
            
            ShopLineItemsUI[offset] = lineItemO;

            

            

            //make ui element for line item
            if (!lineItem.IsAvailable())
            {
                //set ui for sold out items
            }
            else if (true || lineItem.ShopItemProp.CostProp > tickets)
            {
                //set ui for too expensive items
            }
            else //probably not needed
            { 
                // set ui for available items
            }
            offset++;
        }
    }

    public bool PurchaseItem(int itemPosition) 
    {
        if (!shopItems[itemPosition].IsAvailable()) 
        {
            return false;
            //disable when you cant buy, perhaps seperate check for out of stock vs not enough tickets
        }
        
        tickets -= shopItems[itemPosition].Purchase(tickets);

        ShopLineItemsUI[itemPosition].transform.Find("StockText").GetComponent<TextMeshProUGUI>().text = shopItems[itemPosition].AmountLeftProp.ToString();

        _ticketText.text = "Tickets: " + tickets.ToString();

        return true;
    }

    public int EndSession()
    {
        foreach (Transform lineItem in panel.transform) 
        {
            Destroy(lineItem.gameObject);
        }

        return tickets;
    }
}