using UnityEngine;

namespace data
{
    [CreateAssetMenu(fileName = "New Entity Data", menuName = "Entity/Entity Data")]
    public class EntityData : ScriptableObject
    {
        [Header("Identity")]
        public string entityName;
        public Sprite sprite;
    
        [Header("Base Stats")]
        public int maxHealth;
        public int actionSpeed;
        public int baseDamage;
        [Header("Shield Stats")]
        [Range(0f, 1f)]
        public float shieldDamageReduction = 0.5f; 
        public int shieldDuration = 1;
    }
}