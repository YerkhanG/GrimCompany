using System;
using System.Collections.Generic;
using data;
using entity;
using events;
using UnityEngine;
using UnityEngine.UI;

namespace UI.combat
{
    public class TargetSelectionUI : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject swordIconTargetPrefab;
        [SerializeField] private GameObject buffIconTargetPrefab;
        [SerializeField] private GameObject debuffIconTargetPrefab;
        [SerializeField] private GameObject actorIconTargetPrefab;
        [Header("Position")]
        [SerializeField] private Vector2 targetIndicatorOffset ;
        [SerializeField] private Vector2 actorIndicatorOffset;
        [Header("References")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Canvas canvas;
        
        private List<GameObject> activeIndicators = new List<GameObject>();
        private Dictionary<GameObject, Entity> indicatorToEntity = new Dictionary<GameObject, Entity>();
        private GameObject arrowIndicator;
        private bool isSelecting = false;
        
        void Awake()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
        }
        void OnEnable()
        {
            CombatEvents.OnTargetCalculated += HandleTargetsAvailable;
            CombatEvents.OnUtilityTargetCalculated += HandleUtilityTargetsAvailable;
            CombatEvents.OnCancelButtonClicked += HandleCancelButtonClicked;
            CombatEvents.OnCurrentActorPicked += HandleCurrentActorPicked;
        }

        void OnDisable()
        {
            CombatEvents.OnTargetCalculated -= HandleTargetsAvailable;
            CombatEvents.OnUtilityTargetCalculated -= HandleUtilityTargetsAvailable;
            CombatEvents.OnCancelButtonClicked -= HandleCancelButtonClicked;
            CombatEvents.OnCurrentActorPicked -= HandleCurrentActorPicked;
        }

        private void HandleCurrentActorPicked(Entity currentActor)
        {
            if (currentActor == null) return;
            ShowCurrentActorArrow(currentActor);
        }

        private void ShowCurrentActorArrow(Entity currentActor)
        {
            if (arrowIndicator != null)
            {
                Destroy(arrowIndicator);
            }
            if (actorIconTargetPrefab != null)
            {
                arrowIndicator = Instantiate(actorIconTargetPrefab, canvas.transform);
                PositionIndicator(arrowIndicator, currentActor, actorIndicatorOffset);
                
                Debug.Log($"Turn Arrow: Showing below {currentActor.name}");
            }
            else
            {
                Debug.LogWarning("Turn arrow prefab is not assigned!");
            }
        }

        private void HandleTargetsAvailable(List<Entity> validTargets)
        {
            ShowTargetIndicators(validTargets, swordIconTargetPrefab, OnTargetSelected);
        }
        
        private void HandleUtilityTargetsAvailable(List<Entity> validTargets, TargetType type)
        {
            GameObject prefab = GetUtilityPrefab(type);
            ShowTargetIndicators(validTargets, prefab, OnUtilityTargetSelected);
        }
        private GameObject GetUtilityPrefab(TargetType type)
        {
            switch (type)
            {
                case TargetType.AllAllies:
                    return buffIconTargetPrefab;
                case TargetType.AllEnemies:
                    return debuffIconTargetPrefab;
                default:
                    Debug.LogWarning($"Unknown TargetType: {type}. Using buff icon as default.");
                    return buffIconTargetPrefab;
            }
        }
        private void ShowTargetIndicators(List<Entity> validTargets, GameObject prefab, Action<Entity> onClickCallback)
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
                CreateIndicator(target, prefab, onClickCallback);
            }
        }
        
        private void CreateIndicator(Entity target, GameObject prefab, Action<Entity> onClickCallback)
        {
            GameObject indicator = Instantiate(prefab, canvas.transform);
            
            Button button = indicator.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => onClickCallback(target));
            }
            else
            {
                Debug.LogError("Target indicator prefab is missing Button component!");
            }
            
            PositionIndicator(indicator, target,targetIndicatorOffset);

            activeIndicators.Add(indicator);
            indicatorToEntity[indicator] = target;
        }
        private void PositionIndicator(GameObject indicator, Entity target, Vector2 indicatorOffset)
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
        private void OnTargetSelected(Entity target)
        {
            if (!isSelecting) return;
            
            Debug.Log($"Target selected: {target.name}");
            
            CombatEvents.RaiseTargetSelected(target);
            ClearIndicators();
            
            isSelecting = false;
        }
        private void OnUtilityTargetSelected(Entity target)
        {
            if (!isSelecting) return;
            
            Debug.Log($"Target selected: {target.name}");
            
            CombatEvents.RaiseUtilityTargetSelected(target);
            ClearIndicators();
            
            isSelecting = false;
        }
        private void HandleCancelButtonClicked()
        {
            ClearIndicators();
        }
    }
}