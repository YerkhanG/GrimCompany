using UnityEngine;

namespace data
{
    [CreateAssetMenu(fileName = "New Weapon Data", menuName = "Weapon/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [Header("Combat Stats")] 
        public int damage;
        public int range;
    }
}