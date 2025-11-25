using UnityEngine;

public class LobbyUIManager : MonoBehaviour
{
    [Header("Panels")] public GameObject tavernPanel;
    public GameObject recruitPanel;
    public GameObject embarkPanel;
    public GameObject trainingPanel;

    void Awake()
    {
        CloseAll();
    }

    public void ShowTavern() => ShowPanel(tavernPanel);
    public void ShowRecruit() => ShowPanel(recruitPanel);
    public void ShowEmbark() => ShowPanel(embarkPanel);
    public void ShowTraining() => ShowPanel(trainingPanel);
    public void CloseAll() => ShowPanel(null);

    void ShowPanel(GameObject target)
    {
        tavernPanel.SetActive(false);
        recruitPanel.SetActive(false);
        embarkPanel.SetActive(false);
        trainingPanel.SetActive(false);

        if (target != null)
            target.SetActive(true);
    }
}