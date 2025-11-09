using System;
using controller.combat;
using entity;
using events;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace UI.combat
{
    public class CombatUI : MonoBehaviour
    {
        [Header("UI Panels")]
        [SerializeField] private GameObject actionPanel;
        [Header("UI Buttons")]
        [SerializeField] private Button attackButton;

        [Header("For testing")] [SerializeField]
        private Entity entity;

        void Awake()
        {
            UIDeactivate();
        }

        void OnEnable()
        {
            attackButton.onClick.AddListener(OnAttackButtonClicked);
            CombatEvents.OnPlayerTurnStarted += HandleTurnStarted;
            CombatEvents.OnPlayerTurnEnded += HandleTurnEnded;
        }

        void OnDisable()
        {
            attackButton.onClick.RemoveListener(OnAttackButtonClicked);
            CombatEvents.OnPlayerTurnStarted -= HandleTurnStarted;
            CombatEvents.OnPlayerTurnEnded -= HandleTurnEnded;
        }
        private void HandleTurnEnded()
        {
            UIDeactivate();
        }

        private void HandleTurnStarted(Entity obj)
        {
            UIActivate();
        }

        private void OnAttackButtonClicked()
        {
            Entity target = FindFirstEnemy();
            
            if (target != null)
            {
                CombatEvents.RaiseAttackButtonClicked(target);
            }
        }

        private Entity FindFirstEnemy()
        {
            foreach (var entity in CombatManager.Instance.enemyList)
            {
                if (entity.isAlive)
                    return entity;
            }
            return null;
        }

        public void UIActivate()
        {
            Debug.Log($"UIActivate");
            actionPanel.SetActive(true);
        }

        public void UIDeactivate()
        {
            Debug.Log($"UIDeactivate");
            actionPanel.SetActive(false);
        }
    }
}