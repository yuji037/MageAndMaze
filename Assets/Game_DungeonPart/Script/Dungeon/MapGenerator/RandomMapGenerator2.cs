using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// RandomMapGenerator の改良版
public class RandomMapGenerator2 : MapGenerator {

    //struct VECTOR2
    //{
    //    public int x;
    //    public int y;
    //    public VECTOR2(int vx = 0, int vy = 0)
    //    {
    //        x = vx;
    //        y = vy;
    //    }
    //};
    GameObject mapChipParent;

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
    int room_create_attempt = 0;

    public override int GetMaxRoom()
    {
        return MAX_ROOM;
    }
    public int GetMaxRoadLength()
    {
        return MAX_ROAD_LENGTH;
    }
    public override RoomInfo[] GetRoomInfo()
    {
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
            RoomSort();
            RoadCreate();
            _success = CreateSuccessCheck();
        }
        return dung_2D;
    }

    /// <summary>
    /// 部屋がすべてつながっているかチェック
    /// </summary>
    /// <returns></returns>
    bool CreateSuccessCheck()
    {
        bool _connect = true;
        for (int i = 0; i < room_count; i++)
        {
            if (room_connected[i] == 0)
            {
                _connect = false;
            }
        }
        return _connect;
    }

    void InfoInit()
    {
        for (int y = 0; y < dungeon_height; ++y)
        {
            for (int x = 0; x < dungeon_width; ++x)
            {
                dung_2D[y,x] = -1;
            }
        }
        room_count = 0;
        for (int i = 0; i < road.Length; ++i)
        {
            road[i] = new Road();
        }
    }

    bool RoomOverlapCheck(int left, int top, int right, int bottom)
    {
        for (int i = 0; i < room_count; ++i)
        {
            if (right + 1 >= room[i].left && left - 1 <= room[i].right
                && bottom + 1 >= room[i].top && top - 1 <= room[i].bottom)  // 部屋領域かぶりなのでfalse
                return false;
        }
        return true;
    }

    void RoomCreate()
    {
        room_count = 0;
        room_create_attempt = 0;

        while (room_count < MAX_ROOM && room_create_attempt < 500)
        {
            // 試行回数カウント
            room_create_attempt++;

            // ４辺のランダム決定
            int x1, x2, y1, y2;
            x1 = 2 + Random.Range(0, dungeon_width - 4);
            x2 = 2 + Random.Range(0, dungeon_width - 4);
            y1 = 2 + Random.Range(0, dungeon_width - 4);
            y2 = 2 + Random.Range(0, dungeon_width - 4);

            if (x1 > x2)
            {
                int tmp = x1;
                x1 = x2;
                x2 = tmp;
            }
            if (y1 > y2)
            {
                int tmp = y1;
                y1 = y2;
                y2 = tmp;
            }
            // 部屋サイズはOKか
            bool okSize =
                (x1 <= x2 - ROOM_MIN_SIDE && x1 >= x2 - ROOM_MAX_SIDE
                && y1 <= y2 - ROOM_MIN_SIDE && y1 >= y2 - ROOM_MAX_SIDE);
            if (!okSize) continue;

            int left = x1;
            int top = y1;
            int right = x2;
            int bottom = y2;

            // 部屋領域は被っていないか
            bool okOverlap = RoomOverlapCheck(left, top, right, bottom);

            if (!okOverlap) continue;

            // 部屋作成成功
            room[room_count].left = left;
            room[room_count].top = top;
            room[room_count].right = right;
            room[room_count].bottom = bottom;
            room[room_count].center = new Vector3((float)(left + right) / 2, (float)(top + bottom) / 2, 0);

            room_count++;
        }

    }

    void RoomSort()
    {
        // 選択ソート
        int left_number = 0;

        while (left_number < room_count)
        {
            int min_number = 0;
            float min_centerY = 1000;
            for (int i = left_number; i < room_count; ++i)
            {
                if (min_centerY > room[i].center.y)
                {
                    min_number = i;
                    min_centerY = room[i].center.y;
                }
            }
            ROOM tmp = room[left_number];
            room[left_number] = room[min_number];
            room[min_number] = tmp;
            left_number++;
        }

        // 部屋情報の確定、書き込み
        for (int i = 0; i < room_count; ++i)
        {
            int left = room[i].left;
            int top = room[i].top;
            int right = room[i].right;
            int bottom = room[i].bottom;
            for (int y = top; y <= bottom; ++y)
            {
                for (int x = left; x <= right; ++x)
                {
                    dung_2D[y, x] = i;
                }
            }
        }
        room_info = new RoomInfo[room_count];
        for (int i = 0; i < room_info.Length; i++)
        {
            room_info[i] = new RoomInfo();
        }
    }

    int r_num = 0;
    int road_count = 0;
    int[] room_connected = new int[MAX_ROOM];

    int t_num = 0;
    int connected_room_count = 0;
    Vector3 r_centerPos;
    int road_create_finish_attempt = 0;
    VECTOR2 startDir = new VECTOR2(0, 0);
    VECTOR2 dir = new VECTOR2(0, 0);
    int[] road_connected = new int[MAX_ROAD];
    void RoadCreate()
    {
        connected_room_count = 0;
        for (int i = 0; i < MAX_ROOM; ++i)
        {
            room_connected[i] = 0;
        }
        for (int c = 0; c < MAX_ROAD; ++c)
        {
            for (int i = 0; i < MAX_ROAD_LENGTH; ++i)
            {
                road[c].pos[i].x = 0;
                road[c].pos[i].y = 0;
            }
        }
        // ４、最初はr_num＝0とする。0番を「すでにつながってる部屋群」に加えてスタート
        r_num = 0;
        //SetInfoConnectRoom(r_num);
        room_connected[r_num]++;
        connected_room_count++;
        bool road_create_finished = false;
        road_create_finish_attempt = 0;
        road_count = 0;
        float disX = 0;
        float disY = 0;
        while (!road_create_finished && road_create_finish_attempt < 1000)
        {
            road_create_finish_attempt++;
            int pre_t_num = t_num;
            t_num = -1;
            // ５、ｒ部屋から他の部屋の中心点への直線距離を計算
            // 	（このときすでにつながってるものは除外）
            float min_dis = 1024;
            for (int i = 0; i < room_count; ++i)
            {
                if (i == r_num) continue;
                if (room_connected[i] != 0) continue;
                if (i == pre_t_num) continue;
                disX = room[i].center.x - room[r_num].center.x;
                disY = room[i].center.y - room[r_num].center.y;
                float dis = disX * disX + disY * disY;
                if (dis < min_dis)
                {
                    min_dis = dis;
                    t_num = i;
                }
            }
            if (t_num == -1)
            {
                for (int i = 0; i < room_count; ++i)
                {
                    if (i == r_num) continue;
                    if (room_connected[i] > 1) continue;
                    //if (i == pre_t_num)continue;
                    disX = room[i].center.x - room[r_num].center.x;
                    disY = room[i].center.y - room[r_num].center.y;
                    float dis = disX * disX + disY * disY;
                    if (dis < min_dis)
                    {
                        min_dis = dis;
                        t_num = i;
                    }
                }
            }
            if (t_num == -1)
            {
                bool room_selected = false;
                while (!room_selected)
                {
                    r_num = Random.Range(0, room_count);
                    room_selected = (room_connected[r_num] != 0); // すでにつながってる部屋を選択
                    //if (connected_room_count <= 2) {
                    //	room_selected = true; // まだつながっていない部屋ばかりなら true
                    //}
                }
                continue;
            }
            //６、直線距離が一番近いナンバーの部屋をｔとする。
            //	ｔ部屋からｒ部屋への距離と、すべての道の中心点との距離を比較し、
            //	最小になった「部屋か道」を選びｒ点とする。
            for (int i = 0; i < road_count; ++i)
            {
                if (road_connected[i] > 1) continue;
                disX = road[i].center.x - room[t_num].center.x;
                disY = road[i].center.y - room[t_num].center.y;
                float dis = disX * disX + disY * disY;
                if (dis < min_dis)
                {
                    min_dis = dis;
                    r_num = MAX_ROOM + i;   // 道番号
                }
            }
            if (r_num < MAX_ROOM) r_centerPos = room[r_num].center;
            else r_centerPos = road[r_num - MAX_ROOM].center;

            // ７、ｒとｔの位置関係から引く道のタイプを決める
            startDir.x = 0;
            startDir.y = 0;
            disX = r_centerPos.x - room[t_num].center.x;
            disY = r_centerPos.y - room[t_num].center.y;
            if (disX * disX > disY * disY)
            {
                if (disX <= 0) startDir.x = -1;
                else startDir.x = 1;
            }
            else
            {
                if (disY <= 0) startDir.y = -1;
                else startDir.y = 1;
            }
            bool okCreate = false;
            int road_create_attempt = 0;

            while (!okCreate && road_create_attempt < 30)
            {
                road_create_attempt++;
                okCreate = RoadCreateAttempt();
                if (!okCreate)
                {
                    ChangeStartDirection(disX, disY);
                }
                else // 道作成成功
                {
                    if (t_num < room_count) room_info[t_num].SetGateWayPos(new Vector3(dig[0].x, 0, dig[0].y));
                    if (r_num < room_count) room_info[r_num].SetGateWayPos(new Vector3(dig[road_length - 1].x, 0, dig[road_length - 1].y));
                    if (room_connected[t_num] <= 1)
                    {
                        connected_room_count++;
                    }
                    room_connected[t_num]++;
                    if (r_num < MAX_ROOM) room_connected[r_num]++;
                    else road_connected[r_num - MAX_ROOM]++;
                    break;
                }
            }
            if (connected_room_count < (room_count + 2) && road_count < MAX_ROAD)
            {
                bool room_selected = false;
                while (!room_selected)
                {
                    r_num = Random.Range(0, room_count);
                    room_selected = (room_connected[r_num] != 0); // すでにつながってる部屋を選択
                                                                  //if (connected_room_count <= 2) {
                                                                  //	room_selected = true; // まだつながっていない部屋ばかりなら
                                                                  //}
                }
            }
            else road_create_finished = true;
        }
    }

    void ChangeStartDirection(float disX, float disY)
    {
        if (startDir.x != 0)
        {
            if (disY <= 0)
            {
                startDir.y = -1;
            }
            else
            {
                startDir.y = 1;
            }
            startDir.x = 0;
        }
        if (startDir.y != 0)
        {
            if (disX <= 0)
            {
                startDir.x = -1;
            }
            else
            {
                startDir.x = 1;
            }
            startDir.y = 0;
        }
    }

    void ChangeDirection(float disX, float disY)
    {
        if (dir.x != 0)
        {
            if (disY <= 0)
            {
                dir.y = -1;
            }
            else
            {
                dir.y = 1;
            }
            dir.x = 0;
        }
        //if (dir.y) {
        else
        {
            if (disX <= 0)
            {
                dir.x = -1;
            }
            else
            {
                dir.x = 1;
            }
            dir.y = 0;
        }
    }

    int road_length;

    VECTOR2 digPos = new VECTOR2(0, 0);
    VECTOR2 defaultDir = new VECTOR2(0, 0);
    VECTOR2[] dig = new VECTOR2[MAX_ROAD_LENGTH];

    bool RoadCreateAttempt()
    {
        digPos.x = 0;
        digPos.y = 0;
        for (int i = 0; i < MAX_ROAD_LENGTH; ++i)
        {
            dig[i].x = 0;
            dig[i].y = 0;
        }
        road_length = 0;
        bool collision = false;
        defaultDir.x = startDir.x;
        defaultDir.y = startDir.y;
        dir.x = startDir.x;
        dir.y = startDir.y;

        if (startDir.x == -1)
        {   // 左辺から
            digPos.y = (int)room[t_num].center.y + Random.Range(0, 3) - 1;
            digPos.x = room[t_num].left - 1;
            collision = (dung_2D[(int)digPos.y, (int)digPos.x] != -1);
            if (DigSideCheck())
            {
                collision = true;
            }
            while (!collision)
            {
                dig[road_length] = digPos;
                collision = DigRoad();
            }
        }
        else if (startDir.x == 1)
        {   // 右辺から
            digPos.y = (int)room[t_num].center.y + Random.Range(0, 3) - 1;
            digPos.x = room[t_num].right + 1;
            collision = (dung_2D[digPos.y, digPos.x] != -1);
            if (DigSideCheck())
            {
                collision = true;
            }
            while (!collision)
            {
                dig[road_length] = digPos;
                collision = DigRoad();
            }
        }
        else if (startDir.y == -1)
        {   // 上辺から
            digPos.x = (int)room[t_num].center.x + Random.Range(0, 3) - 1;
            digPos.y = room[t_num].top - 1;
            collision = (dung_2D[digPos.y, digPos.x] != -1);
            if (DigSideCheck())
            {
                collision = true;
            }
            while (!collision)
            {
                dig[road_length] = digPos;
                collision = DigRoad();
            }
        }
        else if (startDir.y == 1)
        {   // 下辺から
            digPos.x = (int)room[t_num].center.x + Random.Range(0, 3) - 1;
            digPos.y = room[t_num].bottom + 1;
            collision = (dung_2D[digPos.y, digPos.x] != -1);
            if (DigSideCheck())
            {
                collision = true;
            }
            while (!collision)
            {
                dig[road_length] = digPos;
                collision = DigRoad();
            }
        }

        if ((dung_2D[digPos.y, digPos.x] == r_num || dung_2D[digPos.y, digPos.x] >= MAX_ROOM) && road_length > 0)
        {
            // 道作成成功
            for (int i = 0; i < road_length; ++i)
            {
                dung_2D[dig[i].y, dig[i].x] = road_count + MAX_ROOM;
                road[road_count].pos[i].x = dig[i].x;
                road[road_count].pos[i].y = dig[i].y;
            }
            int center_num = (int)((float)(road_length - 1 + 0) / 2);
            road[road_count].center.x = road[road_count].pos[center_num].x;
            road[road_count].center.y = road[road_count].pos[center_num].y;
            road_count++;
            return true;
        }
        else return false;
    }

    bool DigRoad()
    {
        bool collision = false;

        float disX = r_centerPos.x - digPos.x;
        float disY = r_centerPos.y - digPos.y;
        if (dir.x == defaultDir.x && dir.y == defaultDir.y
            && (disX * disX < 1 || disY * disY < 1) && road_length > 0)
        {
            ChangeDirection(disX, disY);
        }
        //else if (road_length % 3 == 2 && rand()%5 == 0) {
        //	ChangeDirection(disX, disY);
        //}
        digPos.x += dir.x;
        digPos.y += dir.y;
        road_length++;
        if (dung_2D[digPos.y, digPos.x] != -1)
        {
            collision = true;
        }
        //collision = (dung_2D[digPos->y, digPos->x] != -1);
        if (digPos.x <= 1 || digPos.x >= dungeon_width - 2
            || digPos.y <= 1 || digPos.y >= dungeon_height - 2)
        {
            collision = true;
        }
        if (DigSideCheck())
        {
            collision = true;
        }
        //collision = (digPos->x <= 1 || digPos->x >= DUNGEON_WIDTH - 2
        //	|| digPos->y <= 1 || digPos->y >= DUNGEON_HEIGHT - 2);
        return collision;
    }

    bool DigSideCheck()
    {
        bool side_exist = false;
        if (dir.x != 0)
        {
            side_exist = (dung_2D[digPos.y - 1, digPos.x] != -1 || dung_2D[digPos.y + 1, digPos.x] != -1);
        }
        else
        {
            side_exist = (dung_2D[digPos.y, digPos.x - 1] != -1 || dung_2D[digPos.y, digPos.x + 1] != -1);
        }
        return side_exist;
    }
    //[SerializeField] GameObject[] mapChipPrefab;
    //void DungeonGenerate()
    //{
    //    for (int z = 0; z < DUNGEON_HEIGHT; ++z)
    //    {
    //        for (int x = 0; x < DUNGEON_WIDTH; ++x)
    //        {
    //            int mapChipType = 0;
    //            int value = dung_2D[z, x];
    //            if (value == -1) mapChipType = 0;
    //            else if (value < MAX_ROOM) mapChipType = 1;
    //            else if (value < MAX_ROOM + MAX_ROAD) mapChipType = 2;

    //            var obj = Instantiate(mapChipPrefab[mapChipType]);
    //            obj.transform.position = new Vector3(x, 0, z);
    //            dung_mapChips[z, x] = obj;
    //        }
    //    }
    //}

    //void DungeonDestroy()
    //{
    //    foreach(GameObject obj in dung_mapChips)
    //    {
    //        Destroy(obj);
    //    }
    //}
    // Use this for initialization
    void Start ()
    {
        //mapChipParent = GameObject.Find("MapChipParent");
        //DungeonInfoMake();
        //DungeonGenerate();
	}
	
	// Update is called once per frame
	void Update () {

        //if (Input.GetMouseButtonDown(0))
        //{
        //    DungeonDestroy();
        //    DungeonInfoMake();
        //    DungeonGenerate();
        //}
    }
}
