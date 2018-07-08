using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class makeSphere : MonoBehaviour {


	// Use this for initialization
	void Start () {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        Planet p = new Planet();
        p.InitAsIcosohedron();
        p.Subdivide(2);

        mesh.vertices = p.m_Vertices.ToArray();
        int[] triangles = new int[p.m_Polygons.Count * 3];
        int cont = 0;
        for(int i = 0; i < p.m_Polygons.Count; i++) {
            for(int j = 0; j < 3; j++) {
                triangles[cont] = p.m_Polygons[i].m_Vertices[j];
                cont++;
            }
        }
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        

    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Mesh mesh = GetComponent<MeshFilter>().mesh;
            Vector3[] verts = new Vector3[mesh.vertices.Length];
            for (int i = 0; i < mesh.vertices.Length; i++) {
                verts[i] = mesh.vertices[i] + mesh.normals[i];
            }

            mesh.vertices = verts;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
        }
    }


}
