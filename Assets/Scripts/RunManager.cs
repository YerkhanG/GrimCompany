using System;
using System.Collections.Generic;
using persistence;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunManager : MonoBehaviour
{
    public static RunManager Instance { get; private set; }

    [Header("Map state")] public MapNode[] currentMapPath;

    // Start BEFORE any node; set this to -1 in inspector or here:
    public int currentNodeIndex = -1;

    public List<HeroData> party = new List<HeroData>();

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
        if (currentMapPath == null || currentMapPath.Length == 0)
        {
            currentNodeIndex = -1;
        }
        else if (currentNodeIndex >= currentMapPath.Length)
        {
            currentNodeIndex = -1;
        }
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
            case MapNodeType.Boss:
                SceneManager.LoadScene("Boss Combat");
                break;
            case MapNodeType.Combat:
                SceneManager.LoadScene("Combat Scene");
                break;
        }
    }

    public void OnBattleWon()
    {
        var node = currentMapPath[currentNodeIndex];

        float bounty = 20f;

        // Give bounty to Purchaser
        if (Purchaser.Instance != null)
        {
            Purchaser.Instance.AddFunds(bounty);
        }

        if (node.type == MapNodeType.Boss)
        {
            ResetRun();
            SceneManager.LoadScene("Lobby");
            return;
        }

        SceneManager.LoadScene("Map Scene");
    }

    public void OnBattleLost()
    {
        ResetRun();
        SceneManager.LoadScene("Lobby");
    }
    
    public void ResetRun()
    {
        currentMapPath = null;
        currentNodeIndex = -1;
    }
}