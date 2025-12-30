using System;
using System.Collections;
using audio;
using data;
using entity;
using UnityEngine;

public enum AnimatorParameters
{
    Attack
}


namespace animation
{
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
        
        private UtilityData currentUtility; 
        private Entity utilityCaster;

        private void Awake()
        {
            Debug.Log($"[EntityAnimator] Awake on {gameObject.name}");
            
            entity = GetComponentInParent<Entity>();
            
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
                return; 
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
                    OnAttackAnimationComplete(); 
            }
            else
            {

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

        private IEnumerator PlayUtilityLungeSequence()
        {
            if (currentTarget == null)
            {
                Debug.LogWarning($"{entity.entityName} has no target for utility lunge");
                OnUtilityAnimationComplete();
                yield break;
            }
     
            originalPosition = transform.position;
            Vector3 targetPosition = currentTarget.transform.position;
            float lungeDistance = currentUtility.lungeDistance;
            Vector3 lungePosition = Vector3.MoveTowards(originalPosition, targetPosition, lungeDistance);
 
            if (animator != null)
                animator.SetTrigger(UtilityTrigger);

            isLunging = true;
            float elapsed = 0f;
            while (elapsed < lungeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / lungeDuration;
                transform.position = Vector3.Lerp(originalPosition, lungePosition, t);
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
   
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
            originalPosition = transform.position;
            Vector3 targetPosition = currentTarget.transform.position;
            Vector3 lungePosition = Vector3.MoveTowards(originalPosition, targetPosition, lungeDistance);
            if (animator)
                animator.SetTrigger(AttackTrigger);

            isLunging = true;
            float elapsed = 0f;
            while (elapsed < lungeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / lungeDuration;
                transform.position = Vector3.Lerp(originalPosition, lungePosition, t);
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
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
            
            if (!animator)
                OnAttackAnimationComplete();
        }
        
        #endregion
        
        #region Animation Events (called from Animation clips)

        public void OnAttackDamageFrame()
        {
            if (currentTarget != null && currentTarget.isAlive)
            {
                entity.Attack(currentTarget);
                
                if (hitVFXPrefab != null)
                    VFXSpawner.Instance?.SpawnVFX(hitVFXPrefab, currentTarget.transform.position);

                EntityAnimator targetAnimator = currentTarget.GetAnimator();
                targetAnimator?.PlayHitAnimation();
                
                AudioManager.PlaySound(SoundType.SWORD);
                AudioManager.PlaySound(SoundType.HURT, 0.5f);
            }

            if (attackVFXPrefab != null)
                VFXSpawner.Instance?.SpawnVFX(attackVFXPrefab, transform.position);
        }
        
        public void OnAttackAnimationComplete()
        {
            PlayIdleAnimation(); 

            if (!isRangedCharacter)
            {
                AnimationController.Instance?.SignalAnimationComplete();
                currentAnimationCallback?.Invoke();
                currentAnimationCallback = null;
                currentTarget = null;
            }
        }

        private void OnProjectileHit()
        {
            AnimationController.Instance?.SignalAnimationComplete();
            currentAnimationCallback?.Invoke();
            currentAnimationCallback = null;
            currentTarget = null;
            
            AudioManager.PlaySound(SoundType.BOW_HIT);
            AudioManager.PlaySound(SoundType.HURT, 0.5f);
        }
        
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
                    AudioManager.PlaySound(SoundType.BOW);
                    controller.Initialize(
                        entity, 
                        currentTarget, 
                        hitVFXPrefab,
                        OnProjectileHit 
                    );
                }
            }
        }
        
        public void OnUtilityEffectFrame()
        {
            if (currentUtility != null && utilityCaster != null)
            {
                currentUtility.Execute(utilityCaster, currentTarget);
                
                switch (currentUtility.utilityType)
                {
                    case UtilityType.Damage:
                        AudioManager.PlaySound(SoundType.HURT);
                        break;
                    case UtilityType.Heal:
                         AudioManager.PlaySound(SoundType.HEAL);
                         break;
                     case UtilityType.Buff:
                         AudioManager.PlaySound(SoundType.BUFF);
                         break;
                }
            }

            if (utilityVFXPrefab != null)
            {
                if (currentTarget != null)
                    VFXSpawner.Instance?.SpawnVFX(utilityVFXPrefab, currentTarget.transform.position);
                else
                    VFXSpawner.Instance?.SpawnVFX(utilityVFXPrefab, transform.position);
            }
        }
        
        public void OnUtilityAnimationComplete()
        {
            PlayIdleAnimation();
            AnimationController.Instance?.SignalAnimationComplete();
            currentAnimationCallback?.Invoke();
            currentAnimationCallback = null;
            currentTarget = null;
            currentUtility = null;
            utilityCaster = null; 
        }
  
        public void OnHitAnimationComplete()
        {
            PlayIdleAnimation();
        }
        

        public void OnDeathAnimationComplete()
        {
            AudioManager.PlaySound(SoundType.DEATH);
        }
        
        #endregion
    }
}