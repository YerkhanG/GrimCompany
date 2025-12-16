using UnityEngine;

[CreateAssetMenu(menuName = "Map/Node Type", fileName = "MapNodeType_")]
public class MapNodeTypeDef : ScriptableObject
{
    public string id;                    // "Enemy", "Boss", "Shop"
    public MapNodeType logicalType;      // your enum: Combat, Shop, Treasure, Boss
    public GameObject nodePrefab;        // prefab with MapNodeButton etc.
    [Range(0f, 1f)] public float rarity = 0.5f; // relative weight
    public bool mustBeUnique;            // e.g., Boss
    public bool onlyAllowedOnLastRow;    // Boss true
    public bool forbiddenOnLastRow;      // e.g., Shop false, Enemy false, etc.
}