using entity;
using UnityEngine;
using System;

namespace animation
{
    public class ProjectileController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float speed = 10f;
        [SerializeField] private float lifetime = 3f;
        
        private Entity caster;
        private Entity target;
        private GameObject hitVFXPrefab;
        private bool hasHit = false;
        private Action onHitCallback;
        
        public void Initialize(Entity caster, Entity target, GameObject hitVFX, Action onHit = null)
        {
            this.caster = caster;
            this.target = target;
            this.hitVFXPrefab = hitVFX;
            this.onHitCallback = onHit; 
            
            Destroy(gameObject, lifetime);
        }
        
        private void Update()
        {
            if (hasHit || target == null || !target.isAlive)
            {
                Destroy(gameObject);
                return;
            }
            
            Vector3 direction = (target.transform.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            
            if (Vector3.Distance(transform.position, target.transform.position) < 0.3f)
            {
                HitTarget();
            }
        }
        
        private void HitTarget()
        {
            if (hasHit) return;
            hasHit = true;
            
            if (target != null && target.isAlive && caster != null)
            {
                caster.Attack(target);
                
                Debug.Log("Playing hit animation " + caster + " and " + target);
                // Play hit animation - search in children!
                EntityAnimator targetAnimator = target.GetAnimator();
                
                Debug.Log("Target animator: " + targetAnimator);
                targetAnimator?.PlayHitAnimation();
            }
    
            // Spawn hit VFX
            if (hitVFXPrefab != null)
                VFXSpawner.Instance?.SpawnVFX(hitVFXPrefab, transform.position);
    
            // Call the callback to signal animation complete
            onHitCallback?.Invoke();
    
            Destroy(gameObject);
        }
    }
}