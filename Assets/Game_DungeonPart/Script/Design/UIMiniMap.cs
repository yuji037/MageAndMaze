using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMiniMap : MonoBehaviour {

    [SerializeField] GameObject miniMapChip;
    Image[,] miniMapChips;

    [SerializeField] float chipSize;
    int dungeonWidth;
    int dungeonHeight;
    [SerializeField] float alpha;


    [SerializeField] GameObject miniMapParent;
    [SerializeField] GameObject parent;
    MapManager mapMn;
    RevealedMap revealMap;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void MiniMapInit()
    {
        if (!parent) parent = GameObject.Find("GameObjectParent");
        mapMn = parent.GetComponentInChildren<MapManager>();
        revealMap = GetComponent<RevealedMap>();
        dungeonWidth = mapMn.GetDungeonWidth();
        dungeonHeight = mapMn.GetDungeonHeight();
        miniMapChips = new Image[dungeonHeight, dungeonWidth];
        for (int z = 0; z < dungeonHeight; ++z)
        {
            for (int x = 0; x < dungeonWidth; ++x)
            {
                var chip = Instantiate(miniMapChip, miniMapParent.transform);
                chip.transform.localPosition = new Vector3((x - 15) * chipSize, (z - 15) * chipSize, 0);
                miniMapChips[z, x] = chip.GetComponent<Image>();
            }
        }

        revealMap.Init();
        MiniMapUpdate();
    }
    public void MinimapRefresh()
    {
        Vector3 rot = miniMapParent.transform.eulerAngles;
        miniMapParent.transform.eulerAngles = Vector3.zero;
        miniMapParent.transform.eulerAngles = rot;
    }

    public void MiniMapUpdate()
    {
        revealMap.UpdateRevealMap();
        bool[,] reveal2D = revealMap.reveal2D;

        for (int z = 0; z < dungeonHeight; ++z)
        {
            for (int x = 0; x < dungeonWidth; ++x)
            {
                int mapType = mapMn.dung_2D[z, x];
                int charaType = mapMn.chara_exist2D[z, x];
                int objType = mapMn.onground_exist2D[z, x];
                // 初期値：青
                Color color = new Color(0, 0, 1, alpha);
                
                // 階段は紫
                if (objType == 0) { }
                else if(objType == 100 ) { color = new Color(0.5f, 0, 1, 1); }

                if (mapType == -1 || !reveal2D[z,x]) color = new Color(1, 1, 1, 0);

                // デバッグ用
                //switch (mapType) {
                //    case 0:
                //        color = new Color(1, 1, 1, 0);  // 透明
                //        break;
                //    case 1:
                //        color = new Color(0, 0, 1, alpha);  // 
                //        break;
                //    case 2:
                //        color = new Color(0, 0, 1, alpha);  // 
                //        break;
                //    case 3:
                //        color = new Color(0, 0, 1, alpha);  // 
                //        break;
                //    case 4:
                //        color = new Color(0, 0, 1, alpha);  // 
                //        break;
                //    default:
                //        color = new Color(0, 0, 1, alpha);  // 
                //        break;
                //}
                
                // キャラが居る場合
                if (charaType > 0)
                {
                    if ( charaType == 1 )   // プレイヤー
                    {
                        color = new Color(1, 1, 1, alpha);
                    }
                    else if ( charaType >= 500 ) // 敵
                    {
                        if ( revealMap.IsSightRange(new Vector3(x, 0, z)) )
                            color = new Color(1, 0, 0, alpha);
                    }
                    else                    // 障害物
                    {
                        if ( revealMap.IsSightRange(new Vector3(x, 0, z)) )
                            color = new Color(1, 1, 1, 0);
                    }
                }
                miniMapChips[z, x].color = color;
            }
        }
    }
    public void SwitchActive()
    {
        miniMapParent.SetActive(!miniMapParent.activeSelf);
    }

    public void SaveRevealedMap()
    {
        revealMap.Save();
    }
}
