using UnityEngine;

public class DottedConnectionView : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private RectTransform container; 

    [SerializeField] private RectTransform dotPrefab;

    [Header("Layout")] [SerializeField] private float spacing = 24f;
    [SerializeField] private int minDots = 6;
    [SerializeField] private int maxDots = 64;
    [SerializeField] private bool includeEndpoints = false;
    [SerializeField] private bool alignToDirection = false;

    private void Awake()
    {
        if (container == null)
            container = GetComponent<RectTransform>();
    }

    public void Initialize(RectTransform from, RectTransform to)
    {
        if (from == null || to == null || container == null || dotPrefab == null)
            return;

        for (int i = container.childCount - 1; i >= 0; i--)
            Destroy(container.GetChild(i).gameObject);

        var parent = container.parent as RectTransform;
        if (parent == null) return;
        Vector2 a = parent.InverseTransformPoint(from.TransformPoint(from.rect.center));
        Vector2 b = parent.InverseTransformPoint(to.TransformPoint(to.rect.center));

        Vector2 dir = b - a;
        float length = dir.magnitude;
        if (length < 0.01f) return;
        
        container.anchorMin = container.anchorMax = new Vector2(0.5f, 0.5f);
        container.pivot = new Vector2(0.5f, 0.5f);
        container.localScale = Vector3.one;
        
        container.anchoredPosition = (a + b) * 0.5f;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        container.localEulerAngles = new Vector3(0f, 0f, angle);

        container.sizeDelta = new Vector2(length, container.sizeDelta.y);
        
        float baseSpacing = Mathf.Max(1f, spacing);
        int bySpacing = Mathf.Max(1, Mathf.FloorToInt(length / baseSpacing));
        int count = Mathf.Clamp(Mathf.Max(bySpacing, minDots), 1, maxDots);

        float effectiveSpacing =
            (count > 1)
                ? (length / (includeEndpoints ? (count - 1) : (count + 1)))
                : length;

        float start = includeEndpoints ? 0f : effectiveSpacing;

        for (int i = 0; i < count; i++)
        {
            float x = start + i * effectiveSpacing;
            float localX = x - length * 0.5f;

            var dot = Instantiate(dotPrefab, container);
            
            dot.anchorMin = dot.anchorMax = new Vector2(0.5f, 0.5f);
            dot.pivot = new Vector2(0.5f, 0.5f);
            dot.localScale = Vector3.one;
            dot.localRotation = Quaternion.identity;

            dot.anchoredPosition = new Vector2(localX, 0f);
        }
    }
}

public static class VecExt
{
    public static Vector3 WithZ(this Vector3 v, float z)
    {
        v.z = z;
        return v;
    }
}