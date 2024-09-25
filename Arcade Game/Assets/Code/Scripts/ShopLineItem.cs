using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShopLineItem
{
    [SerializeField]
    private ShopItem _shopItem;
    public ShopItem ShopItemProp => _shopItem;

    [SerializeField]
    private bool _isLimited;
    public bool IsLimitedProp => _isLimited;

    [SerializeField]
    private int _amountLeft;
    public int AmountLeftProp => _amountLeft;

    public ShopLineItem(ShopItem shopItem)
    {
        this._shopItem = shopItem;
        _isLimited = false;
    }

    public ShopLineItem(ShopItem shopItem, int stock) 
    {
        this._shopItem = shopItem;
        _isLimited = true;
        this._amountLeft = stock;
    }

    public bool IsAvailable() 
        {
            return (!_isLimited || _amountLeft > 0);
        }

    public int Purchase(int tickets, int amountToPurchase = 1) 
    {
        if (tickets >= amountToPurchase * _shopItem.CostProp)
        {
            if (_isLimited)
            {
                if (_amountLeft <= 0)
                    return 0;
                else if (_amountLeft < amountToPurchase)
                    return 0;
            }
        }
        else 
        {
            return 0;
        }

        _shopItem.PurchaseEffect();
        _amountLeft--;

        if (!_isLimited) _amountLeft = 99;

        return _shopItem.CostProp;
    }
}