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
        if (run != null && run.currentMapPath != null && run.currentMapPath.Length > 0)
        {
            // Map already exists (coming back from combat) -> just rebuild UI
            generator.BuildFromExisting(run.currentMapPath);
        }
        else
        {
            // New run -> generate and push to RunManager
            generator.GenerateAndBuild();
        }
    }
}