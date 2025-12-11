using TMPro;
using UnityEngine;

namespace UI.damage_and_healing_numbers
{
    public class NumberUI : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private CanvasGroup canvasGroup;
        [Header("Animation Settings")]
        [SerializeField] private float lifetime = 1.5f;
        [SerializeField] private float floatSpeed = 50f;
        [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0.5f, 0.2f, 1f);
        private float timer;
        private Vector2 startPosition;
        private RectTransform rectTransform;
        
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }
        public void Initialize(int value, Color color)
        {
            if (text != null)
            {
                text.text = value.ToString();
                text.color = color;
            }
            
            timer = 0f;
            startPosition = rectTransform.anchoredPosition;
        }
        private void Update()
        {
            timer += Time.deltaTime;
            float progress = timer / lifetime;

            Vector2 currentPos = startPosition + Vector2.up * (floatSpeed * timer);
            rectTransform.anchoredPosition = currentPos;
            if (canvasGroup)
            {
                canvasGroup.alpha = fadeCurve.Evaluate(progress);
            }
            if (progress < 0.2f)
            {
                float scale = scaleCurve.Evaluate(progress);
                rectTransform.localScale = Vector3.one * scale;
            }
            if (timer >= lifetime)
            {
                Destroy(gameObject);
            }
        }
    }
}