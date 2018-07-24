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
    float max = -float.MaxValue;
    float min = float.MaxValue;
    public Material mat;
    GameObject[] tiles;
    List<VertexData> orderedVerts;
    int ids = 0;
    public float amplitude;


    void Start () {

        orderedVerts = new List<VertexData>();
        tiles = new GameObject[nTiles];
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
                nextPosition.z -= mSize;
            } else {
                nextPosition.x += mSize;
            }

        }
        for (int i = 0; i < tiles.Length; i++) {
            createTile(nDivisions,i);   
        }
        orderedVerts.Sort();
        orderedVerts.Reverse();
        diamondSquare(orderedVerts.ToArray());
        orderedVerts.Sort(new VertxDataComparer());
        
       
        List<Vector3> listedVerts = new List<Vector3>();
        foreach(VertexData v in orderedVerts) {
            listedVerts.Add(v.vector);
        }
        

        for (int i = 0; i < tiles.Length; i++) {
            Mesh mesh = tiles[i].GetComponent<MeshFilter>().mesh;
            mesh.SetVertices(listedVerts.GetRange(0, (nDivisions+1)* (nDivisions+1)));
            listedVerts.RemoveRange(0, (nDivisions + 1) * (nDivisions + 1));
            mesh.uv = calculateUvs(nDivisions);
            mesh.triangles = calculateTriangles(nDivisions);
            mesh.colors = calculateColors(mesh.vertices);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            
        }
        
    }
    /*
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            for (int i = 0; i < tiles.Length; i++) {
                Mesh mesh = tiles[i].GetComponent<MeshFilter>().mesh;
                Vector3[] verts = MakeSomeNoise(mesh.vertices);
                for(int j = 0; j < verts.Length; j++) {
                    verts[i].y = Mathf.Lerp(verts[i].y, mesh.vertices[i].y, percent);
                }
                mesh.vertices = verts;
                mesh.colors = calculateColors(mesh.vertices);
                mesh.RecalculateBounds();
                mesh.RecalculateNormals();
            }
        }
    }
    */

    private Vector3[] createTile(int sideVerts,int tile) {

        vertCount = (sideVerts + 1) * (sideVerts + 1);
        Vector3[] mVerts = new Vector3[vertCount];
        Vector2[] uvs = new Vector2[vertCount];
        float halfTerrain = mSize * 0.5f;
        float divisionSize = mSize / sideVerts;
        int tileOffset = ((int)(tile / Mathf.Sqrt(nTiles))) * (sideVerts+1);
        for (int i = 0; i <= sideVerts; i++) {
            for (int j = 0; j <= sideVerts; j++) {
                mVerts[i * (sideVerts + 1) + j] = new Vector3(-halfTerrain + j * divisionSize, 0, halfTerrain - i * divisionSize);
                orderedVerts.Add(new VertexData(mVerts[i * (sideVerts + 1) + j], i * (sideVerts + 1) + j, tile,ids,i + tileOffset));
                ids++;
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


    private VertexData[] diamondSquare(VertexData[] mVerts) {

        int side = (int)Mathf.Sqrt(nTiles);
        float mHeight = maxHeight;
        mVerts[0].vector.y = Random.Range(-mHeight, mHeight);
        mVerts[nDivisions*side].vector.y = Random.Range(-mHeight, mHeight);
        mVerts[mVerts.Length - 1].vector.y = Random.Range(-mHeight, mHeight);
        mVerts[mVerts.Length - 1 - (nDivisions*side)].vector.y = Random.Range(-mHeight, mHeight);

        int diamondSquareIterations = (int)Mathf.Log(nDivisions*side, 2);
        int nSquares = 1;
        int squareSize = nDivisions*side;

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
           
            if (mVerts[i].vector.y > max) {
                max = mVerts[i].vector.y;
            }
            if (mVerts[i].vector.y < min) {
                min = mVerts[i].vector.y;
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


    private VertexData[] diamondSquareCalculation(int row, int col, int size, float offset, VertexData[] mVerts) {
        int size2 = (int)Mathf.Sqrt(nTiles);
        int halfSize = (int)(size * 0.5f);
        int topLeft = row * ((nDivisions + 1)* size2) + col;
        int botLeft = (row + size)*((nDivisions + 1) * size2)+col;
        int midpoint = (row + halfSize) * ((nDivisions + 1) * size2) + (col + halfSize);

        mVerts[midpoint].vector.y = (mVerts[topLeft].vector.y + mVerts[topLeft + size].vector.y + mVerts[botLeft].vector.y + mVerts[botLeft + size].vector.y) * 0.25f + Random.Range(-offset, offset);
        
        mVerts[topLeft + halfSize].vector.y = (mVerts[topLeft].vector.y + mVerts[topLeft + size].vector.y + mVerts[midpoint].vector.y) / 3 + Random.Range(-offset, offset);
       
        mVerts[midpoint - halfSize].vector.y = (mVerts[topLeft].vector.y + mVerts[midpoint].vector.y + mVerts[botLeft].vector.y) / 3 + Random.Range(-offset, offset);
       
        mVerts[botLeft + halfSize].vector.y = (mVerts[botLeft].vector.y + mVerts[midpoint].vector.y + mVerts[botLeft + size].vector.y) / 3 + Random.Range(-offset, offset);
        
        mVerts[midpoint + halfSize].vector.y = (mVerts[topLeft + size].vector.y + mVerts[midpoint].vector.y + mVerts[botLeft + size].vector.y) / 3 + Random.Range(-offset, offset);
        
        return mVerts;

    }

   
    public class VertxDataComparer : IComparer<VertexData> {
        public int Compare(VertexData x, VertexData y) {
            return x.id - y.id;
        }
    }


}
