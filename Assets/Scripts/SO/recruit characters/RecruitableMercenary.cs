using UnityEngine;

[CreateAssetMenu(
    menuName = "Recruit/Mercenary",
    fileName = "Mercenary_"
)]
public class RecruitableMercenary : ScriptableObject
{
    [Header("Display")] public string displayName;
    public Sprite portrait;

    [Header("Stats")] public int maxHealth;
    public int attackDamage;
    public int speed; // or haste

    [Header("Recruiting")] public float cost;
}