using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Product
{
    public string productName;
    [Space]
    [Header("Stats")]
    public ShelveTypes storageType;
    public int itemBuyPrice;
    public int itemSellPrice()
    {
        return itemBuyPrice * 3;
    }

    [Space]
    [Header("Visuals")]
    public GameObject itemModel;
    public Sprite itemIcon;
    public Sprite sortIcon;
}

[CreateAssetMenu(fileName = "Product Library", menuName = "ScriptableObjects/ProductLibrary", order = 1)]
public class ProductLibrary : ScriptableObject
{
    public Product[] products;
}
