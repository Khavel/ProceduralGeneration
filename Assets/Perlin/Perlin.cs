
using UnityEngine;

[RequireComponent(typeof(Terrain))]
public class Perlin : MonoBehaviour {


    public float p = 10;
    public int ancho = 250;
    public int alto = 250;
    public float escala = 2;
    public float offX = 100;
    public float offY = 100;
    public float lacunarity = 2;

    public int oct = 4;
    public float pers = 1;


    void Start() {

       
    }
    /*
    // Use this for initialization
    void Update () {

        Terrain t = GetComponent<Terrain>();
        t.terrainData = GenerarDatosTerreno(t.terrainData);
    }
    */
    private TerrainData GenerarDatosTerreno(TerrainData td) {
        td.heightmapResolution = ancho + 1;
        td.size = new Vector3(ancho, p, alto);
        td.SetHeights(0, 0, generarAlturas());

        return td;
    }

    public float[,] generarAlturas() {

        float[,] alturas = new float[ancho, alto];

        for(int x = 0;x<ancho; x++) {

            for(int y = 0; y < alto; y++) {

                alturas[x, y] = OctavePerlin(x, y,oct,pers) * p;
            }
        }
        return alturas;
    }

    private float calcularPerlin(int x, int y,int oct, float pers) {

        float xOffset = (float)x / ancho * escala + offX;
        float yOffset = (float)y / alto * escala + offY;

        return Mathf.PerlinNoise(xOffset, yOffset);
    }

    public float OctavePerlin(int x, int y, int octaves, float persistence) {
        float total = 0;
        float frequency = 1;
        float amplitude = 1;
        float xOffset = (float)x / ancho * escala + offX;
        float yOffset = (float)y / alto * escala + offY;
        float maxValue = 0;  // Used for normalizing result to 0.0 - 1.0
        for (int i = 0; i < octaves; i++) {
            total += Mathf.PerlinNoise(xOffset * frequency, yOffset * frequency) * amplitude;

            maxValue += amplitude;

            amplitude *= persistence;
            frequency *= lacunarity;
        }

        //print(total / maxValue);
        return total / maxValue;
    }








}
