using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nodo {

    public int tile;
    public Vector3 position;

    public Nodo(Vector3 position,int tile) {
        this.tile = tile;
        this.position = position;
    }

}
