using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMapGenerator : MonoBehaviour {

    const int DUNGEON_WIDTH = 30;
    const int DUNGEON_HEIGHT = 30;

    //public int[,] dungeonMap = new int[DUNGEON_HEIGHT, DUNGEON_WIDTH];

    //DungeonMake関係
    const int LEFT = 0;
    const int TOP = 1;
    const int RIGHT = 2;
    const int BOTTOM = 3;

    const int WALL = 0;
    const int ROOM_SPACE = 1;
    const int ROAD = 2;
    const int ROAD_S = 3;
    const int ROAD_G = 4;

    const int ROOM_MIN_SIDE = 2;    //3*3の部屋を最小とする
    const int ROOM_MAX_SIDE = 9;    //10*10の部屋を最大とする
    const int MAX_ROOM_COUNT = 10;  //部屋の最大数
    const int MAX_ROAD_COUNT = 20;
    const int MAX_ROAD_LENGTH = 100;
    const int MAX_ATTEMPT = 30;
    int road_cre_attempt = 0;

    int[,] room = new int[MAX_ROOM_COUNT, 4]; //0:左上のx座標
    int room_count = 0;
    int[,] room_connect = new int[MAX_ROAD_COUNT, 2];
    int[,,] road_info = new int[MAX_ROAD_COUNT, MAX_ROAD_LENGTH, 2];
    int road_count = 0;
    int xr = 0, yr = 0, xt = 0, yt = 0, midX = 0, midY = 0;

    int[,] dungeon2D = new int[DUNGEON_HEIGHT, DUNGEON_WIDTH]; // 現状こいつはダミー
    int[,] dung_2D = new int[DUNGEON_HEIGHT, DUNGEON_WIDTH]; // データこっちに入ってる

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    bool Room_Overlap_Check(int rect_num)
    {
        int left = room[rect_num, 0];
        int top = room[rect_num, 1];
        int right = room[rect_num, 2];
        int bottom = room[rect_num, 3];
        bool ok = false;

        for (int i = 0; i < rect_num; i++)
        {
            ok = false;
            int i_left = room[i, 0];
            int i_top = room[i, 1];
            int i_right = room[i, 2];
            int i_bottom = room[i, 3];

            if (i_left <= left)
            {
                if (i_right + 1 < left)
                {
                    ok = true;
                }
                else if (bottom + 1 < i_top)
                {
                    ok = true;
                }
                else if (i_bottom + 1 < top)
                {
                    ok = true;
                }
            }
            else if (left <= i_left)
            {
                if (right + 1 < i_left)
                {
                    ok = true;
                }
                else if (bottom + 1 < i_top)
                {
                    ok = true;
                }
                else if (i_bottom + 1 < top)
                {
                    ok = true;
                }
            }
            if (false == ok)
            {
                return false;
            }
        }
        return ok;
    }

    bool Road_Create(int r_dir, int t_dir)
    {
        bool road_create_finished = false;
        int length = 0;
        if (r_dir == TOP && t_dir == BOTTOM)
        {
            for (int y = yt; y <= yr; y++)
            {
                if (y == yt)
                {
                    road_info[road_count, length, 0] = xt;
                    road_info[road_count, length, 1] = y;
                }
                if (midY != 0 && y != yt && y != yr)
                {
                    if (y < midY && y > yt)
                    {
                        road_info[road_count, length, 0] = xt;
                        road_info[road_count, length, 1] = y;
                    }
                    if (y == midY)
                    {
                        int minX = 0;
                        int maxX = 0;
                        if (xt <= xr)
                        {
                            minX = xt;
                            maxX = xr;
                        }
                        else
                        {
                            minX = xr;
                            maxX = xt;
                        }
                        for (int x = minX; x <= maxX; x++)
                        {
                            road_info[road_count, length, 0] = x;
                            road_info[road_count, length, 1] = midY;
                            length++;
                        }
                        length--;
                    }
                    if (y > midY && y < yr)
                    {
                        road_info[road_count, length, 0] = xr;
                        road_info[road_count, length, 1] = y;
                    }
                }
                if (midY == 0 && y != yt && y != yr)
                {
                    road_info[road_count, length, 0] = xr;
                    road_info[road_count, length, 1] = y;
                }
                if (y == yr)
                {
                    road_info[road_count, length, 0] = xr;
                    road_info[road_count, length, 1] = y;
                }
                length++;
            }
            road_create_finished = true;
        }
        if (r_dir == TOP && t_dir == RIGHT)
        {
            for (int y = yt; y <= yr; y++)
            {
                if (y == yt)
                {
                    for (int x = xt; x <= xr; x++)
                    {
                        road_info[road_count, length, 0] = x;
                        road_info[road_count, length, 1] = yt;
                        length++;
                    }
                    length--;
                }
                if (y > yt)
                {
                    road_info[road_count, length, 0] = xr;
                    road_info[road_count, length, 1] = y;
                }
                length++;
            }
            road_create_finished = true;
        }
        if (r_dir == TOP && t_dir == LEFT)
        {
            for (int y = yt; y <= yr; y++)
            {
                if (y == yt)
                {
                    for (int x = xt; x >= xr; x--)
                    {
                        road_info[road_count, length, 0] = x;
                        road_info[road_count, length, 1] = yt;
                        length++;
                    }
                    length--;
                }
                if (y > yt)
                {
                    road_info[road_count, length, 0] = xr;
                    road_info[road_count, length, 1] = y;
                }
                length++;
            }
            road_create_finished = true;
        }
        if (r_dir == LEFT && t_dir == RIGHT)
        {
            for (int x = xt; x <= xr; x++)
            {
                if (x == xt)
                {
                    road_info[road_count, length, 0] = xt;
                    road_info[road_count, length, 1] = yt;
                }
                if (midX != 0 && x != xt && x != xr)
                {
                    if (x < midX && x > xt)
                    {
                        road_info[road_count, length, 0] = x;
                        road_info[road_count, length, 1] = yt;
                    }
                    if (x == midX)
                    {
                        int minY = 0;
                        int maxY = 0;
                        if (yt <= yr)
                        {
                            minY = yt;
                            maxY = yr;
                        }
                        else
                        {
                            minY = yr;
                            maxY = yt;
                        }
                        for (int y = minY; y <= maxY; y++)
                        {
                            road_info[road_count, length, 0] = midX;
                            road_info[road_count, length, 1] = y;
                            length++;
                        }
                        length--;
                    }
                    if (x > midX && x < xr)
                    {
                        road_info[road_count, length, 0] = x;
                        road_info[road_count, length, 1] = yr;
                    }
                }
                if (midX == 0 && x > xt && x < xr)
                {
                    road_info[road_count, length, 0] = x;
                    road_info[road_count, length, 1] = yr;
                }
                if (x == xr)
                {
                    road_info[road_count, length, 0] = xr;
                    road_info[road_count, length, 1] = yr;
                }
                length++;
            }
            road_create_finished = true;
        }
        if (r_dir == RIGHT && t_dir == LEFT)
        {
            for (int x = xr; x <= xt; x++)
            {
                if (x == xr)
                {
                    road_info[road_count, length, 0] = x;
                    road_info[road_count, length, 1] = yr;
                }
                if (midX != 0 && x != xr && x != xt)
                {
                    if (x < midX && x > xr)
                    {
                        road_info[road_count, length, 0] = x;
                        road_info[road_count, length, 1] = yr;
                    }
                    if (x == midX)
                    {
                        int minY = 0;
                        int maxY = 0;
                        if (yt <= yr)
                        {
                            minY = yt;
                            maxY = yr;
                        }
                        else
                        {
                            minY = yr;
                            maxY = yt;
                        }
                        for (int y = minY; y <= maxY; y++)
                        {
                            road_info[road_count, length, 0] = midX;
                            road_info[road_count, length, 1] = y;
                            length++;
                        }
                        length--;
                    }
                    if (x > midX && x < xt)
                    {
                        road_info[road_count, length, 0] = x;
                        road_info[road_count, length, 1] = yt;
                    }
                }
                if (midX == 0 && x != xt && x != xr)
                {
                    road_info[road_count, length, 0] = x;
                    road_info[road_count, length, 1] = yr;
                }
                if (x == xt)
                {
                    road_info[road_count, length, 0] = xt;
                    road_info[road_count, length, 1] = yt;
                }
                length++;
            }
            road_create_finished = true;
        }
        if (r_dir == BOTTOM && t_dir == TOP)
        {
            for (int y = yr; y <= yt; y++)
            {
                if (y == yr)
                {
                    road_info[road_count, length, 0] = xr;
                    road_info[road_count, length, 1] = y;
                }
                if (midY != 0 && y != yr && y != yt)
                {
                    if (y < midY && y > yr)
                    {
                        road_info[road_count, length, 0] = xr;
                        road_info[road_count, length, 1] = y;
                    }
                    if (y == midY)
                    {
                        int minX = 0;
                        int maxX = 0;
                        if (xt <= xr)
                        {
                            minX = xt;
                            maxX = xr;
                        }
                        else
                        {
                            minX = xr;
                            maxX = xt;
                        }
                        for (int x = minX; x <= maxX; x++)
                        {
                            road_info[road_count, length, 0] = x;
                            road_info[road_count, length, 1] = midY;
                            length++;
                        }
                        length--;
                    }
                    if (y > midY && y < yt)
                    {
                        road_info[road_count, length, 0] = xt;
                        road_info[road_count, length, 1] = y;
                    }
                }
                if (midY == 0 && y != yr && y != yt)
                {
                    road_info[road_count, length, 0] = xr;
                    road_info[road_count, length, 1] = y;
                }
                if (y == yt)
                {
                    road_info[road_count, length, 0] = xt;
                    road_info[road_count, length, 1] = y;
                }
                length++;
            }
            road_create_finished = true;
        }
        if (r_dir == BOTTOM && t_dir == RIGHT)
        {
            for (int y = yr; y <= yt; y++)
            {
                if (y < yt)
                {
                    road_info[road_count, length, 0] = xr;
                    road_info[road_count, length, 1] = y;
                }
                if (y == yt)
                {
                    for (int x = xr; x >= xt; x--)
                    {
                        road_info[road_count, length, 0] = x;
                        road_info[road_count, length, 1] = yt;
                        length++;
                    }
                    length--;
                }
                length++;
            }
            road_create_finished = true;
        }
        if (r_dir == BOTTOM && t_dir == LEFT)
        {
            for (int y = yr; y <= yt; y++)
            {
                if (y < yt)
                {
                    road_info[road_count, length, 0] = xr;
                    road_info[road_count, length, 1] = y;
                }
                if (y == yt)
                {
                    for (int x = xr; x <= xt; x++)
                    {
                        road_info[road_count, length, 0] = x;
                        road_info[road_count, length, 1] = yt;
                        length++;
                    }
                    length--;
                }
                length++;
            }
            road_create_finished = true;
        }

        if (road_create_finished)
        {
            road_count++;
        }
        return road_create_finished;
    }

    public int[,] DungeonInfoMake()
    {
        //初期化
        for (int y = 0; y < DUNGEON_HEIGHT; y++)
        {
            for (int x = 0; x < DUNGEON_WIDTH; x++)
            {
                dungeon2D[y, x] = 0;
                dung_2D[y, x] = 0;
            }
        }
        for (int k = 0; k < MAX_ROOM_COUNT; k++)
        {
            for (int i = 0; i < 4; i++)
            {
                room[k, i] = 0;
            }
        }
        for (int i = 0; i < MAX_ROAD_COUNT; i++)
        {
            room_connect[i, 0] = 0;     //0はまずくない？0番の部屋もある
            room_connect[i, 1] = 0;
        }
        for (int k = 0; k < MAX_ROAD_COUNT; k++)
        {
            for (int i = 0; i < MAX_ROAD_LENGTH; i++)
            {
                road_info[k, i, 0] = 0;
                road_info[k, i, 1] = 0;
            }
        }
        road_cre_attempt = 0;
        room_count = 0;
        road_count = 0;
        xr = yr = xt = yt = midX = midY = 0;

        //まず部屋の矩形を作る
        for (int i = 0; i < MAX_ROOM_COUNT; i++)
        {
            bool ok = false;
            int atmp_count = 0;
            while (!ok)
            {
                int x1 = Random.Range(0, DUNGEON_WIDTH - 4) + 2;        //マップの左右端は必ず壁なので
                int y1 = Random.Range(0, DUNGEON_HEIGHT - 4) + 2;
                int x2 = Random.Range(0, DUNGEON_WIDTH - 4) + 2;        //マップの左右端は必ず壁なので
                int y2 = Random.Range(0, DUNGEON_HEIGHT - 4) + 2;
                int left = 0;
                int top = 0;
                int right = 0;
                int bottom = 0;
                if (x2 >= x1)
                {
                    left = x1;
                    right = x2;
                }
                else if (x1 >= x2)
                {
                    right = x1;
                    left = x2;
                }
                if (y2 >= y1)
                {
                    top = y1;
                    bottom = y2;
                }
                else if (y1 >= y2)
                {
                    bottom = y1;
                    top = y2;
                }
                if (right - left >= ROOM_MIN_SIDE
                    && bottom - top >= ROOM_MIN_SIDE
                    && right - left <= ROOM_MAX_SIDE
                    && bottom - top <= ROOM_MAX_SIDE)
                {
                    room[i, 0] = left;
                    room[i, 1] = top;
                    room[i, 2] = right;
                    room[i, 3] = bottom;

                    if (Room_Overlap_Check(i))
                    {
                        ok = true;
                        for (int y = top; y <= bottom; y++)
                        {
                            for (int x = left; x <= right; x++)
                            {
                                dung_2D[y, x] = ROOM_SPACE;
                            }
                        }
                    }
                    if (!ok)
                    {
                        room[i, 0] = 0;
                        room[i, 1] = 0;
                        room[i, 2] = 0;
                        room[i, 3] = 0;
                    }
                }
                atmp_count++;
                if (atmp_count >= MAX_ATTEMPT)
                {
                    ok = true;
                }
            }
        }

        //部屋情報を上に詰めて格納し、空きをなくす
        //(room[i, 0] == 0 が間に来ないようにする)
        for (int i = 0; i < MAX_ROOM_COUNT; i++)
        {
            int r = i;
            if (r > 0)
            {
                int count = 1;
                while (r - count >= 0
                    && r - count <= MAX_ROOM_COUNT - 1  //←	r - count + 1 <= MAX_ROOM_COUNT
                    && room[r - count, 0] == 0)
                {

                    room[r - count, 0] = room[r - count + 1, 0];
                    room[r - count + 1, 0] = 0;
                    room[r - count, 1] = room[r - count + 1, 1];
                    room[r - count + 1, 1] = 0;
                    room[r - count, 2] = room[r - count + 1, 2];
                    room[r - count + 1, 2] = 0;
                    room[r - count, 3] = room[r - count + 1, 3];
                    room[r - count + 1, 3] = 0;

                    count++;
                }
            }
        }
        room_count = 0;
        while (room[room_count, 0] != 0
            && room_count <= MAX_ROOM_COUNT)
        {
            room_count++;                           //部屋の数をチェック
        }

        //部屋ができたので次に道をつくる
        int road_c = 0;
        road_cre_attempt = 0;
        while (road_c < (room_count - 1) && road_cre_attempt < 30000)
        {
            if (road_c == 0)
            {
                room_connect[road_c, 0] = Random.Range(0, room_count);
            }
            else
            {
                int num = Random.Range(0, road_c);
                int d = Random.Range(0, 2);
                room_connect[road_c, 0] = room_connect[num, d];
            }
            int t_room = Random.Range(0, room_count);
            bool ok = false;
            int t_room_sel_attempt = 0;
            while (!ok)
            {
                t_room = Random.Range(0, room_count);
                ok = true;
                if (road_c > 0)
                {
                    for (int k = 0; k < road_c; k++)
                    {
                        if (t_room == room_connect[k, 0]
                            || t_room == room_connect[k, 1])
                        {
                            ok = false;
                        }
                    }
                }
                if (t_room == room_connect[road_c, 0])
                {
                    ok = false;
                }
                t_room_sel_attempt++;
                if (t_room_sel_attempt > 1000)
                {
                    ok = true;
                }
            }
            room_connect[road_c, 1] = t_room;
            xr = 0;
            yr = 0;
            xt = 0;
            yt = 0;
            //道を伸ばす主体の部屋の4辺
            int left = room[room_connect[road_c, 0], 0];
            int top = room[room_connect[road_c, 0], 1];
            int right = room[room_connect[road_c, 0], 2];
            int bottom = room[room_connect[road_c, 0], 3];
            //道を伸ばす先のターゲット部屋の4辺
            int t_left = room[room_connect[road_c, 1], 0];
            int t_top = room[room_connect[road_c, 1], 1];
            int t_right = room[room_connect[road_c, 1], 2];
            int t_bottom = room[room_connect[road_c, 1], 3];

            bool create_success = false;

            if (!create_success && t_bottom + 1 <= top - 1)
            {
                if (t_left <= right && left <= t_right)
                {
                    yr = top - 1;
                    yt = t_bottom + 1;
                    int minX = 0;
                    int maxX = 0;
                    midY = 0;
                    if (left <= t_left)
                    {
                        minX = t_left;
                    }
                    else
                    {
                        minX = left;
                    }
                    if (right <= t_right)
                    {
                        maxX = right;
                    }
                    else
                    {
                        maxX = t_right;
                    }
                    if (yr - yt <= 1)
                    {
                        xr = xt = minX + Random.Range(0, maxX + 1 - minX);
                    }
                    else
                    {
                        xr = left + Random.Range(0, right + 1 - left);
                        xt = t_left + Random.Range(0, t_right + 1 - t_left);
                        int d = yr - yt;
                        midY = yt + (d / 2);
                    }
                    create_success = Road_Create(TOP, BOTTOM);
                }
                if (t_right < left)
                {
                    yr = top - 1;
                    xt = t_right + 1;
                    xr = left + Random.Range(0, right + 1 - left);
                    yt = t_top + Random.Range(0, t_bottom + 1 - t_top);
                    create_success = Road_Create(TOP, RIGHT);
                }
                if (right < t_left)
                {
                    yr = top - 1;
                    xt = t_left - 1;
                    xr = left + Random.Range(0, right + 1 - left);
                    yt = t_top + Random.Range(0, t_bottom + 1 - t_top);
                    create_success = Road_Create(TOP, LEFT);
                }
            }
            if (!create_success && t_bottom + 1 > top - 1 && t_top - 1 < bottom + 1)
            {
                //ここはすべて左右辺同士
                if (t_right < left)
                {
                    xt = t_right + 1;
                    xr = left - 1;
                    midX = 0;
                    if (t_bottom == top - 1)
                    {
                        if (t_right + 2 < left - 1)
                        {
                            int d = xr - xt;
                            midX = xt + (d / 2);
                            yt = t_top + Random.Range(0, t_bottom + 1 - t_top);
                            yr = top + Random.Range(0, bottom + 1 - top);
                        }
                        else
                        {
                            yr = top - 1;
                            xr = left + 1 + Random.Range(0, right - left);
                            xt = t_right + 1;
                            yt = t_top + Random.Range(0, t_bottom - t_top);
                            create_success = Road_Create(TOP, RIGHT);
                            break;
                        }
                    }
                    if (top <= t_bottom && t_top <= bottom)
                    {
                        int minY = 0;
                        int maxY = 0;
                        if (t_top <= top)
                        {
                            minY = top;
                        }
                        else
                        {
                            minY = t_top;
                        }
                        if (t_bottom <= bottom)
                        {
                            maxY = t_bottom;
                        }
                        else
                        {
                            maxY = bottom;
                        }
                        yt = yr = minY + Random.Range(0, maxY + 1 - minY);
                    }
                    if (t_top == bottom + 1)
                    {
                        if (t_right + 2 < left - 1)
                        {
                            int d = xr - xt;
                            midX = xt + (d / 2);
                            yt = t_top + Random.Range(0, t_bottom + 1 - t_top);
                            yr = top + Random.Range(0, bottom + 1 - top);
                        }
                        else
                        {
                            yr = bottom + 1;
                            xr = left + 1 + Random.Range(0, right - left);
                            xt = t_right + 1;
                            yt = t_top + 1 + Random.Range(0, t_bottom - t_top);
                            create_success = Road_Create(BOTTOM, RIGHT);
                            break;
                        }
                    }
                    if (yr != 0 && yt != 0)
                    {
                        create_success = Road_Create(LEFT, RIGHT);
                    }

                }
                if (right < t_left)
                {
                    xt = t_left - 1;
                    xr = right + 1;
                    midX = 0;
                    if (t_bottom == top - 1)
                    {
                        if (right + 2 < t_left - 1)
                        {
                            int d = xt - xr;
                            midX = xr + (d / 2);
                            yt = t_top + Random.Range(0, t_bottom + 1 - t_top);
                            yr = top + Random.Range(0, bottom + 1 - top);
                        }
                        else
                        {
                            yr = top - 1;
                            xr = left + Random.Range(0, right - left);
                            xt = t_left - 1;
                            yt = t_top + Random.Range(0, t_bottom - t_top);
                            create_success = Road_Create(TOP, LEFT);
                            break;
                        }
                    }
                    if (top <= t_bottom && t_top <= bottom)
                    {
                        int minY = 0;
                        int maxY = 0;
                        if (t_top <= top)
                        {
                            minY = top;
                        }
                        else
                        {
                            minY = t_top;
                        }
                        if (t_bottom <= bottom)
                        {
                            maxY = t_bottom;
                        }
                        else
                        {
                            maxY = bottom;
                        }
                        yt = yr = minY + Random.Range(0, maxY + 1 - minY);
                    }
                    if (t_top == bottom + 1)
                    {
                        if (right + 2 < t_left - 1)
                        {
                            int d = xt - xr;
                            midX = xr + (d / 2);
                            yt = t_top + Random.Range(0, t_bottom + 1 - t_top);
                            yr = top + Random.Range(0, bottom + 1 - top);
                        }
                        else
                        {
                            yr = bottom + 1;
                            xr = left + Random.Range(0, right - left);
                            xt = t_left - 1;
                            yt = t_top + 1 + Random.Range(0, t_bottom - t_top);
                            create_success = Road_Create(BOTTOM, LEFT);
                            break;
                        }
                    }
                    if (yr != 0 && yt != 0)
                    {
                        create_success = Road_Create(RIGHT, LEFT);
                    }
                }
            }
            if (!create_success && bottom + 1 <= t_top - 1)
            {
                if (t_left <= right && left <= t_right)
                {
                    yr = bottom + 1;
                    yt = t_top - 1;
                    int minX = 0;
                    int maxX = 0;
                    midY = 0;
                    if (left <= t_left)
                    {
                        minX = t_left;
                    }
                    else
                    {
                        minX = left;
                    }
                    if (right <= t_right)
                    {
                        maxX = right;
                    }
                    else
                    {
                        maxX = t_right;
                    }
                    if (yt - yr <= 1)
                    {
                        xr = xt = minX + Random.Range(0, maxX + 1 - minX);
                    }
                    else
                    {
                        xr = left + Random.Range(0, right + 1 - left);
                        xt = t_left + Random.Range(0, t_right + 1 - t_left);
                        int d = yt - yr;
                        midY = yr + (d / 2);
                    }
                    create_success = Road_Create(BOTTOM, TOP);
                }
                if (t_right < left)
                {
                    yr = bottom + 1;
                    xt = t_right + 1;
                    xr = left + Random.Range(0, right + 1 - left);
                    yt = t_top + Random.Range(0, t_bottom + 1 - t_top);
                    create_success = Road_Create(BOTTOM, RIGHT);
                }
                if (right < t_left)
                {
                    yr = bottom + 1;
                    xt = t_left - 1;
                    xr = left + Random.Range(0, right + 1 - left);
                    yt = t_top + Random.Range(0, t_bottom + 1 - t_top);
                    create_success = Road_Create(BOTTOM, LEFT);
                }
            }
            if (!create_success)
            {
                room_connect[road_c, 0] = 0;
                room_connect[road_c, 1] = 0;
            }
            if (create_success)
            {
                road_c++;
                create_success = false;
            }
            road_cre_attempt++;
        }
        for (int i = 0; i < MAX_ROAD_COUNT; i++)
        {
            for (int k = 0; k < MAX_ROAD_LENGTH; k++)
            {
                if (road_info[i, k, 0] != 0 || road_info[i, k, 1] != 0)
                {
                    dung_2D[road_info[i, k, 1], road_info[i, k, 0]] = ROAD;
                    if (k == 0)
                    {
                        dung_2D[road_info[i, k, 1], road_info[i, k, 0]] = ROAD_S;
                    }
                }
                else
                {
                    if (k >= 1)
                    {
                        if (road_info[i, k - 1, 0] != 0 || road_info[i, k - 1, 1] != 0)
                        {
                            dung_2D[road_info[i, k - 1, 1], road_info[i, k - 1, 0]] = ROAD_G;
                        }
                    }
                }
            }
        }

        for (int y = 0; y < DUNGEON_HEIGHT; y++)
        {
            for (int x = 0; x < DUNGEON_WIDTH; x++)
            {
                dungeon2D[y, x] = dung_2D[y, x];

            }
        }

        //for (int y = 0; y < DUNGEON_HEIGHT; y++)
        //{
        //    string info = "";
        //    for (int x = 0; x < DUNGEON_WIDTH; x++)
        //    {
        //        info += dung_2D[y, x];
        //    }
        //    Debug.Log(info);
        //}
        Debug.Log("DungeonMake");
        return dung_2D;
    }
}
