using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem_DamageMultiplier : ShopItem
{
    [SerializeField]
    private float damageMultiplier;
    public override void PurchaseEffect()
    {
        FindObjectOfType<RayCast>().damage *= damageMultiplier;
    }
}
