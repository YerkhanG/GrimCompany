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
            if (target is PlayableCharacter playable)
            {
                playable.ApplyBuff(damageBoost, defenseBoost, duration);
            }
        }
    }
}