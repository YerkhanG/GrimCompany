using System.Collections;
using combat;
using data;
using events;
using UnityEngine;

namespace entity
{
    public class Entity : MonoBehaviour
    { 
        [Header("Stats reference")]
        public string entityName;
        public EntityData entityData;

        [Header("Runtime State")] 
        public int currentHealth;
        public int currentPosition;
        public int ActionSpeed => entityData.actionSpeed;
        public int BaseDamage => entityData.baseDamage; 
        public int MaxHealth => entityData.maxHealth;
        public bool isAlive = true;
        public bool isPlayable;
        private GameObject entityParentObject;
        [Header("Repositioning")]
        private Vector3 targetPosition;
        private bool isMoving = false;
        
        [Header("Status Effects")]
        private int stunTurnsRemaining = 0;
        public bool IsStunned => stunTurnsRemaining > 0;
        
        public void Awake()
        {
            InitializeStats();
            targetPosition = transform.position;
            entityParentObject = transform.parent.gameObject;
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
        private void InitializeStats()
        {
            currentHealth = entityData.maxHealth;
        }

        public virtual void TakeDamage(int  damage)
        {
            if (!isAlive) return;
            currentHealth -= damage;
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
                return;
            }
            
            Debug.Log($"StartTurn by {entityName}");
        }

        public virtual void Attack(Entity target)
        {
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