using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField]
    Canvas canvas;
    [SerializeField]
    GameObject panel;

    [SerializeField]
    GameObject shopLineItemUI;

    [SerializeField] 
    ShopLineItem [] shopItems;
    ShopSession currentSession;

    [SerializeField]
    TextMeshProUGUI ticketText;

    public void OpenShop(int tickets)
    {
        canvas.gameObject.SetActive(true);
        ticketText.text = "Tickets: " + tickets.ToString();
        currentSession = new ShopSession(shopItems, panel, shopLineItemUI, ticketText);
        currentSession.StartSession(tickets);
    }

    public int CloseShop() 
    {
        canvas.gameObject.SetActive(false);
        return currentSession.EndSession();
    }

    void Update()
    {
        int testTickets = 1000;
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (!canvas.gameObject.activeSelf)
            {
                Debug.Log("Open Menu");
                OpenShop(testTickets);
                Debug.Log("starting tickets" + testTickets);
            }
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            if (canvas.gameObject.activeSelf)
            {
                Debug.Log("Close Menu");
                testTickets = CloseShop();
                Debug.Log("ending tickets " + testTickets);
            }
        }
    }
}