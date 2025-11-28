using UnityEngine;

public class MapUIController : MonoBehaviour
{
    [SerializeField] private MapNodeButton[] nodeButtons;

    private void Start()
    {
        var run = RunManager.Instance;
        if (run == null || run.currentMapPath == null) return;

        var map = run.currentMapPath;

        for (int i = 0; i < nodeButtons.Length; i++)
        {
            if (i >= map.Length)
            {
                nodeButtons[i].gameObject.SetActive(false);
                continue;
            }

            nodeButtons[i].Initialize(i, map[i]);
        }
    }
}