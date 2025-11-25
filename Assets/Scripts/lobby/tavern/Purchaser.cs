using UnityEngine;

public interface IPurchaser
{
    float GetCurrentFunds();
    bool SpendFunds(float amount);
}

public class Purchaser : MonoBehaviour, IPurchaser
{
    [SerializeField] private float currentFunds;


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
}