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
    
        [Header("Position Rules")]
        public int defaultPosition; 
        public int minRange = 0;    
        public int maxRange = 1;
    }
}