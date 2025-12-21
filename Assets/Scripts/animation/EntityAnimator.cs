using System;
using System.Collections;
using combat;
using data;
using entity;
using UnityEngine;

public enum AnimatorParameters
{
    Attack
}


namespace animation
{
    /// <summary>
    /// Handles animations for individual entities
    /// </summary>
    public class EntityAnimator : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Animator animator;
        private Entity entity;
        
        [Header("Animation States")]
        private static readonly int IdleState = Animator.StringToHash("Idle");
        private static readonly int AttackReadyState = Animator.StringToHash("AttackReady");
        private static readonly int AttackTrigger = Animator.StringToHash("Attack");
        private static readonly int UtilityTrigger = Animator.StringToHash("Utility");
        private static readonly int HitTrigger = Animator.StringToHash("Hit");
        private static readonly int DeathTrigger = Animator.StringToHash("Death");
        
        [Header("Melee Attack Settings")]
        [SerializeField] private bool isMeleeCharacter = false;
        [SerializeField] private float lungeDistance = 2f;
        [SerializeField] private float lungeDuration = 0.3f;
        
        [Header("Ranged Attack Settings")]
        [SerializeField] private bool isRangedCharacter = false;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform projectileSpawnPoint;
        
        [Header("VFX Settings")]
        [SerializeField] private GameObject attackVFXPrefab;
        [SerializeField] private GameObject utilityVFXPrefab;
        [SerializeField] private GameObject hitVFXPrefab;
        
        private Action currentAnimationCallback;
        private Entity currentTarget;
        private Vector3 originalPosition;
        private bool isLunging = false;
        
        private UtilityData currentUtility; // NEW
        private Entity utilityCaster; // NEW

        private void Awake()
        {
            Debug.Log($"[EntityAnimator] Awake on {gameObject.name}");
    
            // First try parent
            entity = GetComponentInParent<Entity>();
    
            // If not found in parent, try siblings through parent
            if (entity == null && transform.parent != null)
            {
                entity = transform.parent.GetComponentInChildren<Entity>();
            }
    
            if (entity != null)
            {
                Debug.Log($"[EntityAnimator] Found entity: {entity.entityName} on GameObject: {entity.gameObject.name}");
                Debug.Log($"[EntityAnimator] Entity has data: {entity.entityData != null}");
                if (entity.entityData != null)
                    Debug.Log($"[EntityAnimator] BaseDamage: {entity.BaseDamage}");
            }
            else
            {
                Debug.LogError($"[EntityAnimator] Entity is NULL!");
            }
    
            originalPosition = transform.position;
    
            if (animator == null)
                animator = GetComponentInChildren<Animator>();
        }
        
        private void Update()
        {
            if (isLunging)
                return; // Position handled by lunge coroutine
        }

        public Animator getAnimator()
        {
            return animator;
        }
        
        #region Public Animation Methods
        
        public void PlayIdleAnimation()
        {
            if (animator != null)
                animator.Play(IdleState);
        }
        
        public void PlayAttackReadyAnimation()
        {
            if (animator != null)
                animator.Play(AttackReadyState);
        }
        
        public void PlayAttackAnimation(Entity target, Action onComplete)
        {
            currentTarget = target;
            currentAnimationCallback = onComplete;
            
            Debug.Log("Playing attack animation" + isMeleeCharacter + isRangedCharacter + animator);
            if (isMeleeCharacter)
            {
                StartCoroutine(PlayMeleeAttackSequence());
            }
            else if (isRangedCharacter)
            {
                if (animator != null)
                    animator.SetTrigger(AttackTrigger);
                else
                    OnAttackAnimationComplete(); // Fallback if no animator
            }
            else
            {
                // Generic attack
                if (animator != null)
                    animator.SetTrigger(AttackTrigger);
                else
                    OnAttackAnimationComplete();
            }
        }
        
        public void PlayUtilityAnimation(Entity target, Action onComplete, UtilityData utility = null, Entity caster = null)
        {
            currentTarget = target;
            currentAnimationCallback = onComplete;
            currentUtility = utility;
            utilityCaster = caster;
            
            // Check if this utility should lunge
            if (utility != null && utility.shouldLunge && currentTarget != null)
            {
                StartCoroutine(PlayUtilityLungeSequence());
            }
            else
            {
                if (animator != null)
                    animator.SetTrigger(UtilityTrigger);
                else
                    OnUtilityAnimationComplete();
            }
        }

        // NEW - Utility version of lunge
        private IEnumerator PlayUtilityLungeSequence()
        {
            if (currentTarget == null)
            {
                Debug.LogWarning($"{entity.entityName} has no target for utility lunge");
                OnUtilityAnimationComplete();
                yield break;
            }
            
            // Store original position
            originalPosition = transform.position;
            Vector3 targetPosition = currentTarget.transform.position;
            float lungeDistance = currentUtility.lungeDistance;
            Vector3 lungePosition = Vector3.MoveTowards(originalPosition, targetPosition, lungeDistance);
            
            // Start utility animation
            if (animator != null)
                animator.SetTrigger(UtilityTrigger);
            
            // Lunge forward
            isLunging = true;
            float elapsed = 0f;
            while (elapsed < lungeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / lungeDuration;
                transform.position = Vector3.Lerp(originalPosition, lungePosition, t);
                yield return null;
            }
            
            // Wait a bit at extended position
            yield return new WaitForSeconds(0.1f);
            
            // Return to original position
            elapsed = 0f;
            while (elapsed < lungeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / lungeDuration;
                transform.position = Vector3.Lerp(lungePosition, originalPosition, t);
                yield return null;
            }
            
            transform.position = originalPosition;
            isLunging = false;
            
            // Animation completion handled by animation event
            if (animator == null)
                OnUtilityAnimationComplete();
        }
        
