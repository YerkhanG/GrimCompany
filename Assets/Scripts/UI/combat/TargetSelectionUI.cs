using System.Collections.Generic;
using entity;
using events;
using UnityEngine;
using UnityEngine.UI;

namespace UI.combat
{
    public class TargetSelectionUI : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] private GameObject targetIconPrefab;
        [Header("Position")]
        [SerializeField] private Vector2 indicatorOffset = new Vector2(0, 50);
        [Header("References")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Canvas canvas;
        
        
        
        private List<GameObject> activeIndicators = new List<GameObject>();
        private Dictionary<GameObject, Entity> indicatorToEntity = new Dictionary<GameObject, Entity>();
        
        private bool isSelecting = false;
        
        void Awake()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
        }
        void OnEnable()
        {
            CombatEvents.OnTargetCalculated += HandleTargetsAvailable;
            CombatEvents.OnCancelButtonClicked += HandleCancelButtonClicked;
        }

        void OnDisable()
        {
            CombatEvents.OnTargetCalculated -= HandleTargetsAvailable;
            CombatEvents.OnCancelButtonClicked -= HandleCancelButtonClicked;
        }
        private void HandleTargetsAvailable(List<Entity> validTargets)
        {
            ClearIndicators();
            
            if (validTargets == null || validTargets.Count == 0)
            {
                Debug.LogWarning("No valid targets available!");
                return;
            }

            isSelecting = true;
            foreach (var target in validTargets)
            {
                CreateIndicator(target);
            }
        }

        private void CreateIndicator(Entity target)
        {
            GameObject indicator = Instantiate(targetIconPrefab, canvas.transform);
            Button button = indicator.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnTargetSelected(target));
            }
            else
            {
                Debug.LogError("Target indicator prefab is missing Button component!");
            }
            PositionIndicator(indicator, target);

            activeIndicators.Add(indicator);
            indicatorToEntity[indicator] = target;
        }
        private void OnTargetSelected(Entity target)
        {
            if (!isSelecting) return;
            
            Debug.Log($"Target selected: {target.name}");
            
            CombatEvents.RaiseTargetSelected(target);
            ClearIndicators();
            
            isSelecting = false;
        }
        
        private void PositionIndicator(GameObject indicator, Entity target)
        {
            Vector3 worldPosition = target.transform.position;
            
            Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(mainCamera, worldPosition);

            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            Vector2 canvasPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, 
                screenPosition, 
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera, 
                out canvasPosition
            );
            
            RectTransform indicatorRect = indicator.GetComponent<RectTransform>();
            indicatorRect.anchoredPosition = canvasPosition + indicatorOffset;
        }

        private void ClearIndicators()
        {
            foreach (var indicator in activeIndicators)
            {
                if (indicator != null)
                    Destroy(indicator);
            }
            
            activeIndicators.Clear();
            indicatorToEntity.Clear();
            isSelecting = false;
        }
        
        private void HandleCancelButtonClicked()
        {
            ClearIndicators();
        }
    }
}