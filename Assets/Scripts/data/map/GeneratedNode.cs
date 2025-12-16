using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GeneratedNode
{
    public int index;              // unique runtime index
    public int row;                // 0..height-1
    public int col;                // 0..width-1 or a float x
    public MapNodeTypeDef typeDef; // reference to SO
    public List<int> next = new List<int>(); // indices of nodes in next row
}