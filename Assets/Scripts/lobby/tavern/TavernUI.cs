using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TavernUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI availableMoney;
    [SerializeField] private Transform categoryUIRoot;
    [SerializeField] private Transform itemUIRoot;

    [SerializeField] private Button purchaseButton;

    [SerializeField] private GameObject categoryUIPrefab;
    [SerializeField] private GameObject itemUIPrefab;

    [SerializeField] private List<ShopItem> availableItems;

    private IPurchaser currentPurchaser;

    private ShopItemCategory selectedCategory;
    private ShopItem selectedItem;
    private List<ShopItemCategory> shopCategories;
    private Dictionary<ShopItemCategory, ShopUI_Category> shopCategoryToUIMap;
    private Dictionary<ShopItem, ShopUI_Item> shopItemToUIMap;

    private bool _initialized;

    private void OnEnable()
    {
        if (_initialized) return;
        _initialized = true;

        currentPurchaser = FindFirstObjectByType<Purchaser>();

        RefreshShopUI_Common();
        RefreshShopUI_Categories();
    }

    private void RefreshShopUI_Common()
    {
        if (currentPurchaser != null)
        {
            availableMoney.text = $"{currentPurchaser.GetCurrentFunds():0.00}";
        }
        else
        {
            availableMoney.text = string.Empty;
        }

        if (currentPurchaser != null && selectedItem != null &&
            currentPurchaser.GetCurrentFunds() >= selectedItem.price)
        {
            purchaseButton.interactable = true;
        }
        else
        {
            purchaseButton.interactable = false;
        }

        if (shopItemToUIMap != null)
        {
            foreach (var kvp in shopItemToUIMap)
            {
                var item = kvp.Key;
                var itemUI = kvp.Value;

                if (currentPurchaser != null)
                {
                    itemUI.SetCanAfford(item.price <= currentPurchaser.GetCurrentFunds());
                }
                else
                {
                    itemUI.SetCanAfford(false);
                }
            }
        }
    }

    private void RefreshShopUI_Categories()
    {
        for (int childIndex = categoryUIRoot.childCount - 1; childIndex >= 0; childIndex--)
        {
            var childGO = categoryUIRoot.GetChild(childIndex).gameObject;
            Destroy(childGO);
        }

        shopCategories = new List<ShopItemCategory>();
        shopCategoryToUIMap = new Dictionary<ShopItemCategory, ShopUI_Category>();

        foreach (var item in availableItems)
        {
            if (!shopCategories.Contains(item.category))
            {
                shopCategories.Add(item.category);
            }
        }

        shopCategories.Sort((lhs, rhs) => String.Compare(lhs.categoryName, rhs.categoryName, StringComparison.Ordinal));

        foreach (var category in shopCategories)
        {
            var newCategoryUI = Instantiate(categoryUIPrefab, categoryUIRoot);
            var newShopUI = newCategoryUI.GetComponent<ShopUI_Category>();

            newShopUI.Bind(category, OnCategorySelected);
            shopCategoryToUIMap[category] = newShopUI;
        }

        if (!shopCategories.Contains(selectedCategory))
        {
            selectedCategory = null;
        }

        OnCategorySelected(selectedCategory);
    }

    private void OnCategorySelected(ShopItemCategory newlySelectedCategory)
    {

        if (selectedCategory != null && newlySelectedCategory != null && selectedCategory != newlySelectedCategory)
        {
            selectedItem = null;
        }

        selectedCategory = newlySelectedCategory;

        foreach (var category in shopCategories)
        {
            shopCategoryToUIMap[category].SetIsCategorySelected(category == newlySelectedCategory);
        }

        RefreshShopUI_Items();
    }

    private void RefreshShopUI_Items()
    {
        for (int childIndex = itemUIRoot.childCount - 1; childIndex >= 0; childIndex--)
        {
            var childGO = itemUIRoot.GetChild(childIndex).gameObject;
            Destroy(childGO);
        }

        shopItemToUIMap = new Dictionary<ShopItem, ShopUI_Item>();

        foreach (var item in availableItems)
        {
            if (item.category != selectedCategory)
            {
                continue;
            }

            var newItemUI = Instantiate(itemUIPrefab, itemUIRoot);
            var newShopUI = newItemUI.GetComponent<ShopUI_Item>();

            newShopUI.Bind(item, OnItemSelected);
            shopItemToUIMap[item] = newShopUI;
        }

        RefreshShopUI_Common();
    }

    private void OnItemSelected(ShopItem newlySelectedItem)
    {
        selectedItem = newlySelectedItem;

        foreach (var kvp in shopItemToUIMap)
        {
            var item = kvp.Key;
            var itemUI = kvp.Value;

            itemUI.SetIsItemSelected(item == newlySelectedItem);
        }

        RefreshShopUI_Common();
    }

    public void OnClickPurchase()
    {
        currentPurchaser.SpendFunds(selectedItem.price);
        RefreshShopUI_Common();
    }

    public void OnClickExit()
    {
        gameObject.SetActive(false);
    }
}