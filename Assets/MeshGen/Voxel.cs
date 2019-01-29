using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel : MonoBehaviour {

    public static Vector3 tamano = new Vector3(1, 1, 1);
    public Mesh mesh;
    public Vector3 position;

	public Voxel(Vector3 position,Mesh mesh) {

        this.position = position;
        this.mesh = mesh;

    }
}
