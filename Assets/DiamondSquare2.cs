
using UnityEngine;

public class DiamondSquare2 : MonoBehaviour {


    public float maxHeight;
    public int iter = 1;


    public float[,] generarAlturas(float[,] mVerts) {

        int side = mVerts.GetLength(0);
        float mHeight = maxHeight;
        mVerts[0,0] = Random.Range(-mHeight, mHeight);
        mVerts[0,side-1] = Random.Range(-mHeight, mHeight);
        mVerts[side-1,0] = Random.Range(-mHeight, mHeight);
        mVerts[side-1,side-1] = Random.Range(-mHeight, mHeight);

        int step = side;
        while (step > 1) {

            for (int x = 0; x < side; x += step) {

                for (int y = 0; y < side; y += step) {

                    mVerts = diamondSquareCalculation(x-1 , y-1, step, mHeight, mVerts);
                }
            }
            step = step / 2;
            mHeight = mHeight * 0.5f;
        }




        return mVerts;

    }


    private float[,] diamondSquareCalculation(int x,int y, int step, float mHeight, float[,] mVerts) {


        if (x == -1) {
            x = 0;
            step--;
        }
        if (y == -1) {
            y = 0;
            if(x != 0) {
                step--;
            }

        }

        int halfSize = (int)(step * 0.5f);

        float topLeft = mVerts[x, y];
        float botLeft = mVerts[x, y + step];
        float topRight = mVerts[x + step, y];
        float botRight = mVerts[x + step, y + step];


        mVerts[x + halfSize, y + halfSize] = (topLeft + topRight + botLeft + botRight) * 0.25f + Random.Range(-mHeight, mHeight);
        float mid = mVerts[x + halfSize, y + halfSize];

        //Top
        mVerts[x + halfSize, y] = (mid + topLeft + topRight) / 3 + Random.Range(-mHeight, mHeight);
        //Bot
        mVerts[x + halfSize, y + step] = (mid + botLeft + botRight) / 3 + Random.Range(-mHeight, mHeight);
        //Right
        mVerts[x + step, y + halfSize] = (mid + topRight + botRight) / 3 + Random.Range(-mHeight, mHeight);
        //Left
        mVerts[x, y + halfSize] = (mid + topLeft + botLeft) / 3 + Random.Range(-mHeight, mHeight);

        return mVerts;

    }
}
