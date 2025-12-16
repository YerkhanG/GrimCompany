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
    public string id;
    public MapNodeType type;
    public int[] nextNodeIndices;
    
    public int row;
    public int col;
}