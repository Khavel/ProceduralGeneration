using UnityEngine;
using System.Collections;

public class MapDisplay : MonoBehaviour {


    public int side = 250;
    public GameObject g;
    public int lado = 4;
    private int nTiles = 0;
    public Gradient coloring;
    float min = float.MaxValue;
    float max = -100;
    public int tipo = 0;

    void Update() {

 
        if (Input.GetKey("space")) {

            int multZ = nTiles / lado;
            int multX = nTiles % lado;
            GameObject ng = Instantiate(g);
            ng.transform.position = new Vector3(ng.transform.position.x + (249*multX), ng.transform.position.y , ng.transform.position.z - (multZ * 249));
            generate(ng, side * multX, multZ * side);
            nTiles++;
        }

        
    }

    private void generate(GameObject gg,int xx, int yy) {

        float[,] alts = new float[side,side];

        switch (tipo) {
            case 0:

                Perlin2 p = GetComponent<Perlin2>();
                MeshGenerator.x = xx;
                MeshGenerator.y = yy;
                alts = p.generarAlturas(MeshGenerator.x, MeshGenerator.y);
                break;

            case 1:

                DiamondSquare2 dia = GetComponent<DiamondSquare2>();
                MeshGenerator.x = xx;
                MeshGenerator.y = yy;
                alts = dia.generarAlturas(alts);
                break;

        }

        MeshData m = MeshGenerator.GenerateTerrainMesh(alts);
        Mesh mesh = m.CreateMesh();
        mesh.colors = calculateColors(mesh.vertices);
        gg.GetComponent<MeshFilter>().mesh = mesh;

    }

    private Color[] calculateColors(Vector3[] mVerts) {


        for (int i = 0; i < mVerts.Length; i++) {

            if (mVerts[i].y > max) {
                max = mVerts[i].y;
            }
            if (mVerts[i].y < min) {
                min = mVerts[i].y;
            }
        }

        Color[] colors = new Color[mVerts.Length];
        for (int i = 0; i < mVerts.Length; i++) {
            
            colors[i] = coloring.Evaluate(((mVerts[i].y - (min))/ (max - min)));
            //print((((mVerts[i].y - (min)) * (1 - 0)) / (max - min) + 0) + 0.2f);
        }
        return colors;
    }

}