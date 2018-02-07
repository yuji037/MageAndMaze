using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InactiveFarFromPlayer : MonoBehaviour {

    [SerializeField] float distanceInvisible = 10;

    GameObject parent;
    GameObject player;
    GameObject[] gameObjectList;
    bool isActive = true;

	// Use this for initialization
	void Start () {
        parent = GameObject.Find("GameObjectParent");
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
	void Update () {
        float distanceSqrMag = ( transform.position - player.transform.position ).sqrMagnitude;

        // プレイヤーから一定距離遠かったら非表示
        if (isActive != (distanceSqrMag < distanceInvisible * distanceInvisible ) )
        {
            isActive = ( distanceSqrMag < distanceInvisible * distanceInvisible );
            foreach(GameObject obj in gameObjectList )
            {
                obj.SetActive(isActive);
            }
        }
	}
}
