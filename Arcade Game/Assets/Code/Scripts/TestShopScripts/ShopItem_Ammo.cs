using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem_Ammo : ShopItem
{
    [SerializeField]
    private int refillAmount;
    public override void PurchaseEffect()
    {
        FindObjectOfType<WeaponManager>().blasterShots += refillAmount;
    }
}
