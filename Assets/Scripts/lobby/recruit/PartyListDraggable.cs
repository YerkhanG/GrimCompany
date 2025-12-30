using persistence;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace lobby.recruit
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PartyListDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler
    {
        [Header("UI Refs")]
        [SerializeField] private Image iconImage;
        
        public HeroData Data { get; private set; }
        private RecruitPartyUI _manager;
        
        private int startingSiblingIndex;
        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;
        
        private static PartyListDraggable currentlyDragging;
        private static GameObject dragVisual; 
        private Canvas canvas;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Init(HeroData data, RecruitPartyUI manager)
        {
            Data = data;
            _manager = manager;
            canvas = GetComponentInParent<Canvas>();
            LoadHeroIcon(data);
        }

        private void LoadHeroIcon(HeroData data)
        {
            if (iconImage == null)
            {
                Debug.LogWarning("Icon Image is not assigned in PartyListDraggable!");
                return;
            }

            if (data.icon != null)
            {
                iconImage.sprite = data.icon;
                iconImage.color = Color.white;
            }
            else
            {
                Debug.LogWarning($"Hero {data.prefabName} has no icon assigned!");
                iconImage.color = Color.gray;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            startingSiblingIndex = transform.GetSiblingIndex();
            currentlyDragging = this;
           
            canvasGroup.alpha = 0.3f;
            canvasGroup.blocksRaycasts = false;
            
            CreateDragVisual();
        }

        private void CreateDragVisual()
        {
            dragVisual = new GameObject("DragVisual");
            dragVisual.transform.SetParent(canvas.transform);
            dragVisual.transform.SetAsLastSibling();
            
            var img = dragVisual.AddComponent<Image>();
            img.sprite = iconImage.sprite;
            img.color = iconImage.color;
            
            var dragRect = dragVisual.GetComponent<RectTransform>();
            dragRect.sizeDelta = new Vector2(rectTransform.rect.width, rectTransform.rect.height);
            dragRect.pivot = new Vector2(0.5f, 0.5f);

            var cg = dragVisual.AddComponent<CanvasGroup>();
            cg.alpha = 0.8f;
            cg.blocksRaycasts = false; 
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (dragVisual != null)
            {
                dragVisual.transform.position = eventData.position;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (currentlyDragging != null && currentlyDragging != this)
            {
                int draggingIndex = currentlyDragging.transform.GetSiblingIndex();
                int ourIndex = transform.GetSiblingIndex();
                
                currentlyDragging.transform.SetSiblingIndex(ourIndex);
                transform.SetSiblingIndex(draggingIndex);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;

            if (dragVisual != null)
            {
                Destroy(dragVisual);
                dragVisual = null;
            }
            
            int finalIndex = transform.GetSiblingIndex();
            currentlyDragging = null;
            if (finalIndex != startingSiblingIndex)
            {
                _manager.OnListReordered();
            }
        }
    }
}