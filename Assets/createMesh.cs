using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class createMesh : MonoBehaviour {

	// Use this for initialization

    public int side;
    public float step = 0.5f;
    Vector3[] vertices;
    public float amplitude = 3.0f;
    public float scale = 1.0f;
    public Vector2 v2SampleStart = new Vector2(0f, 0f);



    void Start() {
        MeshFilter mf = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mf.mesh = mesh;
        vertices = new Vector3[side*side];
        float x = 0;
        float y = 0;
        float z = 0;
        for (int i = 0; i < side; i++) {
            for (int j = 0; j < side; j++) {
                vertices[(i * side)+j] = new Vector3(x,z,y);
                y = y + step;
            }
            x = x + step;
            y = 0;
        }

        mesh.vertices = vertices;

        int[] tri = new int[side * side * 6];
        int triCont = 0;
        for (int i = 0; i < side; i++) {
            if(i != side - 1) { 
                for (int j = 0; j < side; j++) {
                    if (j % side != 0) { 
                        //Primer triangulo
                        tri[triCont] = (i * side) + j - 1;
                        triCont++;
                        tri[triCont] = (i * side) + j;
                        triCont++;
                        tri[triCont] = (i * side) + j + side - 1;
                        triCont++;
                        //Segundo triangulo
                        tri[triCont] = (i * side) + j;
                        triCont++;
                        tri[triCont] = (i * side) + j + side;
                        triCont++;
                        tri[triCont] = (i * side) + j + side -1;
                        triCont++;
                    }
                }

            }

        }

        mesh.triangles = tri;
 
    }
    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            v2SampleStart = new Vector2(Random.Range(0.0f, 100.0f), Random.Range(0.0f, 100.0f));
            MakeSomeNoise();
        }
    }

    void MakeSomeNoise() {
        MeshFilter mf = GetComponent<MeshFilter>();
        Vector3[] vertices = mf.mesh.vertices;
        float[] octave1, octave2, octave3;
        octave1 = octave2 = octave3 = new float[vertices.Length];
        for (int i = 0; i < vertices.Length; i++) {
            float xCoord = v2SampleStart.x + vertices[i].x * scale;
            float yCoord = v2SampleStart.y + vertices[i].z * scale;
            vertices[i].y = (Mathf.PerlinNoise(xCoord, yCoord)) * amplitude;
        }
        //Octave 1
        for (int i = 0; i < vertices.Length; i++) {
            float xCoord = v2SampleStart.x + vertices[i].x * scale;
            float yCoord = v2SampleStart.y + vertices[i].z * scale;
            octave1[i] = (Mathf.PerlinNoise(xCoord*0.5f, yCoord*0.5f)) * amplitude;
        }
        //Octave 2
        for (int i = 0; i < vertices.Length; i++) {
            float xCoord = v2SampleStart.x + vertices[i].x * scale;
            float yCoord = v2SampleStart.y + vertices[i].z * scale;
            octave2[i] = (Mathf.PerlinNoise(xCoord * 0.3f, yCoord * 0.3f)) * amplitude;
        }
        //Octave 3
        for (int i = 0; i < vertices.Length; i++) {
            float xCoord = v2SampleStart.x + vertices[i].x * scale;
            float yCoord = v2SampleStart.y + vertices[i].z * scale;
            octave3[i] = (Mathf.PerlinNoise(xCoord * 0.1f, yCoord * 0.1f)) * amplitude;
        }

        for (int i = 0; i < vertices.Length; i++) {
            
            vertices[i].y = Mathf.Lerp(octave1[i], vertices[i].y, 0.5f);
            //vertices[i].y = Mathf.Lerp(octave2[i], vertices[i].y, 0.5f);
            //vertices[i].y = Mathf.Lerp(octave3[i], vertices[i].y, 0.5f);
        }

        mf.mesh.vertices = vertices;
        mf.mesh.RecalculateBounds();
        mf.mesh.RecalculateNormals();
    }
    /*
    private void OnDrawGizmos() {
        if (vertices == null) {
            return;
        }
        Gizmos.color = Color.black;
        for (int i = 0; i < vertices.Length; i++) {
            Gizmos.DrawSphere(transform.TransformPoint(vertices[i]), 0.1f);
        }
    }
    */
}
