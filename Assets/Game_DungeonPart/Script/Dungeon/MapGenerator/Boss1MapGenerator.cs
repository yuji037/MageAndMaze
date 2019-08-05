﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Boss1MapGenerator : MapGenerator
{
    // -1 : 壁
    // 0 ～ MAX_ROOM -1 : 部屋（番号）
    // MAX_ROOM ～ : 道（番号）
    
    public const int MAX_ROOM = 10;
    const int ROOM_MIN_SIDE = 2;    //3*3の部屋を最小とする
    const int ROOM_MAX_SIDE = 9;    //10*10の部屋を最大とする
    const int MAX_ROAD = 12;
    const int MAX_ROAD_LENGTH = 60;

    struct ROOM
    {
        public int number;
        public int left;
        public int top;
        public int right;
        public int bottom;
        public Vector3 center;
        public ROOM(int num = 0, int le = 0, int to = 0
            , int ri = 0, int bo = 0)
        {
            number = num;
            left = le;
            top = to;
            right = ri;
            bottom = bo;
            center = Vector3.zero;
        }
    };

    ROOM[] room = new ROOM[MAX_ROOM];

    Road[] road = new Road[MAX_ROAD];

    int[,] dung_2D;

    int room_count = 0;

    public override int GetMaxRoom()
    {
        int max_room = 1;
        return max_room;
    }
    public int GetMaxRoadLength()
    {
        return MAX_ROAD_LENGTH;
    }
    public override RoomInfo[] GetRoomInfo()
    {
        room_info = new RoomInfo[1];
        room_info[0] = new RoomInfo();

        return room_info;
    }
    public override int[,] DungeonInfoMake()
    {
        bool _success = false;
        while (!_success)
        {
            dung_2D = new int[dungeon_height, dungeon_width];
            InfoInit();
            RoomCreate();
            //RoomSort();
            //RoadCreate();
            //_success = CreateSuccessCheck();
            _success = true;
        }
        return dung_2D;
    }

    void RoomCreate()
    {
        parent = GameObject.Find("GameObjectParent");
        dungeonInitializer = parent.GetComponentInChildren<DungeonInitializer>();

        int[,] map2D = new int[MapManager.DUNGEON_HEIGHT, MapManager.DUNGEON_WIDTH];

        TextAsset mapData = Resources.Load<TextAsset>("MapData/Boss1Map") as TextAsset;
        StringReader reader = new StringReader(mapData.text);
        // 最初の行は日本語の情報のみなので飛ばす
        reader.ReadLine();

        int Z = MapManager.DUNGEON_HEIGHT - 1;
        while ( reader.Peek() > -1 )
        {
            string line = reader.ReadLine();
            char[] delimiterChars = { ',' };
            string[] words = line.Split(delimiterChars);
            for ( int X = 0; X < words.Length; ++X )
            {
                switch ( words[X] )
                {
                    case "P":
                        dungeonInitializer.fixedPlayerPos = new Vector3(X, 0, Z);
                        map2D[Z, X] = 3;
                        break;
                    case "E":
                        dungeonInitializer.EnemySet(new Vector3(X, 0, Z), eEnemyType.BOSS1);
                        map2D[Z, X] = 3;
                        break;
                    default:
                        // 数値＝マップデータ
                        int.TryParse(words[X], out map2D[Z, X]);
                        break;
                }
            }
            Z--;
        }

        // 2 = 破壊不可能な壁、1 = 壁、0 = 部屋空間、3 = 道空間、E = 固定敵、P = プレイヤー
        for ( var z = 0; z < dungeon_height; z++)
        {
            for (var x = 0; x < dungeon_width; x++)
            {
                switch (map2D[z,x])
                {
                    case 2:
                        dung_2D[z, x] = -2;
                        break;
                    case 1:
                        dung_2D[z, x] = -1;
                        break;
                    case 0:
                        dung_2D[z, x] = 0;
                        break;
                    case 3:
                        dung_2D[z, x] = 11;
                        break;
                }
            }
        }
    }

    void InfoInit()
    {
        for (int y = 0; y < dungeon_height; ++y)
        {
            for (int x = 0; x < dungeon_width; ++x)
            {
                dung_2D[y, x] = -1;
            }
        }
        room_count = 0;
        for (int i = 0; i < road.Length; ++i)
        {
            road[i] = new Road();
        }
    }

}
