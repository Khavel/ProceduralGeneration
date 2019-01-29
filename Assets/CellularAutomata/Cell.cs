using UnityEngine;
using UnityEditor;

public class Cell {

    public string id;
    public Vector3 position;
    public Cell[] vecinos;
    public bool exists;
    public GameObject cube;


    public Cell(Vector3 position, float tol, GameObject cube, bool existance =false) {

        this.position = position;
        vecinos = new Cell[26];
        id = position.ToString();
        this.cube = cube;
        if(Random.value > tol) {

            exists = true;
        } else {

            exists = false;
        }
        if (existance) {

            exists = true;
        }
    }

    public void addVecino(int lado, Cell cell) {

        vecinos[lado] = cell;
    }

    public void deleteVecino(Cell cell) {

        for (int i = 0; i < vecinos.Length; i++) {
            if(vecinos[i] != null) {

                if (vecinos[i].Equals(cell)) {

                    vecinos[i] = null;
                    return;
                }
            }
            
        }
    }

    public int getVecinos() {
        int cont = 0;
        foreach(Cell c in vecinos) {

            if (c != null) {

                cont++;
            }
        }

        return cont;

    }

    public void delete() {

        foreach(Cell c in vecinos) {
            if(c != null) {
               c.deleteVecino(this);
            }
           
        }
    }




    
}