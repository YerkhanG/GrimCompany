using entity;
using events;
using UnityEngine;

namespace UI.damage_and_healing_numbers
{
    public class DamageAndHealingNumbersSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject damageNumberPrefab;
        [SerializeField] private Transform numbersContainer; 
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Canvas canvas;
        [Header("Settings")]
        [SerializeField] private Vector3 spawnOffset = new Vector3(0, 1.5f, 0);
        [SerializeField] private Vector3 randomOffset = new Vector3(0.5f, 0.3f, 0);
        [Header("Colors")]
        [SerializeField] private Color damageColor = Color.red;
        [SerializeField] private Color healColor = Color.green;

        private void Awake()
        {
            CombatEvents.OnDamageTaken += SpawnDamageNumber;
            CombatEvents.OnHealingTaken += SpawnHealingNumber;
        }
        private void OnDestroy()
        {
            CombatEvents.OnDamageTaken -= SpawnDamageNumber;
            CombatEvents.OnHealingTaken -= SpawnHealingNumber;
        }

        private void SpawnHealingNumber(Entity entity, int heal)
        {
            SpawnNumber(entity, heal, healColor);
        }

        private void SpawnDamageNumber(Entity entity, int damage)
        {
            SpawnNumber(entity, damage, damageColor);
        }
        
        private void SpawnNumber(Entity target, int value , Color color)
        {
            Vector3 randomizedOffset = spawnOffset + new Vector3(
                Random.Range(-randomOffset.x, randomOffset.x),
                Random.Range(-randomOffset.y, randomOffset.y),
                Random.Range(-randomOffset.z, randomOffset.z)
            );
            Vector3 worldPos = target.transform.position + randomizedOffset;
            Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);
            if (screenPos.z < 0) return;
            GameObject numberObj = Instantiate(damageNumberPrefab, numbersContainer);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                screenPos,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera,
                out Vector2 canvasPos
            );
            RectTransform rectTransform = numberObj.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = canvasPos;
            NumberUI numberUI = numberObj.GetComponent<NumberUI>();
            if (numberUI != null)
            {
                numberUI.Initialize(value, color);
            }
        }
    }
}