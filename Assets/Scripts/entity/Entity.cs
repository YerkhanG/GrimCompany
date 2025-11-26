using System.Collections;
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
        [Header("Death and repositioning")]
        [SerializeField] private float movementSpeed = 300f;
        [SerializeField] private float deathFadeDuration = 0.5f;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private GameObject entityTransform;
        private Vector3 targetPosition;
        private bool isMoving = false;
        public void Awake()
        {
            InitializeStats();
            targetPosition = transform.position;
        }
        private void Update()
        {
            if (isMoving)
            { 
                entityTransform.transform.position = Vector3.MoveTowards(
                    transform.position, 
                    targetPosition, 
                    movementSpeed * Time.deltaTime
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
            StartCoroutine(PlayDeathAnimation());
            CombatEvents.RaiseEntityDied(this);
        }
        public void SetTargetPosition(Vector3 newPosition)
        {
            if (!isAlive || !gameObject.activeInHierarchy) return;
            Debug.Log($"Setting target position to {newPosition}");
            targetPosition = newPosition;
            isMoving = true;
        }

        private IEnumerator PlayDeathAnimation()
        {
            if (spriteRenderer)
            {
                float elapsedTime = 0f;
                Color originalColor = spriteRenderer.color;
                while (elapsedTime < deathFadeDuration)
                {
                    elapsedTime += Time.deltaTime;
                    float alpha = Mathf.Lerp(originalColor.a, 0, elapsedTime / deathFadeDuration);
                    spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                    yield return null;
                }
            }
            else
            {
                yield return new WaitForSeconds(deathFadeDuration);
            }
            entityTransform.gameObject.SetActive(false);
        }
    }
}