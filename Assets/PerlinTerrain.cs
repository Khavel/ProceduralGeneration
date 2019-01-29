using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class PerlinTerrain : MonoBehaviour {

    GameObject g;
    public float power = 3.0f;
    public float scale = 1.0f;
    private Vector2 v2SampleStart = new Vector2(0f, 0f);

    void Start() {
        v2SampleStart = new Vector2(Random.Range(0.0f, 100.0f), Random.Range(0.0f, 100.0f));
        g = GameObject.CreatePrimitive(PrimitiveType.Plane);
        Mesh mesh = g.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        for (int i=0;i<vertices.Length;i++) {

            float xCoord = v2SampleStart.x + vertices[i].x * scale;
            float yCoord = v2SampleStart.y + vertices[i].z * scale;
            vertices[i].y = (Mathf.PerlinNoise(xCoord, yCoord) - 0.5f) * power;
            print(vertices[i]);
        }
        mesh.vertices = vertices;
    }

    void GenerateTile() {
        
    }

    private Texture2D BuildTexture(float[,] heightMap) {
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);

        Color[] colorMap = new Color[tileDepth * tileWidth];
        for (int zIndex = 0; zIndex < tileDepth; zIndex++) {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++) {
                // transform the 2D map index is an Array index
                int colorIndex = zIndex * tileWidth + xIndex;
                float height = heightMap[zIndex, xIndex];
                // assign as color a shade of grey proportional to the height value
                colorMap[colorIndex] = Color.Lerp(Color.black, Color.white, height);
            }
        }

        // create a new texture and set its pixel colors
        Texture2D tileTexture = new Texture2D(tileWidth, tileDepth);
        tileTexture.wrapMode = TextureWrapMode.Clamp;
        tileTexture.SetPixels(colorMap);
        tileTexture.Apply();

        return tileTexture;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
