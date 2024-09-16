using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShopItem : MonoBehaviour
{
    [SerializeField]
    protected int _cost;
    public int CostProp => _cost;

    [SerializeField]
    protected Sprite _itemImage;
    public Sprite ItemImageProp => _itemImage;

    [SerializeField]
    protected string _itemName;
    public string ItemNameProp => _itemName;

    [SerializeField]
    protected string _itemDescription;
    public string ItemDescriptionProp => _itemDescription;

    [SerializeField]
    protected bool _isCompoundUpgrade = false;
    public bool IsCompoundUpgradeProp => _isCompoundUpgrade;

    [SerializeField]
    protected int _initialCost = 100;
    public int InitialCostProp => _initialCost;

    public abstract void PurchaseEffect(); //could turn this into a list of listeners or something, like how on click works

    public void ResetCost() 
    {
        _cost = _initialCost;
    }
}
