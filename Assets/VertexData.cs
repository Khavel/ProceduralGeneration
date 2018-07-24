using UnityEngine;
using System;

/// <summary>
/// This class contains all the data needed to store the data from each vertex on a mesh.
/// </summary>
public class VertexData : IComparable<VertexData>{

    /// <summary>
    /// This variable will hold the position of the vertex inside its own tile.
    /// </summary>
    public int position;

    /// <summary>
    /// This variable will hold the vertex's tile.
    /// </summary>
    public int tile;

    /// <summary>
    /// This variable will hold the value of the vertex.
    /// </summary>
    public Vector3 vector;

    /// <summary>
    /// This variable will hold the vertex id, a number indicating the order in which the vertex was created.
    /// </summary>
    public int id;

    /// <summary>
    /// This variable will hold the verexs' row, which is the row of the vertex matrix where it is located.
    /// </summary>
    public int row;


    public VertexData(Vector3 vector,int position, int tile, int id, int row) {
        this.vector = vector;
        this.position = position;
        this.tile = tile;
        this.id = id;
        this.row = row;
    }



    /// <summary>
    /// This method is used to order the vertices so that they for a matrix.
    /// </summary>
    /// <param name="other">
    /// The VertexData to compare this one to.
    /// </param>
    /// <returns>
    /// This method returns a number less than zero if this object is greater than other, zero if they are the same, although it should never occur,
    /// and a number grater than zero if this object is smaller than other.
    /// </returns>
    public int CompareTo(VertexData other) {

        //First the row is compared, and, were they equal, then the tile number is compared, otherwise, the comparison between rows is returned.
        if(row == other.row) {

            //If the tiles are equal, then the positions are compared, if not, the tiles are comapred.
            if(tile == other.tile) {

                return other.position - position;
            } else {

                return other.tile - tile;
            }
        } else {

            return other.row - row;
        }
    }

}