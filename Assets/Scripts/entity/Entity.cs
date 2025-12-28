using System.Collections;
using combat;
using data;
using events;
using UnityEngine;
using animation;

namespace entity
{
    public struct EntityRuntimeStats
    {
        public int maxHealth;
        public int actionSpeed;
        public int baseDamage;

        public float shieldDamageReduction;
        public int shieldDuration;
    }

    public class Entity : MonoBehaviour
    {
        private EntityRuntimeStats _runtime;

        public int ActionSpeed => _runtime.actionSpeed;
        public int BaseDamage => _runtime.baseDamage;
        public int MaxHealth => _runtime.maxHealth;

        [Header("Stats reference")] public string entityName;
        public EntityData entityData;

        [Header("Runtime State")] public int currentHealth;
        public int currentPosition;

        public bool isAlive = true;
        public bool isPlayable;
        private GameObject entityParentObject;

        [Header("Repositioning")] private Vector3 targetPosition;
        private bool isMoving = false;

        [Header("Status Effects")] private int stunTurnsRemaining = 0;
        public bool IsStunned => stunTurnsRemaining > 0;

        protected EntityAnimator entityAnimator;

        protected virtual void InitializeRuntimeStatsFromData()
        {
            _runtime = new EntityRuntimeStats
            {
                maxHealth = entityData.maxHealth,
                actionSpeed = entityData.actionSpeed,
                baseDamage = entityData.baseDamage,
                shieldDamageReduction = entityData.shieldDamageReduction,
                shieldDuration = entityData.shieldDuration,
            };
        }

        public void ApplyRuntimeStats(EntityRuntimeStats stats, bool refillHealth)
        {
            _runtime = stats;

            if (refillHealth)
                currentHealth = MaxHealth;
            else
                currentHealth = Mathf.Min(currentHealth, MaxHealth);
        }

        protected virtual void Awake()
        {
            InitializeRuntimeStatsFromData();
            InitializeStats();
            
            targetPosition = transform.position;
            entityParentObject = transform.parent != null ? transform.parent.gameObject : gameObject;

            // First try children
            entityAnimator = GetComponentInChildren<EntityAnimator>();

            // If not found, try siblings (through parent)
            if (entityAnimator == null && transform.parent != null)
            {
                entityAnimator = transform.parent.GetComponentInChildren<EntityAnimator>();
            }

            if (entityAnimator != null)
                Debug.Log($"{entityName}: Found EntityAnimator on {entityAnimator.gameObject.name}");
            else
                Debug.LogWarning($"{entityName}: EntityAnimator not found!");
        }

        private void Update()
        {
            if (isMoving)
            {
                entityParentObject.transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPosition,
                    entityData.movementSpeed * Time.deltaTime
                );

                if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
                {
                    transform.position = targetPosition;
                    isMoving = false;
                }
            }
        }

        public EntityAnimator GetAnimator()
        {
            return entityAnimator;
        }

        private void InitializeStats()
        {
            currentHealth = MaxHealth;
        }

        public virtual void TakeDamage(int damage)
        {
            if (!isAlive) return;

            currentHealth -= damage;
            CombatEvents.RaiseDamageTaken(this, damage);

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public virtual void StartTurn()
        {
            if (CombatManager.Instance.getCurrentActor() != this)
                return;

            if (stunTurnsRemaining > 0)
            {
                Debug.Log($"{entityName} is stunned and cannot act!");
                stunTurnsRemaining--;
                CombatManager.Instance.EndCurrentTurn();
                return;
            }

            Debug.Log($"StartTurn by {entityName}");
        }

        public virtual void Attack(Entity target)
        {
            Debug.Log("Attacking the target " + target);
            target.TakeDamage(BaseDamage);
        }

        public virtual void EndTurn()
        {
            Debug.Log($"EndTurn by {entityName}");
        }

        public virtual void Die()
        {
            if (!isAlive) return;

            isAlive = false;
            CombatEvents.RaiseEntityDied(this);

            if (entityAnimator != null)
            {
                entityAnimator.PlayDeathAnimation();
            }

            CombatEvents.RaiseEntityDeathAnimation(entityParentObject);
        }

        public void SetTargetPosition(Vector3 newPosition)
        {
            if (!isAlive || !gameObject.activeInHierarchy) return;

            Debug.Log($"Setting target position to {newPosition}");
            targetPosition = newPosition;
            isMoving = true;
        }

        public void ApplyStun(int duration)
        {
            stunTurnsRemaining = Mathf.Max(stunTurnsRemaining, duration);
            Debug.Log($"{entityName} stunned for {duration} turns!");
        }
    }
}