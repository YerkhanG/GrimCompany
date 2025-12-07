using entity;
using UnityEngine;

namespace data
{
    [CreateAssetMenu(fileName = "Archer Volley", menuName = "Utility/Archer Volley")]
    public class ArcherVolleyUtility : UtilityData
    {
        [Header("Volley Settings")]
        [Range(1.5f, 3f)]
        public float damageMultiplier = 2f;
        
        public override void Execute(Entity caster, Entity target = null)
        {
            if (target != null && target.isAlive)
            {
                int damage = Mathf.RoundToInt(caster.BaseDamage * damageMultiplier);
                target.TakeDamage(damage);
                Debug.Log($"{caster.entityName} unleashed volley on {target.entityName} for {damage} damage!");
            }
        }
    }
}