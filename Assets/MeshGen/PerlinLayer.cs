using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PerlinLayer : MonoBehaviour {

    public int ancho = 250;
    public int alto = 250;
    public int prof = 250;
    public float escala = 2;
    public float offX = 100;
    public float offY = 100;
    public float offZ = 100;
    public float threshold = 0.5f;
    public Material mat;
    private Vector3 pos = new Vector3(0, 0, 0);
    GameObject g;
    Mesh m;

    private void Start() {

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        m = cube.GetComponent<MeshFilter>().mesh;

    }



    private void Update() {

        //if (transform.position != pos) {

            Destroy(g);
            pos = transform.position;
            float centerX = transform.position.x - (ancho / 2);
            float centerY = transform.position.y - (prof / 2);
            float centerZ = transform.position.z - (ancho / 2);

            float posx = transform.position.x + (ancho / 2);
            float posy = transform.position.y + (prof / 2);
            float posz = transform.position.z + (ancho / 2);

            for (float x = centerX; x < posx; x++) {

                for (float y = centerY; y < posy; y++) {

                    for (float z = centerZ; z < posz; z++) {

                        float val = perlin3d(x, z, y);
                        if (val > threshold) {

                            Graphics.DrawMesh(m, new Vector3(x, y, z), transform.rotation, mat, 0);

                        }

                    }

                }
            }

        //}
        
    }



    void juntar(GameObject gam) {
        MeshFilter[] meshFilters = gam.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length) {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }
        gam.transform.GetComponent<MeshFilter>().mesh = new Mesh();
        gam.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        gam.transform.gameObject.SetActive(true);
        
    }


    public float perlin3d(float x, float y, float z) {

        x = x / ancho * escala + offX;
        y = y / alto * escala + offY;
        z = z / alto * escala + offZ;

        float AB = Mathf.PerlinNoise(x, y);
        float BC = Mathf.PerlinNoise(y, z);
        float AC = Mathf.PerlinNoise(x, z);

        float BA = Mathf.PerlinNoise(y, x);
        float CB = Mathf.PerlinNoise(z, y);
        float CA = Mathf.PerlinNoise(z, x);

        return (AB + BC + AC + BA + CB + CA) / 6;
    }
}
