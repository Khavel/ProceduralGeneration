using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileNode{
    public TileNode rightNeighbour, leftNeighbour, topNeighbour, botNeighbour;
    public Vector3 rightVertex, leftVertex, topVertex, botVertex;
    public Vector3 topRightVertex, topLeftVertex, botRightVertex, botLeftVertex;
}