        public void PlayHitAnimation()
        {
            if (animator != null)
                animator.SetTrigger(HitTrigger);
        }
        
        public void PlayDeathAnimation()
        {
            if (animator != null)
                animator.SetTrigger(DeathTrigger);
        }
        
        #endregion
        
        #region Melee Attack Sequence
        
        private IEnumerator PlayMeleeAttackSequence()
        {
            if (currentTarget == null)
            {
                Debug.LogWarning($"{entity.entityName} has no target for melee attack");
                OnAttackAnimationComplete();
                yield break;
            }
            
            // Store original position
            originalPosition = transform.position;
            Vector3 targetPosition = currentTarget.transform.position;
            Vector3 lungePosition = Vector3.MoveTowards(originalPosition, targetPosition, lungeDistance);
            
            // Start attack animation
            if (animator != null)
                animator.SetTrigger(AttackTrigger);
            
            // Lunge forward
            isLunging = true;
            float elapsed = 0f;
            while (elapsed < lungeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / lungeDuration;
                transform.position = Vector3.Lerp(originalPosition, lungePosition, t);
                yield return null;
            }
            
            // Wait a bit at extended position
            yield return new WaitForSeconds(0.1f);
            
            // Return to original position
            elapsed = 0f;
            while (elapsed < lungeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / lungeDuration;
                transform.position = Vector3.Lerp(lungePosition, originalPosition, t);
                yield return null;
            }
            
            transform.position = originalPosition;
            isLunging = false;
            
            // Animation completion handled by animation event or fallback timer
            if (animator == null)
                OnAttackAnimationComplete();
        }
        
        #endregion
        
        #region Animation Events (called from Animation clips)
        
        /// <summary>
        /// Called at the damage frame of attack animation
        /// </summary>
        public void OnAttackDamageFrame()
        {
            if (currentTarget != null && currentTarget.isAlive)
            {
                entity.Attack(currentTarget);
        
                // Spawn hit VFX on target
                if (hitVFXPrefab != null)
                    VFXSpawner.Instance?.SpawnVFX(hitVFXPrefab, currentTarget.transform.position);
        
                // Play hit animation on target - search in children!
                EntityAnimator targetAnimator = currentTarget.GetAnimator();
                targetAnimator?.PlayHitAnimation();
            }
    
            // Spawn attack VFX at attacker
            if (attackVFXPrefab != null)
                VFXSpawner.Instance?.SpawnVFX(attackVFXPrefab, transform.position);
        }
        
        /// <summary>
        /// Called when attack animation completes
        /// </summary>
        public void OnAttackAnimationComplete()
        {
            PlayIdleAnimation(); // Transition back to idle
    
            // DON'T signal animation controller here for ranged attacks
            // The projectile will do it when it hits
            if (!isRangedCharacter)
            {
                // For melee, complete immediately
                AnimationController.Instance?.SignalAnimationComplete();
                currentAnimationCallback?.Invoke();
                currentAnimationCallback = null;
                currentTarget = null;
            }
            // For ranged, projectile callback will handle completion
        }

        // Called when projectile hits target
        private void OnProjectileHit()
        {
            // NOW signal animation complete
            AnimationController.Instance?.SignalAnimationComplete();
            currentAnimationCallback?.Invoke();
            currentAnimationCallback = null;
            currentTarget = null;
        }
        
        /// <summary>
        /// Called at projectile spawn frame
        /// </summary>
        public void OnProjectileSpawnFrame()
        {
            if (currentTarget != null && projectilePrefab != null)
            {
                Vector3 spawnPos = projectileSpawnPoint != null 
                    ? projectileSpawnPoint.position 
                    : transform.position;
        
                GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
                ProjectileController controller = projectile.GetComponent<ProjectileController>();
        
                if (controller != null)
                {
                    // Pass a callback to complete animation when projectile hits
                    controller.Initialize(
                        entity, 
                        currentTarget, 
                        hitVFXPrefab,
                        OnProjectileHit // NEW CALLBACK
                    );
                }
            }
        }
        
        /// <summary>
        /// Called at the effect frame of utility animation
        /// </summary>
        public void OnUtilityEffectFrame()
        {
            // Execute the actual utility effect at this frame
            if (currentUtility != null && utilityCaster != null)
            {
                currentUtility.Execute(utilityCaster, currentTarget);
            }
    
            // Spawn VFX
            if (utilityVFXPrefab != null)
            {
                if (currentTarget != null)
                    VFXSpawner.Instance?.SpawnVFX(utilityVFXPrefab, currentTarget.transform.position);
                else
                    VFXSpawner.Instance?.SpawnVFX(utilityVFXPrefab, transform.position);
            }
        }
        
        /// <summary>
        /// Called when utility animation completes
        /// </summary>
        public void OnUtilityAnimationComplete()
        {
            PlayIdleAnimation();
            AnimationController.Instance?.SignalAnimationComplete();
            currentAnimationCallback?.Invoke();
            currentAnimationCallback = null;
            currentTarget = null;
            currentUtility = null; // Clear
            utilityCaster = null; // Clear
        }
        
        /// <summary>
        /// Called when hit animation completes
        /// </summary>
        public void OnHitAnimationComplete()
        {
            PlayIdleAnimation();
        }
        
        /// <summary>
        /// Called when death animation completes
        /// </summary>
        public void OnDeathAnimationComplete()
        {
            // Handled by EntityAnimationManager fade
        }
        
        #endregion
    }
}