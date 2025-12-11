using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using entity;
using events;
using UI.healthBar;

namespace combat
{
    public class EntityHealthBarUIManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject healthBarPrefab;
        [SerializeField] private Transform healthBarContainer;
        [SerializeField] private Camera mainCamera;
        
        [Header("Settings")]
        [SerializeField] private Vector3 worldOffset = new Vector3(0, 2f, 0);
        
        private Dictionary<Entity, HealthBarData> healthBars = new Dictionary<Entity, HealthBarData>();
        [SerializeField] Canvas canvas;

        private void Awake()
        {
            CombatEvents.OnCombatStarted += InitializeHealthBars;
            CombatEvents.OnDamageTaken += UpdateHealthBar;
            CombatEvents.OnHealingTaken += UpdateHealthBar;
            CombatEvents.OnEntityDied += RemoveHealthBar;
        }

        private void Start()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
            canvas = healthBarContainer.GetComponentInParent<Canvas>();
            
        }

        private void InitializeHealthBars(List<Entity> allEntities)
        {
            Debug.Log("Firing health bars");
            foreach (Entity entity in allEntities)
            {
                CreateHealthBar(entity);
            }
        }

        private void CreateHealthBar(Entity entity)
        {
            GameObject barObj = Instantiate(healthBarPrefab, healthBarContainer);
            
            HealthBarData data = new HealthBarData
            {
                rectTransform = barObj.GetComponent<RectTransform>(),
                slider = barObj.GetComponentInChildren<Slider>(),
                fillImage = barObj.GetComponentInChildren<Image>(),
                healthText = barObj.GetComponentInChildren<TextMeshProUGUI>(),
                canvasGroup = barObj.GetComponent<CanvasGroup>()
            };
            
            if (data.canvasGroup == null)
                data.canvasGroup = barObj.AddComponent<CanvasGroup>();

            if (data.slider != null)
            {
                data.slider.maxValue = entity.MaxHealth;
                data.slider.value = entity.currentHealth;
            }
            
            healthBars[entity] = data;
            UpdateHealthBar(entity, 0); 
        }

        private void UpdateHealthBar(Entity entity, int amount)
        {
            if (!healthBars.TryGetValue(entity, out HealthBarData data)) return;

            if (data.slider != null)
            {
                data.slider.value = entity.currentHealth;
            }
    
            if (data.healthText != null)
            {
                data.healthText.text = $"{entity.currentHealth}/{entity.MaxHealth}";
            }
        }

        private void RemoveHealthBar(Entity entity)
        {
            if (healthBars.TryGetValue(entity, out HealthBarData data))
            {
                StartCoroutine(FadeOutAndDestroy(data));
                healthBars.Remove(entity);
            }
        }

        private System.Collections.IEnumerator FadeOutAndDestroy(HealthBarData data)
        {
            float duration = 0.5f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                if (data.canvasGroup)
                    data.canvasGroup.alpha = 1f - (elapsed / duration);
                yield return null;
            }
            
            if (data.rectTransform)
                Destroy(data.rectTransform.gameObject);
        }

        private void LateUpdate()
        {
            foreach (var kvp in healthBars)
            {
                Entity entity = kvp.Key;
                HealthBarData data = kvp.Value;
                
                if (!entity || !data.rectTransform) continue;
                
                Vector3 worldPos = entity.transform.position + worldOffset;
                Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);
                
                if (screenPos.z < 0)
                {
                    data.rectTransform.gameObject.SetActive(false);
                    continue;
                }
                
                data.rectTransform.gameObject.SetActive(true);

                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.GetComponent<RectTransform>(),
                    screenPos,
                    canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera,
                    out Vector2 canvasPos
                );
                
                data.rectTransform.anchoredPosition = canvasPos;
            }
        }

        private void OnDestroy()
        {
            CombatEvents.OnCombatStarted -= InitializeHealthBars;
            CombatEvents.OnDamageTaken -= UpdateHealthBar;
            CombatEvents.OnHealingTaken -= UpdateHealthBar;
            CombatEvents.OnEntityDied -= RemoveHealthBar;
        }
    }
}