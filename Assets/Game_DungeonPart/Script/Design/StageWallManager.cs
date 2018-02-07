using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageWallManager : MonoBehaviour {


    [SerializeField]
    GameObject[] bigWalls;

    [SerializeField]
    GameObject wallParent;

    GameObject parent;
    DungeonPartManager dMn;

	// Use this for initialization
	public void Init () {
        parent = GameObject.Find("GameObjectParent");
        dMn = parent.GetComponentInChildren<DungeonPartManager>();

        int wallType = dMn.dungeonType + 1;

        bigWalls = new GameObject[4];
        
        for ( int i = 0; i < 4; ++i )
        {
            int key = wallType * 10 + i;
            var prefab = Resources.Load<GameObject>("StageWallPrefab/" + key) as GameObject;
            var obj = Instantiate(prefab, wallParent.transform);
            bigWalls[i] = obj;
        }
	}
	
}
