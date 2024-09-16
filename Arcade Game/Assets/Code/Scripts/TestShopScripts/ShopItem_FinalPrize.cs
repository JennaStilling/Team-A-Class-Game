using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem_FinalPrize : ShopItem
{
    public override void PurchaseEffect()
    {
        WinGame();
    }

    private void WinGame() 
    {
        ShopManager shopManager = FindObjectOfType<ShopManager>();

        GameObject winScreen = GameObject.Find("WinScreen");
        winScreen.transform.Find("WinText").gameObject.SetActive(true);

        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        playerMovement.CanMoveProp = false;

        shopManager.CloseShop();
    }
}
