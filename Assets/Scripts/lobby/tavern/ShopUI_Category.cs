using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopUI_Category : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI categoryName;
    [SerializeField] private Image backgroundPanel;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color unselectedColor;

    ShopItemCategory category;
    UnityAction<ShopItemCategory> onCategorySelectedFn;

    public void Bind(ShopItemCategory category, UnityAction<ShopItemCategory> onCategorySelectedFn)
    {
        this.category = category;
        categoryName.text = category.categoryName;
        this.onCategorySelectedFn = onCategorySelectedFn;

        SetIsCategorySelected(false);
    }

    public void SetIsCategorySelected(bool isSelected)
    {
        backgroundPanel.color = isSelected ? selectedColor : unselectedColor;
    }

    public void OnClickedCategory()
    {
        if (onCategorySelectedFn == null)
        {
            Debug.LogWarning(
                $"{name}: Category clicked but no onCategorySelectedFn was bound.",
                this
            );
            return;
        }

        onCategorySelectedFn.Invoke(category);
    }
}