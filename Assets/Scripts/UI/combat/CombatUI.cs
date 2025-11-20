using System.Collections.Generic;
using combat;
using entity;
using events;
using UnityEngine;
using UnityEngine.UI;

namespace UI.combat
{
    public class CombatUI : MonoBehaviour
    {
        [Header("UI Panels")]
        [SerializeField] private GameObject actionPanel;
        [Header("UI Buttons")]
        [SerializeField] private Button attackButton;
        [SerializeField] private Button cancelButton;
        [Header("For testing")] [SerializeField]
        private Entity entity;

        void Awake()
        {
            UIDeactivate();
        }

        void OnEnable()
        {
            attackButton.onClick.AddListener(OnAttackButtonClicked);
            cancelButton.onClick.AddListener(OnCancelButtonClicked);
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
            CombatEvents.RaiseAttackButtonClicked();
            HideActionButtons();
            ShowCancelButton();
        }
        private void OnCancelButtonClicked()
        {
            CombatEvents.RaiseCancelButtonClicked();
            ShowActionButtons();
            HideCancelButton();
        }

        private void HideCancelButton()
        {
            cancelButton.gameObject.SetActive(false);
        }
        private void ShowCancelButton()
        {
            cancelButton.gameObject.SetActive(true);
        }
    
        public void UIActivate()
        {
            Debug.Log($"UIActivate");
            actionPanel.SetActive(true);
            ShowActionButtons();
            HideCancelButton();
        }

        public void UIDeactivate()
        {
            Debug.Log($"UIDeactivate");
            actionPanel.SetActive(false);
        }

        public void ShowActionButtons()
        {
            attackButton.gameObject.SetActive(true);
        }

        public void HideActionButtons()
        {
            attackButton.gameObject.SetActive(false);
        }
    }
}