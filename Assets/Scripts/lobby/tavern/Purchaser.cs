using UnityEngine;

public interface IPurchaser
{
    float GetCurrentFunds();
    bool SpendFunds(float amount);
    void AddFunds(float amount);
}

public class Purchaser : MonoBehaviour, IPurchaser
{
    public static Purchaser Instance { get; private set; }

    [SerializeField] private float currentFunds;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public float GetCurrentFunds()
    {
        return currentFunds;
    }

    public bool SpendFunds(float amount)
    {
        if (currentFunds >= amount)
        {
            currentFunds -= amount;
            return true;
        }

        return false;
    }

    public void AddFunds(float amount)
    {
        if (amount < 0f)
        {
            Debug.LogWarning("Use SpendFunds to remove funds, not AddFunds.");
            return;
        }

        currentFunds += amount;
    }
}