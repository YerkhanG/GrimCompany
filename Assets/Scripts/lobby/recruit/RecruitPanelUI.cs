using System.Collections.Generic;
using persistence;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecruitPanelUI : MonoBehaviour
{
    [Header("UI")] [SerializeField] private TextMeshProUGUI fundsLabel;
    [SerializeField] private Transform listRoot; // Content transform
    [SerializeField] private Button recruitButton;

    [Header("Prefabs")] [SerializeField] private GameObject recruitRowPrefab;

    [Header("Data")] [SerializeField] private List<RecruitableMercenary> availableMercenaries;

    private IPurchaser _purchaser;
    private RecruitableMercenary _selected;

    private readonly Dictionary<RecruitableMercenary, RecruitRowUI> _rows =
        new Dictionary<RecruitableMercenary, RecruitRowUI>();

    private bool _initialized;

    private void OnEnable()
    {
        if (!_initialized)
        {
            Initialize();
            _initialized = true;
        }

        RefreshFundsAndAffordability();
    }

    private void Initialize()
    {
        _purchaser = FindFirstObjectByType<Purchaser>();
        BuildList();
    }

    private void BuildList()
    {
        // Clear old UI
        for (var i = listRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(listRoot.GetChild(i).gameObject);
        }

        _rows.Clear();
        _selected = null;

        foreach (var merc in availableMercenaries)
        {
            if (merc == null) continue;

            var go = Instantiate(recruitRowPrefab, listRoot);
            var row = go.GetComponent<RecruitRowUI>();

            row.Bind(merc, OnMercenarySelected);
            _rows[merc] = row;
        }
    }

    private void OnMercenarySelected(RecruitableMercenary merc)
    {
        _selected = merc;

        foreach (var pair in _rows)
        {
            pair.Value.SetSelected(pair.Key == merc);
        }

        RefreshFundsAndAffordability();
    }

    private void RefreshFundsAndAffordability()
    {
        if (_purchaser != null)
            fundsLabel.text = $"{_purchaser.GetCurrentFunds():0.00}";
        else
            fundsLabel.text = string.Empty;

        var canRecruit =
            _purchaser != null &&
            _selected != null &&
            _purchaser.GetCurrentFunds() >= _selected.cost;

        if (recruitButton != null)
            recruitButton.interactable = canRecruit;

        foreach (var pair in _rows)
        {
            var merc = pair.Key;
            var row = pair.Value;

            if (_purchaser == null)
                row.SetCanAfford(false);
            else
                row.SetCanAfford(
                    merc.cost <= _purchaser.GetCurrentFunds()
                );
        }
    }

    public void OnClickRecruit()
    {
        if (_purchaser == null || _selected == null)
            return;

        if (_purchaser.SpendFunds(_selected.cost))
        {
            // Add the recruited hero to the run manager's party
            var heroData = new HeroData(_selected.prefabName);
            RunManager.Instance.party.Add(heroData);
        
            Debug.Log($"Recruited {_selected.displayName} - Added to party");

            RefreshFundsAndAffordability();
        }
    }

    public void OnClickExit()
    {
        gameObject.SetActive(false);
    }
}