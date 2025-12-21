using entity;
using UnityEditor.Playables;
using UnityEngine;

namespace data.utilityTypes
{
    [CreateAssetMenu(fileName = "New Buff Data", menuName = "Utility/Buff Data")]
    public class BuffUtility : UtilityData
    {
        [Header("Buff Stats")]
        public float damageBoost;
        public int defenseBoost;
        public int duration;
        
        public override void Execute(Entity caster, Entity target = null)
        {
            Debug.Log($"[BuffUtility] Execute called - Caster: {caster?.entityName}, Target: {target?.entityName}");
    
            if (target is PlayableCharacter playable)
            {
                Debug.Log($"[BuffUtility] Applying buff to {playable.entityName}");
                playable.ApplyBuff(damageBoost, defenseBoost, duration);
            }
            else
            {
                Debug.Log($"[BuffUtility] Target is not PlayableCharacter! Type: {target?.GetType().Name}");
            }
        }
    }
}