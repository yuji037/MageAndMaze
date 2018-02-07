using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Calc {

    public static bool init = false;
    public static float[] sin_f { get; private set; }
    public static float[] cos_f { get; private set; }
    
    // Use this for initialization
    public static void Init () {
        if ( init ) return;
        sin_f = new float[360];
        cos_f = new float[360];

        for (int i = 0; i < 360; i++)
        {
            sin_f[i] = Mathf.Sin(2 * Mathf.PI * i / 360);
            cos_f[i] = Mathf.Cos(2 * Mathf.PI * i / 360);
        }
        init = true;
	}

    public static Vector3 RandomDir()
    {
        Vector3 dir = new Vector3(0, 0, -1);

        return RotateY(dir, 45 * Random.Range(0, 8));
    }

    public static Vector3 RotateY(Vector3 vec, int angle)
    {
        Init();
        while (angle < 0)
        {
            angle += 360;
        }
        while (angle >= 360)
        {
            angle -= 360;
        }
        // Y軸周りの回転
        // 
        // x'		(cosA	0       sinA    0)(x)
        // y'	=	(0  	1   	0	    0)(y)
        // z'		(-sinA	0		cosA    0)(z)
        // 1		(0		0		0	    1)(1)
        // 
        float _x = cos_f[angle] * vec.x + sin_f[angle] * vec.z;
        float _y = vec.y;
        float _z = -sin_f[angle] * vec.x + cos_f[angle] * vec.z;

        return new Vector3(_x, _y, _z);
    }

    public static Vector3 RotateZ(Vector3 vec, int angle)
    {
        Init();
        while (angle < 0)
        {
            angle += 360;
        }
        while (angle >= 360)
        {
            angle -= 360;
        }
        // Z軸周りの回転
        // 
        // x'		(cosA	-sinA	0	0)(x)
        // y'	=	(sinA	cosA	0	0)(y)
        // z'		(0		0		1	0)(z)
        // 1		(0		0		0	1)(1)
        // 
        float _x = cos_f[angle] * vec.x - sin_f[angle] * vec.y;
        float _y = sin_f[angle] * vec.x + cos_f[angle] * vec.y;
        float _z = vec.z;

        return new Vector3(_x, _y, _z);
    }
}
