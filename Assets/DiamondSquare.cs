using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class DiamondSquare : MonoBehaviour {

    public int nTiles = 1;
    public Material material;
    public int nDivisions;
    public float mSize;
    public float maxHeight;
    public float power = 3.0f;
    public float scale = 1.0f;
    public float frq = 100f;
    public float percent = 0.5f;
    Vector2 v2SampleStart;
    public Gradient coloring;
    int vertCount;
    public int maxVerts = 16384;
    float max = -float.MaxValue;
    float min = float.MaxValue;
    public Material mat;


    void Start () {

        GameObject[] tiles = new GameObject[nTiles];
        Vector3 nextPosition = new Vector3(0, 0, 0);
        for (int i = 0; i < nTiles; i++) {
            GameObject obj = new GameObject();
            obj.transform.SetParent(this.transform);
            obj.AddComponent<MeshFilter>();
            obj.AddComponent<MeshRenderer>();
            obj.GetComponent<MeshRenderer>().material = material;
            Mesh mesh = new Mesh();
            obj.GetComponent<MeshFilter>().mesh = mesh;
            obj.transform.position = nextPosition;
            tiles[i] = obj;
        if ((i + 1)%(Mathf.Sqrt(nTiles)) == 0) {
                nextPosition.x = 0;
                nextPosition.z += mSize;
            } else {
                nextPosition.x += mSize;
            }

        }
        
        for (int i = 0; i < tiles.Length; i++) {
            Mesh mesh = tiles[i].GetComponent<MeshFilter>().mesh;
            mesh.vertices = diamondSquare(createTile(nDivisions));
            mesh.uv = calculateUvs(nDivisions);
            mesh.triangles = calculateTriangles(nDivisions);
            mesh.colors = calculateColors(mesh.vertices);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            
        }
        
    }



    private Vector3[] createTile(int sideVerts) {

        vertCount = (sideVerts + 1) * (sideVerts + 1);
        Vector3[] mVerts = new Vector3[vertCount];
        Vector2[] uvs = new Vector2[vertCount];
        float halfTerrain = mSize * 0.5f;
        float divisionSize = mSize / sideVerts;

        for (int i = 0; i <= sideVerts; i++) {

            for (int j = 0; j <= sideVerts; j++) {
                mVerts[i * (sideVerts + 1) + j] = new Vector3(-halfTerrain + j * divisionSize, 0, halfTerrain - i * divisionSize);
                uvs[i * (sideVerts + 1) + j] = new Vector2((float)i / sideVerts, (float)j / sideVerts);
            }
        }
        return mVerts;

    }

    private Vector2[] calculateUvs(int sideVerts) {

        Vector2[] uvs = new Vector2[vertCount];
        for (int i = 0; i <= sideVerts; i++) {

            for (int j = 0; j <= sideVerts; j++) {
               
                uvs[i * (sideVerts + 1) + j] = new Vector2((float)i / sideVerts, (float)j / sideVerts);
            }
        }
        return uvs;

    }

    private int[] calculateTriangles(int sideVerts) {

        int triOffset = 0;
        int[] tris = new int[sideVerts * sideVerts * 6];

        for (int i = 0; i <= sideVerts; i++) {

            for (int j = 0; j <= sideVerts; j++) {

                if (i < sideVerts && j < sideVerts) {
                    int topLeft = i * (sideVerts + 1) + j;
                    int botLeft = (i + 1) * (sideVerts + 1) + j;

                    tris[triOffset] = topLeft;
                    tris[triOffset + 1] = topLeft + 1;
                    tris[triOffset + 2] = botLeft + 1;

                    tris[triOffset + 3] = topLeft;
                    tris[triOffset + 4] = botLeft + 1;
                    tris[triOffset + 5] = botLeft;

                    triOffset = triOffset + 6;
                }
            }
        }
        return tris;
    }


    private Vector3[] diamondSquare(Vector3[] mVerts) {

        float mHeight = maxHeight;
        mVerts[0].y = Random.Range(-mHeight, mHeight);
        mVerts[nDivisions].y = Random.Range(-mHeight, mHeight);
        mVerts[mVerts.Length - 1].y = Random.Range(-mHeight, mHeight);
        mVerts[mVerts.Length - 1 - nDivisions].y = Random.Range(-mHeight, mHeight);

        int diamondSquareIterations = (int)Mathf.Log(nDivisions, 2);
        int nSquares = 1;
        int squareSize = nDivisions;

        for (int i = 0; i < diamondSquareIterations; i++) {

            int row = 0;
            for (int j = 0; j < nSquares; j++) {

                int col = 0;
                for (int k = 0; k < nSquares; k++) {

                    mVerts = diamondSquareCalculation(row, col, squareSize, mHeight,mVerts);
                    col = col + squareSize;
                }
                row = row + squareSize;
            }
            nSquares = nSquares * 2;
            squareSize = squareSize / 2;
            mHeight = mHeight * 0.5f;
        }
        for (int i = 0; i < mVerts.Length; i++) {
           
            if (mVerts[i].y > max) {
                max = mVerts[i].y;
            }
            if (mVerts[i].y < min) {
                min = mVerts[i].y;
            }
        }
       
        return mVerts;

    }

    private Color[] calculateColors(Vector3[] mVerts) {
        Color[] colors = new Color[mVerts.Length];
        for (int i = 0; i < mVerts.Length; i++) {
            colors[i] = coloring.Evaluate((((mVerts[i].y - (min)) * (1 - 0)) / (max - min) + 0) + 0.2f);
        }
        return colors;
    }


    private Vector3[] diamondSquareCalculation(int row, int col, int size, float offset, Vector3[] mVerts,TileNode node) {
        int halfSize = (int)(size * 0.5f);
        int topLeft = row * (nDivisions + 1) + col;
        int botLeft = (row + size)*(nDivisions+1)+col;
        int midpoint = (row + halfSize) * (nDivisions + 1) + (col + halfSize);


        if(node.leftNeighbour != null) {
            mVerts[midpoint].y = (node.topLeftVertex.y + mVerts[topLeft + size].y + node.botLeftVertex.y + mVerts[botLeft + size].y) * 0.25f + Random.Range(-offset, offset);
            mVerts[topLeft + halfSize].y = (node.topLeftVertex.y + mVerts[topLeft + size].y + mVerts[midpoint].y) / 3 + Random.Range(-offset, offset);
            mVerts[midpoint - halfSize].y = (node.topLeftVertex.y + mVerts[midpoint].y + node.botLeftVertex.y) / 3 + Random.Range(-offset, offset);
            mVerts[botLeft + halfSize].y = (node.botLeftVertex.y + mVerts[midpoint].y + mVerts[botLeft + size].y) / 3 + Random.Range(-offset, offset);
            mVerts[midpoint + halfSize].y = (mVerts[topLeft + size].y + mVerts[midpoint].y + mVerts[botLeft + size].y) / 3 + Random.Range(-offset, offset);
        }
        if(node.rightNeighbour != null) {
            mVerts[midpoint].y = (mVerts[topLeft].y + node.topRightVertex.y + mVerts[botLeft].y + node.botRightVertex.y) * 0.25f + Random.Range(-offset, offset);
            mVerts[topLeft + halfSize].y = (mVerts[topLeft].y + node.topRightVertex.y + mVerts[midpoint].y) / 3 + Random.Range(-offset, offset);
            mVerts[midpoint - halfSize].y = (mVerts[topLeft].y + mVerts[midpoint].y + mVerts[botLeft].y) / 3 + Random.Range(-offset, offset);
            mVerts[botLeft + halfSize].y = (mVerts[botLeft].y + mVerts[midpoint].y + node.botRightVertex.y) / 3 + Random.Range(-offset, offset);
            mVerts[midpoint + halfSize].y = (node.topRightVertex.y + mVerts[midpoint].y + node.botRightVertex.y) / 3 + Random.Range(-offset, offset);
        }
        if(node.topNeighbour != null) {
            mVerts[midpoint].y = (mVerts[topLeft].y + mVerts[topLeft + size].y + mVerts[botLeft].y + mVerts[botLeft + size].y) * 0.25f + Random.Range(-offset, offset);
            mVerts[topLeft + halfSize].y = (mVerts[topLeft].y + mVerts[topLeft + size].y + mVerts[midpoint].y) / 3 + Random.Range(-offset, offset);
            mVerts[midpoint - halfSize].y = (mVerts[topLeft].y + mVerts[midpoint].y + mVerts[botLeft].y) / 3 + Random.Range(-offset, offset);
            mVerts[botLeft + halfSize].y = (mVerts[botLeft].y + mVerts[midpoint].y + mVerts[botLeft + size].y) / 3 + Random.Range(-offset, offset);
            mVerts[midpoint + halfSize].y = (mVerts[topLeft + size].y + mVerts[midpoint].y + mVerts[botLeft + size].y) / 3 + Random.Range(-offset, offset);
        }
        mVerts[midpoint].y = (mVerts[topLeft].y + mVerts[topLeft + size].y + mVerts[botLeft].y + mVerts[botLeft + size].y) * 0.25f + Random.Range(-offset, offset);
        mVerts[topLeft + halfSize].y = (mVerts[topLeft].y + mVerts[topLeft + size].y + mVerts[midpoint].y) / 3 + Random.Range(-offset, offset);
        mVerts[midpoint - halfSize].y = (mVerts[topLeft].y + mVerts[midpoint].y + mVerts[botLeft].y) / 3 + Random.Range(-offset, offset);
        mVerts[botLeft + halfSize].y = (mVerts[botLeft].y + mVerts[midpoint].y + mVerts[botLeft + size].y) / 3 + Random.Range(-offset, offset);
        mVerts[midpoint + halfSize].y = (mVerts[topLeft + size].y + mVerts[midpoint].y + mVerts[botLeft + size].y) / 3 + Random.Range(-offset, offset);
        return mVerts;

    }

    private float[] MakeSomeNoise() {
        
        MeshFilter mf = GetComponent<MeshFilter>();
        Vector3[] vertices = mf.mesh.vertices;
        float[] noiseMap = new float[vertices.Length];
        float gain = 1.0f;
        for (int i = 0; i < vertices.Length; i++) {
            float xCoord = v2SampleStart.x + vertices[i].x * scale;
            float yCoord = v2SampleStart.y + vertices[i].z * scale;
            noiseMap[i] = (Mathf.PerlinNoise(xCoord*frq, yCoord * frq) ) * power;
            gain *= 2.0f;
        }
        return noiseMap;
    }
    

}
