using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUpgrade1 : ShopItem
{
    public TestUpgrade1(int cost, string itemName, string itemDescription)
    {
        
    }
    public override void PurchaseEffect()
    {
        Debug.Log(ItemNameProp + " was bought!");
    }
}
