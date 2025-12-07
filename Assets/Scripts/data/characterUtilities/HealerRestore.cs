using System;
using combat;
using entity;
using UnityEngine;

namespace data
{
    [CreateAssetMenu(fileName = "Healer Restore", menuName = "Utility/Healer Restore")]
    public class HealerRestoreUtility : UtilityData
    {
        [Header("Healing Settings")]
        public int healAmount = 30;
        public bool healSelf = true;
        
        public override void Execute(Entity caster, Entity target = null)
        {
            if (targetType == TargetType.AllAllies)
            {
                if (target != null && target.isAlive)
                {
                    Heal(target, caster.entityName);
                }
                else
                {
                    Debug.Log("No target found to heal");
                }
            }
        }

        private void Heal(Entity target, string casterName)
        {
            int oldHealth = target.currentHealth;
            target.currentHealth = Mathf.Min(target.currentHealth + healAmount, target.MaxHealth);
            int actualHeal = target.currentHealth - oldHealth;
            Debug.Log($"{casterName} healed {target.entityName} for {actualHeal} HP!");
        }
    }
}