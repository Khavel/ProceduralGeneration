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
    Color[] colors;
    private Vector3[] mVerts;
    int vertCount;
    float max = -float.MaxValue;
    float min = float.MaxValue;

    List<GameObject> meshes; //This list contains all the tiles of the map as different gameobjects.
    private Vector3[] megaArray; //This array will contain all the vertices from all the tiles, ordered sequentially.
    private SortedDictionary<Nodo,int[]> sortedVertices;

    void Start () {
        sortedVertices = new SortedDictionary<Nodo, int[]>(new SortVector3());
        megaArray = new Vector3[((nDivisions + 1) * (nDivisions + 1))*nTiles];
        meshes = new List<GameObject>();
        v2SampleStart = new Vector2(Random.Range(0.0f, 100.0f), Random.Range(0.0f, 100.0f));
        createTerrain();
        Vector3 nextPosition = new Vector3(0, 0, 0);
        for(int i = 0; i < meshes.Count; i++) {
            diamondSquare(meshes[i].GetComponent<MeshFilter>().mesh);
            meshes[i].transform.position = nextPosition;
            if((i + 1)%(Mathf.Sqrt(nTiles)) == 0) {
                nextPosition.x = 0;
                nextPosition.z += mSize;
            } else {
                nextPosition.x += mSize;
            }

        }
 
	}

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            
            float[] noiseAux = MakeSomeNoise();
            for (int i = 0; i < mVerts.Length; i++) {
                mVerts[i].y = Mathf.Lerp(noiseAux[i], mVerts[i].y,percent);
            }
            Mesh mesh = GetComponent<MeshFilter>().mesh;
            diamondSquare(mesh);
            mesh.vertices = mVerts;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
        }
    }


    private void createTerrain() {

        for(int x = 0; x < nTiles; x++) {
            GameObject obj = new GameObject();
            obj.transform.SetParent(this.transform);
            obj.AddComponent<MeshFilter>();
            obj.AddComponent<MeshRenderer>();
            obj.GetComponent<MeshRenderer>().material = material;
            
            vertCount = (nDivisions + 1) * (nDivisions + 1);
            mVerts = new Vector3[vertCount];
            Vector2[] uvs = new Vector2[vertCount];
            int[] tris = new int[nDivisions * nDivisions * 6];
            float halfTerrain = mSize * 0.5f;
            float divisionSize = mSize / nDivisions;

            Mesh mesh = new Mesh();
            obj.GetComponent<MeshFilter>().mesh = mesh;

            int triOffset = 0;

            for(int i = 0; i <= nDivisions; i++) {

                for(int j =0;j<= nDivisions; j++) {
                    mVerts[i * (nDivisions + 1) + j] = new Vector3(-halfTerrain + j * divisionSize, 0, halfTerrain - i * divisionSize);
                    uvs[i * (nDivisions + 1) + j] = new Vector2((float)i / nDivisions, (float)j / nDivisions);
                    sortedVertices.Add(new Nodo(mVerts[i * (nDivisions + 1) + j], x),new int[] {x,(i * (nDivisions + 1) + j) });
                    if (i<nDivisions && j < nDivisions) {
                        int topLeft = i * (nDivisions + 1) + j;
                        int botLeft = (i+1) * (nDivisions + 1) + j;

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
            mesh.vertices = mVerts;
            mesh.uv = uvs;
            mesh.triangles = tris;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            meshes.Add(obj);

        }
    }


    private void diamondSquare(Mesh mesh) {

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

                    diamondSquareCalculation(row, col, squareSize, mHeight);
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
        colors = new Color[mVerts.Length];
        for (int i = 0; i < mVerts.Length; i++) {
            colors[i] = coloring.Evaluate((((mVerts[i].y - (min)) * (1 - 0)) / (max - min) + 0)+0.2f); 
        }

        mesh.vertices = mVerts;
        mesh.colors = colors;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        
    }


    private void diamondSquareCalculation(int row, int col, int size, float offset) {
        int halfSize = (int)(size * 0.5f);
        int topLeft = row * (nDivisions + 1) + col;
        int botLeft = (row + size)*(nDivisions+1)+col;
        int midpoint = (row + halfSize) * (nDivisions + 1) + (col + halfSize);

        mVerts[midpoint].y = (mVerts[topLeft].y+mVerts[topLeft+size].y+mVerts[botLeft].y+mVerts[botLeft+size].y)*0.25f + Random.Range(-offset,offset);

        mVerts[topLeft + halfSize].y = (mVerts[topLeft].y + mVerts[topLeft + size].y + mVerts[midpoint].y) / 3 + Random.Range(-offset,offset);
        mVerts[midpoint - halfSize].y = (mVerts[topLeft].y + mVerts[midpoint].y + mVerts[botLeft].y) / 3 + Random.Range(-offset, offset);
        mVerts[botLeft + halfSize].y = (mVerts[botLeft].y + mVerts[midpoint].y + mVerts[botLeft+size].y) / 3 + Random.Range(-offset, offset);
        mVerts[midpoint + halfSize].y = (mVerts[topLeft+size].y + mVerts[midpoint].y + mVerts[botLeft+size].y) / 3 + Random.Range(-offset, offset);

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
public class SortVector3 : IComparer<Nodo> {
    public int Compare(Nodo x, Nodo y) {
        if(x.position.z > y.position.z) {
            return 1;
        }
        if(x.position.z < y.position.z) {
            return -1;
        }
        if(x.position.z == y.position.z) {
            if (x.position.x > y.position.x) {
                return 1;
            }
            if (x.position.x < y.position.x) {
                return -1;
            }
        }

        return 0;

    }
}
