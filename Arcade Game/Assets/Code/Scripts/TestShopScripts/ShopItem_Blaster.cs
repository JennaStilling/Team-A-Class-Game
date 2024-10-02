using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem_Blaster : ShopItem
{
    public override void PurchaseEffect()
    {
        FindObjectOfType<WeaponManager>().gunUnlocked = true;
    }
}
