using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InactiveFarFromPlayer : MonoBehaviour {

    public float distanceInvisible = 10;

    GameObject parent;
    InactiveFarManager inactiveFarMn;
    GameObject player;
    GameObject[] gameObjectList;
    bool isActive = true;

	// Use this for initialization
	void Start () {
        parent = GameObject.Find("GameObjectParent");
        inactiveFarMn = parent.GetComponentInChildren<InactiveFarManager>();
        inactiveFarMn.objects.Add(this);
        player = parent.GetComponentInChildren<Player>().gameObject;

        Body[] bodys = GetComponentsInChildren<Body>();
        // 初期化としてすべてアクティブ化
        // （無駄かつ重いかも？）
        gameObjectList = new GameObject[bodys.Length];
        for(int i = 0; i < gameObjectList.Length; i++ )
        {
            gameObjectList[i] = bodys[i].gameObject;
        }
	}
	
	// Update is called once per frame
	public void UpdateInactivate () {
        float distanceSqrMag = ( transform.position - player.transform.position ).sqrMagnitude;

        // プレイヤーから一定距離遠かったら非表示
        bool isClose = ( distanceSqrMag < distanceInvisible * distanceInvisible );
        if (isActive != isClose )
        {
            isActive = isClose;
            foreach(GameObject obj in gameObjectList )
            {
                if ( obj )
                    obj.SetActive(isActive);
            }
        }
	}
}
