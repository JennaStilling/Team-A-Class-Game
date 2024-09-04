using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShopItem : MonoBehaviour
{
    [SerializeField]
    protected int _cost;
    public int CostProp => _cost;

    [SerializeField]
    protected Sprite _itemImage;//probably some sort of ui element, png or something
    public Sprite ItemImageProp => _itemImage;//probably some sort of ui element, png or something

    [SerializeField]
    protected string _itemName;
    public string ItemNameProp => _itemName;

    [SerializeField]
    protected string _itemDescription;
    public string ItemDescriptionProp => _itemDescription;

    public abstract void PurchaseEffect();
}
