using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    public int dungeon_height;
    public int dungeon_width;
    protected RoomInfo[] room_info;

    protected GameObject parent;
    protected DungeonInitializer dungeonInitializer;

    public virtual int[,] DungeonInfoMake()
    {
        int[,] dung_2D = new int[30,30];

        return dung_2D;
    }

    public virtual int GetMaxRoom()
    {
        return 1;
    }

    public virtual RoomInfo[] GetRoomInfo()
    {
        return room_info;
    }
}
