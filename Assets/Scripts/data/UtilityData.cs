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
        public abstract void Execute(Entity caster, Entity target = null);
    }
    public enum TargetType
    {
        AllEnemies,     // Targets all enemies
        AllAllies       // Targets all allies
    }
}