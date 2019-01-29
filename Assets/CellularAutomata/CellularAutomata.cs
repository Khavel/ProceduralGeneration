using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CellularAutomata : MonoBehaviour {


    private int[,] neighbours = new int[26,3] { {1,1,1} , {0,1,1} , {-1,1,1},
                                                {1,0,1} , {0,0,1} , {-1,0,1},
                                                {1,-1,1} , {0,-1,1} , {-1,-1,1},

                                                {1,1,0} , {0,1,0} , {-1,1,0},
                                                {1,0,0} ,            {-1,0,0},
                                                {1,-1,0} , {0,-1,0} , {-1,-1,0},

                                                {1,1,-1} , {0,1,-1} , {-1,1,-1},
                                                {1,0,-1} , {0,0,-1} , {-1,0,-1},
                                                {1,-1,-1} , {0,-1,-1}, { -1,-1,-1} };

    public int size = 50;
    public float tol = 0.5f;
    Dictionary<string, Cell> cells = new Dictionary<string, Cell>();
    Dictionary<string, Cell> existingCells = new Dictionary<string, Cell>();
    Dictionary<string, Cell> evaluated = new Dictionary<string, Cell>();

    // Use this for initialization
    void Start () {
        Vector3 v;

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        GameObject newCube;
        Cell aux;

        for (int x = 0; x < size; x++) {

            for(int y = 0; y < size; y++) {

                for(int z = 0;z < size; z++) {

                    v = new Vector3(x, y, z);
                    string id = v.ToString();

                    newCube = Instantiate(cube, this.transform);
                    newCube.transform.position = v;
                    /*
                    if (x == size -1 || y == size - 1 || z == size - 1 || x == 0 || y == 0 || z == 0) {

                        aux = new Cell(v, tol, newCube, true);
                        cells.Add(id, aux);

                    } else {
                    */
                        aux = new Cell(v, tol, newCube);
                        cells.Add(id, aux);
                   // }

                    if (!aux.exists) {
                        cells[id].cube.SetActive(false);
                    } else {
                        existingCells.Add(id, aux);
                    }
                    calcularVecinos(aux);


                }
            }
        }

        //combinarMeshes();

       

    }


    void calcularVecinos(Cell celda) {

        Vector3 pos = celda.position;
        for(int i = 0; i < neighbours.GetLength(0); i++) {

            Vector3 v = new Vector3(pos.x + neighbours[i, 0], pos.x + neighbours[i, 1], pos.x + neighbours[i, 2]);
            if(cells.ContainsKey(v.ToString())) {

                if(cells[v.ToString()].exists) {

                    celda.addVecino(i, cells[v.ToString()]);
                } else {

                    celda.deleteVecino(cells[v.ToString()]);
                }
                
            }

        }
    }


    void combinarMeshes() {

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        List<List<CombineInstance>> cubes = new List<List<CombineInstance>>();
        cubes.Add(new List<CombineInstance>());

        int listCont = 0;
        int vertCont = 0;
        foreach (Cell c in cells.Values) {
            if (c.exists) { 
                CombineInstance combine = new CombineInstance();
                combine.mesh = c.cube.GetComponent<MeshFilter>().mesh;
                combine.transform = c.cube.GetComponent<MeshFilter>().transform.localToWorldMatrix;
                vertCont += combine.mesh.vertexCount;

                if (vertCont >= 65000) {

                    listCont++;
                    cubes.Add(new List<CombineInstance>());
                    vertCont = 0;
                }
                cubes[listCont].Add(combine);
                c.cube.SetActive(false);
            }

        }

        Mesh[] combined = new Mesh[cubes.Count];
        foreach (List<CombineInstance> l in cubes) {
            Mesh cubesMesh = new Mesh();
            cubesMesh.CombineMeshes(l.ToArray());
            GameObject g = new GameObject();
            g.AddComponent<MeshFilter>().mesh = cubesMesh;
            g.AddComponent<MeshRenderer>().material = cube.GetComponent<MeshRenderer>().material;

        }
        //GetComponent<MeshFilter>().mesh = cubesMesh;
        //GetComponent<MeshRenderer>().material = cube.GetComponent<MeshRenderer>().material;
        
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown("space")) {

            print("Update");



            evaluateAutomata();
            
        }

    }

    private void evaluateAutomata() {

        Dictionary<string, Cell> dicAux = new Dictionary<string, Cell>();
        foreach (KeyValuePair<String,Cell> c in existingCells) {

            dicAux.Add(c.Key,c.Value);
        }

            foreach (Cell c in dicAux.Values) {
            
            foreach(Cell v in c.vecinos) {
                if(v != null) {

                    if (!evaluated.ContainsKey(v.id)) {

                        v.cube.SetActive(false);
                        if (v.getVecinos() <= 3 && existingCells.ContainsKey(v.id)) {

                            v.delete();
                            v.exists = false;
                            existingCells.Remove(c.id);
                            calcularVecinos(v);
                        } else if (v.getVecinos() > 3 && v.getVecinos() <= 12 && !existingCells.ContainsKey(v.id)) {
                            v.cube.SetActive(true);
                            v.exists = true;
                            existingCells.Add(v.id, v);
                            calcularVecinos(v);

                        } else if (v.getVecinos() > 12 && existingCells.ContainsKey(v.id)) {

                            v.delete();
                            v.exists = false;
                            existingCells.Remove(c.id);
                            calcularVecinos(v);
                        }
                        evaluated.Add(v.id, v);
                    }
                }

                
            }
            
        }
        //combinarMeshes();
        evaluated.Clear();
    }
    
}
