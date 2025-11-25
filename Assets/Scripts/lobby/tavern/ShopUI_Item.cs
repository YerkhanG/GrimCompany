using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopUI_Item : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] TextMeshProUGUI price;

    [SerializeField] private Image backgroundPanel;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color unselectedColor;

    UnityAction<ShopItem> onSelectedItemFn;

    private ShopItem item;

    public void Bind(ShopItem item, UnityAction<ShopItem> onSelectedItemFn)
    {
        this.item = item;
        this.onSelectedItemFn = onSelectedItemFn;

        itemName.text = item.itemName;
        description.text = item.description;
        price.text = $"{item.price:0.00}";

        SetIsItemSelected(false);
    }

    public void SetIsItemSelected(bool isSelected)
    {
        backgroundPanel.color = isSelected ? selectedColor : unselectedColor;
    }

    public void OnClickedItem()
    {
        onSelectedItemFn.Invoke(item);
    }

    public void SetCanAfford(bool canAfford)
    {
        price.fontStyle = canAfford ? FontStyles.Normal : FontStyles.Strikethrough;
        price.color = canAfford ? Color.white : Color.red;
    }
}