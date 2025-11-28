using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum MapNodeType
{
    Combat,
    Treasure,
    Shop,
    Boss
}

[System.Serializable]
public class MapNode
{
    public string id; // e.g. "N0", "N1"
    public MapNodeType type;
    public int[] nextNodeIndices; // indices in an array for now
}

public class RunManager : MonoBehaviour
{
    public static RunManager Instance { get; private set; }

    [Header("Map state")] public MapNode[] currentMapPath; // assign in inspector for now

    // Start BEFORE any node; set this to -1 in inspector or here:
    public int currentNodeIndex = -1;

    [Header("Economy / party etc.")] public float gold;
    // public List<Hero> party; // later

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Ensure we start before any node
        if (currentNodeIndex >= currentMapPath.Length)
            currentNodeIndex = -1;
    }

    public MapNode CurrentNode =>
        currentMapPath != null &&
        currentNodeIndex >= 0 &&
        currentNodeIndex < currentMapPath.Length
            ? currentMapPath[currentNodeIndex]
            : null;

    public bool CanTravelTo(int nodeIndex)
    {
        if (currentMapPath == null) return false;

        // START OF RUN: allow clicking the first node(s)
        if (currentNodeIndex == -1)
        {
            // For now: only node 0 is a valid first node
            return nodeIndex == 0;
        }

        var node = CurrentNode;
        if (node == null || node.nextNodeIndices == null)
            return false;

        return Array.Exists(node.nextNodeIndices, i => i == nodeIndex);
    }

    public void TravelToNode(int nodeIndex)
    {
        if (!CanTravelTo(nodeIndex))
        {
            Debug.LogWarning($"Cannot travel from {currentNodeIndex} to {nodeIndex}");
            return;
        }

        currentNodeIndex = nodeIndex;
        var node = currentMapPath[nodeIndex];

        switch (node.type)
        {
            case MapNodeType.Combat:
            case MapNodeType.Boss:
                SceneManager.LoadScene("Combat Scene");
                break;
        }
    }

    public void OnBattleWon()
    {
        // Rewards
        gold += 20f;

        var node = currentMapPath[currentNodeIndex];

        if (node.type == MapNodeType.Boss)
        {
            // Optional: reset run state for a new run
            currentNodeIndex = -1;
            // maybe reset gold, map, etc.

            SceneManager.LoadScene("Lobby");
            return; // IMPORTANT: do not also load Map
        }

        // Normal node: go back to map to pick next node
        SceneManager.LoadScene("Map Scene");
    }

    public void OnBattleLost()
    {
        // On defeat, straight to lobby (or a game-over screen)
        SceneManager.LoadScene("Lobby");
    }
}