using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road
{
    int max_road_length = 60;
    public int r_num = 0;
    public int t_num = 0;
    public VECTOR2[] pos;
    public Vector3 center = Vector3.zero;

    public Road()
    {
        pos = new VECTOR2[max_road_length];
    }
}
