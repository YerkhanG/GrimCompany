using System;
using data;
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
        public void Awake()
        {
            InitializeStats();
        }
        private void InitializeStats()
        {
            currentHealth = entityData.maxHealth;
        }

        public virtual void TakeDamage(int  damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public virtual void StartTurn()
        {
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
            isAlive = false;
            
        }
    }
}