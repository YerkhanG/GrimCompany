using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapNodeButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Button button;

    private int _nodeIndex;
    private MapNode _data;

    public void Initialize(int nodeIndex, MapNode data)
    {
        _nodeIndex = nodeIndex;
        _data = data;

        if (label != null)
            label.text = data.id;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClicked);

        UpdateInteractable();
    }

    public void UpdateInteractable()
    {
        var run = RunManager.Instance;
        if (run == null)
        {
            button.interactable = true;
            return;
        }

        // Only nodes that are direct next nodes are clickable
        bool canTravel = run.CanTravelTo(_nodeIndex);
        button.interactable = canTravel;
    }

    private void OnClicked()
    {
        RunManager.Instance.TravelToNode(_nodeIndex);
    }
}