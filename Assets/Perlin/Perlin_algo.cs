using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perlin_algo : MonoBehaviour {


    public int n;
    public float umbral = 0.5f;
    public float offset = 0.23f;
    public float radio = 1;
	// Use this for initialization
	void Start () {
        int centro = n / 2;
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        for (int i = -centro; i < centro; i++) {

            for(int j = -centro; j < centro; j++) {

                for(int k = -centro; k < centro; k++) {

                    if(perlinAlgo(i+offset,j + offset, k + offset) > umbral) {
                        if (estaDentro(i,j,k)) {
                            Instantiate(cube, new Vector3(i, j, k), this.transform.rotation);
                        }
                        
                    }
                }
            }

        }
		
	}



    private bool estaDentro(int x, int y, int z) {

        
        return (((Mathf.Abs(x) - 10) ^ 2) + ((Mathf.Abs(y) - 10) ^ 2) + ((Mathf.Abs(z) - 10) ^ 2)) < (radio * radio);
    }
	



    private float perlinAlgo(float x, float y, float z) {

        float xy = Mathf.PerlinNoise(x, y);
        float xz = Mathf.PerlinNoise(x, z);
        float yz = Mathf.PerlinNoise(y, z);

        float yx = Mathf.PerlinNoise(y, x);
        float zx = Mathf.PerlinNoise(z, x);
        float zy = Mathf.PerlinNoise(z, y);

        //print(x + " " + y + " " + z);
        return (xy + xz + yz + yx + zx + zy) / 6f;
    }
	// Update is called once per frame
	void Update () {
		
	}
}
