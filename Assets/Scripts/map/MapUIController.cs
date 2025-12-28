using UnityEngine;

public class MapUIController : MonoBehaviour
{
    [Header("Generation")] [SerializeField]
    private MapGenerator generator; // assign in inspector

    [Header("Legacy (optional)")] [SerializeField]
    private MapNodeButton[] nodeButtons; // only if you still use fixed buttons

    private void Start()
    {
        if (generator == null) return;

        var run = RunManager.Instance;

        if (run != null &&
            run.currentMapPath != null &&
            run.currentMapPath.Length > 0 &&
            run.currentNodeIndex >= 0)
        {
            generator.BuildFromExisting(run.currentMapPath);
        }
        else
        {
            generator.GenerateAndBuild();
        }
    }
}