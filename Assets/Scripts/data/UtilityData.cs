using System.Collections.Generic;
using entity;
using UnityEngine;

namespace data
{
    [CreateAssetMenu(fileName = "New Utility Data", menuName = "Utility/Utility Data")]
    public abstract class UtilityData : ScriptableObject
    {
        [Header("Targeting")]
        public TargetType targetType;
        
        [Header("Animation")]
        public bool shouldLunge = false;
        [Range(1f, 3f)]
        public float lungeDistance = 1.5f; 
        
        public abstract void Execute(Entity caster, Entity target = null);
    }
    
    public enum TargetType
    {
        AllEnemies,
        AllAllies
    }
}