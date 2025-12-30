using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RecruitRowUI : MonoBehaviour
{
    [Header("UI")] [SerializeField] private Image portraitImage;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private TextMeshProUGUI healthLabel;
    [SerializeField] private TextMeshProUGUI attackLabel;
    [SerializeField] private TextMeshProUGUI speedLabel;
    [SerializeField] private TextMeshProUGUI costLabel;

    [Header("Selection")] [SerializeField] private Image background;
    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private Color unselectedColor = Color.clear;

    private RecruitableMercenary _data;
    private UnityAction<RecruitableMercenary> _onClicked;

    public void Bind(
        RecruitableMercenary data,
        UnityAction<RecruitableMercenary> onClicked
    )
    {
        _data = data;
        _onClicked = onClicked;

        if (portraitImage != null)
            portraitImage.sprite = data.portrait;

        nameLabel.text = data.displayName;
        healthLabel.text = data.maxHealth.ToString();
        attackLabel.text = data.attackDamage.ToString();
        speedLabel.text = data.speed.ToString();
        costLabel.text = $"{data.cost:0.0}";

        SetSelected(false);
    }

    public void SetSelected(bool isSelected)
    {
        if (background != null)
            background.color = isSelected ? selectedColor : unselectedColor;
    }

    public void SetCanAfford(bool canAfford)
    {
        if (costLabel == null) return;

        costLabel.color = canAfford ? Color.white : Color.red;
    }
    
    public void OnRowClicked()
    {
        _onClicked?.Invoke(_data);
    }
}