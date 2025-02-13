using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShopUpgrade_CoinMultiplier : ShopItem
{
    public override void PurchaseEffect()
    {
        FindObjectOfType<Spawner>().incrementMaxCoins();
    }
}
