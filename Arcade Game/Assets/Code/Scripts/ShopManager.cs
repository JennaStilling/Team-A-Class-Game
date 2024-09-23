using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField]
    Canvas canvas;

    [SerializeField]
    GameObject panel;

    [SerializeField]
    GameObject shopLineItemUI;

    [SerializeField]
    ShopLineItem[] shopItems;

    [SerializeField]
    TextMeshProUGUI ticketText;

    [SerializeField]
    private float distance;

    GameObject player;

    PlayerMovement playerMovement;

    private GameManager _gameManager;

    private UIManager _uIManager;

    private int localTickets;

    private GameObject[] ShopLineItemsUI;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, distance);
    }

    public void Start()
    {
        player = GameObject.Find("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
        _gameManager = FindObjectOfType<GameManager>();
        _uIManager = FindObjectOfType<UIManager>();
    }

    public void OpenShop(int tickets)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        playerMovement.CanMoveProp = false;
        canvas.gameObject.SetActive(true);
        ticketText.text = "Tickets: " + tickets.ToString();
        StartSession(tickets);
    }

    public void CloseShop()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerMovement.CanMoveProp = true;
        _gameManager.ticketValueProp = EndSession();
        canvas.gameObject.SetActive(false);
    }

    public bool TryOpenShop(int tickets)
    {
        if (Vector3.Distance(player.transform.position, transform.position) <= distance) // check distance from player
        {
            if (!canvas.gameObject.activeSelf)
            {
                OpenShop(_gameManager.GetTicketValue());
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryOpenShop(1000);
        }
        else if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (canvas.gameObject.activeSelf)
            {
                CloseShop();
            }
        }
    }


    private void StartSession(int startingTickets)
    {
        ShopLineItemsUI = new GameObject[shopItems.Length];

        localTickets = startingTickets;

        int offset = 0;
        foreach (ShopLineItem lineItem in shopItems)
        {
            int unboundOffset = offset;
            GameObject lineItemO = Instantiate(shopLineItemUI, panel.transform);

            lineItemO.transform.Find("BuyButton").gameObject.GetComponent<Button>().onClick.AddListener(() => PurchaseItem(unboundOffset));
            lineItemO.transform.Translate(Vector3.down * 100 * offset);

            ShopLineItemsUI[offset] = lineItemO;
            offset++;
        }

        RefreshShopUI();
    }

    private void RefreshShopUI() 
    {
        int offset = 0;
        foreach (ShopLineItem lineItem in shopItems)
        {
            GameObject lineItemO = ShopLineItemsUI[offset];

            lineItemO.transform.Find("CostText").GetComponent<TextMeshProUGUI>().text = lineItem.ShopItemProp.CostProp.ToString();
            lineItemO.transform.Find("NameText").GetComponent<TextMeshProUGUI>().text = lineItem.ShopItemProp.ItemNameProp;
            lineItemO.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>().text = lineItem.ShopItemProp.ItemDescriptionProp;
            lineItemO.transform.Find("StockText").GetComponent<TextMeshProUGUI>().text = lineItem.AmountLeftProp.ToString();
            //lineItem.transform.Find("ItemImage").GetComponent<Image>().sprite = shopItems[i].getImage();

            //make ui element for line item
            if (!lineItem.IsAvailable())
            {
                //set ui for sold out items
            }
            else if (true || lineItem.ShopItemProp.CostProp > localTickets)
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
        }

        localTickets -= shopItems[itemPosition].Purchase(localTickets);

        ShopLineItemsUI[itemPosition].transform.Find("StockText").GetComponent<TextMeshProUGUI>().text = shopItems[itemPosition].AmountLeftProp.ToString();

        ticketText.text = "Tickets: " + localTickets.ToString();

        RefreshShopUI();

        return true;
    }

    private int EndSession()
    {
        foreach (GameObject lineItem in ShopLineItemsUI)
        {
            Destroy(lineItem);
        }

        return localTickets;
    }
}