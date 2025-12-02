using entity;
using UnityEngine;

namespace data
{
    [CreateAssetMenu(fileName = "Cavalry Charge", menuName = "Utility/Cavalry Charge")]
    public class CavalryChargeUtility : UtilityData
    {
        [Header("Charge Settings")]
        public float damageMultiplier = 1.5f;
        public int stunDuration = 1;
        
        public override void Execute(Entity caster, Entity target = null)
        {
            if (target != null && target.isAlive)
            {
                int damage = Mathf.RoundToInt(caster.BaseDamage * damageMultiplier);
                target.TakeDamage(damage);
                
                // If target is an enemy, apply stun (you'll need to add stun logic to Entity)
                if (target is Enemy enemy)
                {
                    enemy.ApplyStun(stunDuration);
                }
                
                Debug.Log($"{caster.entityName} charged {target.entityName} for {damage} damage and stun!");
            }
        }
    }
}