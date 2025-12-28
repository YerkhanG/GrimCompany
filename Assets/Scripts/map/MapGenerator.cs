using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Layout")] [Min(2)] public int height = 6; // rows
    [Min(1)] public int width = 4; // max nodes per row (spread)
    [Range(0, 1)] public float rowNodeDensity = 0.7f; // % of columns used per row (except enforced start/boss)

    [Header("Types")] public MapNodeTypeDef enemyType; // quick reference (optional)
    public MapNodeTypeDef bossType; // quick reference
    public List<MapNodeTypeDef> availableTypes; // weighted pool for non-last rows

    [Header("Connection")] [Range(1, 3)]
    public int maxParentsWindow = 1; // edges allowed from row r to r+1 within +/- window columns

    public bool ensureReachability = true; // enforce at least one path snake

    [Header("Instancing")] public RectTransform nodeRoot; // parent under Canvas for nodes
    public GameObject connectionPrefab; // your MapConnection (tiled or dotted)
    public RectTransform connectionRoot;

    // Outputs
    [System.NonSerialized] private List<GeneratedNode> graph; // generated DAG

    Dictionary<int, RectTransform> rtByIndex = new Dictionary<int, RectTransform>();

    [SerializeField] private float horizontalMargin = 24f;
    [SerializeField] private float verticalMargin = 24f;

    private void ClearChildren(Transform root)
    {
        for (int i = root.childCount - 1; i >= 0; i--)
        {
            if (Application.isPlaying)
                Destroy(root.GetChild(i).gameObject);
            else
                DestroyImmediate(root.GetChild(i).gameObject);
        }
    }

    public void GenerateAndBuild()
    {
        if (nodeRoot != null) ClearChildren(nodeRoot);
        if (connectionRoot != null) ClearChildren(connectionRoot);

        rtByIndex.Clear();
        graph = GenerateGraph();
        PushToRunManager(graph);
        InstantiateGraphUI();
        RefreshNodeInteractables();
    }

    private void RefreshNodeInteractables()
    {
        if (nodeRoot == null) return;
        var buttons = nodeRoot.GetComponentsInChildren<MapNodeButton>(true);
        foreach (var b in buttons) b.UpdateInteractable();
    }

    private List<GeneratedNode> GenerateGraph()
    {
        var nodes = new List<GeneratedNode>();
        int index = 0;

        // Helper: create node
        GeneratedNode MakeNode(int row, int col, MapNodeTypeDef def)
        {
            return new GeneratedNode { index = index++, row = row, col = col, typeDef = def };
        }

        // 1) Build rows
        var rows = new List<List<GeneratedNode>>(height);
        for (int r = 0; r < height; r++) rows.Add(new List<GeneratedNode>());

        // Start row (0): exactly one node, type enemy
        rows[0].Add(MakeNode(0, width / 2, enemyType));

        // Middle rows
        var usableTypes = new List<MapNodeTypeDef>(availableTypes);
        usableTypes.RemoveAll(t => t == null || t.mustBeUnique || t.onlyAllowedOnLastRow);
        if (usableTypes.Count == 0)
            usableTypes.Add(enemyType);

        for (int r = 1; r < height - 1; r++)
        {
            int targetCount = Mathf.Max(1, Mathf.RoundToInt(width * rowNodeDensity));
            var usedColumns = new HashSet<int>();
            for (int k = 0; k < targetCount; k++)
            {
                int col = UniqueRandomColumn(usedColumns);
                var def = PickWeightedType(usableTypes);
                rows[r].Add(MakeNode(r, col, def));
            }

            rows[r].Sort((a, b) => a.col.CompareTo(b.col));
        }

        // Boss row
        rows[height - 1].Add(MakeNode(height - 1, width / 2, bossType));

        // 2) Flatten
        foreach (var rr in rows) nodes.AddRange(rr);

        // 3) Connections
        ConnectRows(rows);

        // 4) Ensure at least one guaranteed path (optional)
        if (ensureReachability) EnsureSnake(rows);

        return nodes;
    }

    private int UniqueRandomColumn(HashSet<int> used)
    {
        // Try random columns until one free found, fallback to nearest free
        for (int tries = 0; tries < 20; tries++)
        {
            int col = Random.Range(0, width);
            if (used.Add(col)) return col;
        }

        // fallback
        for (int col = 0; col < width; col++)
            if (used.Add(col))
                return col;
        return Mathf.Clamp(width - 1, 0, width - 1);
    }

    private MapNodeTypeDef PickWeightedType(List<MapNodeTypeDef> pool)
    {
        float total = 0f;
        foreach (var t in pool) total += Mathf.Max(0f, t.rarity);
        float roll = Random.value * (total <= 0f ? 1f : total);
        foreach (var t in pool)
        {
            float w = Mathf.Max(0f, t.rarity);
            if (roll <= w) return t;
            roll -= w;
        }

        return pool.Count > 0 ? pool[0] : enemyType;
    }

    private void ConnectRows(List<List<GeneratedNode>> rows)
    {
        for (int r = 0; r < rows.Count - 1; r++)
        {
            var cur = rows[r];
            var nxt = rows[r + 1];

            // 1) First pass: each node in cur gets 1–2 children
            foreach (var n in cur)
            {
                var candidates = new List<GeneratedNode>();
                foreach (var m in nxt)
                    if (Mathf.Abs(m.col - n.col) <= maxParentsWindow)
                        candidates.Add(m);

                if (candidates.Count == 0)
                {
                    var nearest = NearestByCol(nxt, n.col);
                    if (nearest != null) AddUnique(n.next, nearest.index);
                    continue;
                }

                var first = candidates[Random.Range(0, candidates.Count)];
                AddUnique(n.next, first.index);

                if (candidates.Count > 1 && Random.value < 0.5f)
                {
                    var second = first;
                    int guard = 0;
                    while (second.index == first.index && guard++ < 10)
                        second = candidates[Random.Range(0, candidates.Count)];

                    AddUnique(n.next, second.index);
                }
            }

            // 2) Second pass: ensure every node in nxt has at least one parent
            var hasParent = new HashSet<int>();
            foreach (var p in cur)
            foreach (var child in p.next)
                hasParent.Add(child);

            foreach (var child in nxt)
            {
                if (hasParent.Contains(child.index)) continue;

                // Prefer parents within window, otherwise nearest overall
                var parentCandidates = new List<GeneratedNode>();
                foreach (var p in cur)
                    if (Mathf.Abs(p.col - child.col) <= maxParentsWindow)
                        parentCandidates.Add(p);

                var parent = parentCandidates.Count > 0
                    ? parentCandidates[Random.Range(0, parentCandidates.Count)]
                    : NearestByCol(cur, child.col);

                if (parent != null)
                {
                    AddUnique(parent.next, child.index);
                    hasParent.Add(child.index);
                }
            }
        }
    }

    private static void AddUnique(List<int> list, int value)
    {
        if (!list.Contains(value)) list.Add(value);
    }

    private static GeneratedNode NearestByCol(List<GeneratedNode> list, int col)
    {
        GeneratedNode best = null;
        int bestD = int.MaxValue;

        foreach (var n in list)
        {
            int d = Mathf.Abs(n.col - col);
            if (d < bestD)
            {
                bestD = d;
                best = n;
            }
        }

        return best;
    }

    private void EnsureSnake(List<List<GeneratedNode>> rows)
    {
        // Force a guaranteed single path through random nearest children
        var path = new List<GeneratedNode>();
        var cur = rows[0][0];
        path.Add(cur);
        for (int r = 1; r < rows.Count; r++)
        {
            // pick nearest in next row; ensure parent has link to it
            var nextRow = rows[r];
            GeneratedNode nearest = null;
            int best = 999;
            foreach (var m in nextRow)
            {
                int d = Mathf.Abs(m.col - cur.col);
                if (d < best)
                {
                    best = d;
                    nearest = m;
                }
            }

            if (nearest != null)
            {
                if (!cur.next.Contains(nearest.index))
                    cur.next.Add(nearest.index);
                cur = nearest;
                path.Add(cur);
            }
        }
    }

    private void InstantiateGraphUI()
    {
        if (nodeRoot == null) return;

        // Make sure rect sizes are correct before using nodeRoot.rect
        Canvas.ForceUpdateCanvases();

        rtByIndex.Clear();

        Rect rect = nodeRoot.rect;

        // Local-space bounds inside MapArea
        float left = rect.xMin + horizontalMargin;
        float right = rect.xMax - horizontalMargin;
        float top = rect.yMax - verticalMargin;
        float bottom = rect.yMin + verticalMargin;

        foreach (var n in graph)
        {
            var prefab = n.typeDef != null && n.typeDef.nodePrefab != null
                ? n.typeDef.nodePrefab
                : enemyType.nodePrefab;

            var go = Instantiate(prefab, nodeRoot);
            var rt = go.GetComponent<RectTransform>();

            // Force predictable UI anchoring
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.localScale = Vector3.one;

            float tx = (width <= 1) ? 0.5f : (n.col / (width - 1f));
            float ty = (height <= 1) ? 0.5f : (n.row / (height - 1f));

            float x = Mathf.Lerp(left, right, tx);
            float y = Mathf.Lerp(top, bottom, ty);

            rt.anchoredPosition = new Vector2(x, y);

            rtByIndex[n.index] = rt;

            var btn = go.GetComponent<MapNodeButton>();
            if (btn != null)
            {
                btn.Initialize(n.index, new MapNode
                {
                    id = n.typeDef != null ? n.typeDef.id : "Enemy",
                    type = n.typeDef != null ? n.typeDef.logicalType : MapNodeType.Combat,
                    nextNodeIndices = n.next.ToArray()
                });
            }
        }

        // connections
        if (connectionPrefab != null && connectionRoot != null)
        {
            foreach (var n in graph)
            {
                foreach (var child in n.next)
                {
                    if (!rtByIndex.TryGetValue(n.index, out var fromRT)) continue;
                    if (!rtByIndex.TryGetValue(child, out var toRT)) continue;

                    var conn = Instantiate(connectionPrefab, connectionRoot);
                    var dotted = conn.GetComponent<DottedConnectionView>();
                    if (dotted != null)
                    {
                        Debug.Log($"Conn {n.index}->{child} from={fromRT.position} to={toRT.position}");
                        dotted.Initialize(fromRT, toRT);
                    }
                }
            }
        }
    }

    private float GetHorizontalSpacing(RectTransform area, int columns)
    {
        float usable = Mathf.Max(0f, area.rect.width - horizontalMargin * 2f);
        return (columns <= 1) ? 0f : usable / (columns - 1);
    }

    private float GetVerticalSpacing(RectTransform area, int rows)
    {
        float usable = Mathf.Max(0f, area.rect.height - verticalMargin * 2f);
        return (rows <= 1) ? 0f : usable / (rows - 1);
    }

    private void PushToRunManager(List<GeneratedNode> data)
    {
        var rm = RunManager.Instance;
        if (rm == null) return;

        var map = new MapNode[data.Count];
        foreach (var g in data)
        {
            map[g.index] = new MapNode
            {
                id = g.typeDef != null ? g.typeDef.id : "Enemy",
                type = g.typeDef != null ? g.typeDef.logicalType : MapNodeType.Combat,
                nextNodeIndices = g.next.ToArray(),
                row = g.row,
                col = g.col
            };
        }

        rm.currentMapPath = map;
        rm.currentNodeIndex = -1;
    }

    public void BuildFromExisting(MapNode[] map)
    {
        if (map == null || map.Length == 0) return;

        if (nodeRoot != null) ClearChildren(nodeRoot);
        if (connectionRoot != null) ClearChildren(connectionRoot);

        rtByIndex.Clear();

        graph = new List<GeneratedNode>(map.Length);

        for (int i = 0; i < map.Length; i++)
        {
            var m = map[i];

            // Resolve prefab/typeDef from stored node info
            var def = ResolveTypeDef(m);

            var g = new GeneratedNode
            {
                index = i,
                row = m.row,
                col = m.col,
                typeDef = def
            };

            if (m.nextNodeIndices != null)
                g.next.AddRange(m.nextNodeIndices);

            graph.Add(g);
        }

        InstantiateGraphUI();
    }

    private MapNodeTypeDef ResolveTypeDef(MapNode node)
    {
        if (node.type == MapNodeType.Boss) return bossType;

        // Prefer matching by id if possible
        if (availableTypes != null)
        {
            for (int i = 0; i < availableTypes.Count; i++)
            {
                var t = availableTypes[i];
                if (t != null && t.id == node.id) return t;
            }
        }

        return enemyType;
    }
}