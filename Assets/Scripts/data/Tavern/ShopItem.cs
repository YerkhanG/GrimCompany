using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Item", fileName = "ShopItem_")]
public class ShopItem : ScriptableObject
{
    public ShopItemCategory category;
    public string itemName;
    [TextArea(3, 5)] public string description;

    public float price;
}