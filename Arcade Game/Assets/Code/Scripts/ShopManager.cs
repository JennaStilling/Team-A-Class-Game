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
    ShopLineItem[] shopItems;
    ShopSession currentSession;

    [SerializeField]
    TextMeshProUGUI ticketText;

    [SerializeField]
    private float distance;

    GameObject player;

    PlayerMovement playerMovement;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, distance);
    }

    public void Start()
    {
        player = GameObject.Find("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
        //find game manager or whatever has the ticket count

    }

    public void OpenShop(int tickets)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        playerMovement.CanMoveProp = false;
        canvas.gameObject.SetActive(true);
        ticketText.text = "Tickets: " + tickets.ToString();
        currentSession = new ShopSession(shopItems, panel, shopLineItemUI, ticketText);
        currentSession.StartSession(tickets);
    }

    public void CloseShop()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerMovement.CanMoveProp = true;
        canvas.gameObject.SetActive(false);
        if (currentSession != null)
        {
            currentSession.EndSession();
        }
    }

    public bool TryOpenShop(int tickets)
    {
        if (Vector3.Distance(player.transform.position, transform.position) <= distance) // check distance from player
        {
            if (!canvas.gameObject.activeSelf)
            {
                Debug.Log("Open Menu");
                OpenShop(tickets);
                Debug.Log("starting tickets" + tickets);
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
        if (Input.GetKeyDown(KeyCode.O))
        {
            TryOpenShop(1000);
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            if (canvas.gameObject.activeSelf)
            {
                CloseShop();
            }
        }
    }
}