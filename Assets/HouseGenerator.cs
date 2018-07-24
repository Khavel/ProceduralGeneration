using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseGenerator : MonoBehaviour {

    public float side = 1;

	// Use this for initialization
	void Start () {
        GameObject[] walls = new GameObject[6];
        for(int i = 0; i < walls.Length; i++) {
            walls[i] = GeneratePlane();
            walls[i].transform.SetParent(this.transform);
        }

        walls[1].transform.Rotate(Vector3.up,90);
        walls[1].transform.Rotate(Vector3.forward, 90);

        walls[2].transform.Rotate(Vector3.up, -90);
        walls[2].transform.Rotate(Vector3.right, -90);

        walls[3].transform.Translate(side,0,0);
        walls[3].transform.Rotate(Vector3.forward, 90);

        walls[4].transform.Translate(0, 0, side);
        walls[4].transform.Rotate(Vector3.left, -90);

        walls[5].transform.Translate(0, side, 0);
    }

    private GameObject GeneratePlane() {
        Vector3[] vertices = new Vector3[4];
        int[] triangles = new int[6];
        Vector2[] uv = new Vector2[4];

        vertices[0] = new Vector3(0, 0, 0);
        uv[0] = new Vector2(0, 0);
        vertices[1] = new Vector3(0, 0, side);
        uv[0] = new Vector2(1, 0);
        vertices[2] = new Vector3(side, 0, side);
        uv[0] = new Vector2(1, 1);
        vertices[3] = new Vector3(side, 0, 0);
        uv[0] = new Vector2(0, 1);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 3;

        GameObject g = new GameObject();
        MeshFilter m = g.AddComponent<MeshFilter>();
        g.AddComponent<MeshRenderer>();

        Mesh mesh = m.mesh;
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        m.mesh = mesh;

        return g;
    }
}
