using UnityEngine;
using UnityEngine.UI;

namespace UI.general
{
    public class PopupManager: MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject popupPanel;
        [SerializeField] private Button button;
        [SerializeField] private Button closeButton;
        
        private void Start()
        {
            popupPanel.SetActive(false);
            
            button.onClick.AddListener(ShowPopup);
            closeButton.onClick.AddListener(HidePopup);
        }
        
        private void ShowPopup()
        {
            popupPanel.SetActive(true);
        }
        
        private void HidePopup()
        {
            popupPanel.SetActive(false);
        }
        
        private void OnDestroy()
        {
            button.onClick.RemoveListener(ShowPopup);
            closeButton.onClick.RemoveListener(HidePopup);
        }
    }
}