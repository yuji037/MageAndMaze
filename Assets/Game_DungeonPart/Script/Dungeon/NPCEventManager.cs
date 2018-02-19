using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCEventManager : MonoBehaviour {

    EnemyType playerMetEnemyType;

    GameObject parent;
    EventSceneManager eventSceneMn;
    EnemyManager eneMn;

    // Use this for initialization
    void Start () {
        parent = GameObject.Find("GameObjectParent");
        eventSceneMn = parent.GetComponentInChildren<EventSceneManager>();
        eneMn = parent.GetComponentInChildren<EnemyManager>();
	}
	
    public void SetEnemyType(EnemyType type)
    {
        playerMetEnemyType = type;
    }

    public void CauseEvent()
    {
        string _fileName = "EventTextNPC1";

        eventSceneMn.EventStart(_fileName);
    }

    public void CauseEffectiveEvent(int eventNum)
    {
        switch ( eventNum )
        {
            case 20:
                // ソウルストーン入手
                {
                    var playerItem = parent.GetComponentInChildren<PlayerItem>();
                    playerItem.GetItem(0, 10);
                    playerItem.GetItem(1, 10);
                    playerItem.GetItem(2, 10);
                }
                break;
            case 25:
                // ソウルストーン減少
                {
                    var playerItem = parent.GetComponentInChildren<PlayerItem>();
                    playerItem.GetItem(0, -10);
                    playerItem.GetItem(1, -10);
                    playerItem.GetItem(2, -10);
                }
                break;
            case 30:
                // ランダム敵ポップ
                for ( int i = 0; i < 5; i++ )
                {
                    eneMn.Spawn(false);
                }
                break;
            case 31:
                // 話しかけたNPCの敵種のみポップ
                for ( int i = 0; i < 5; i++ )
                {
                    eneMn.Spawn(false, playerMetEnemyType);
                }
                break;
            case 32:
                // 光源タイプのみポップ
                for ( int i = 0; i < 5; i++ )
                {
                    eneMn.Spawn(false, EnemyType.LIGHT);
                }
                break;
            case 33:
                // 豪華タイプのみポップ
                for ( int i = 0; i < 5; i++ )
                {
                    eneMn.Spawn(false, EnemyType.TREASURE);
                }
                break;
            default:
                break;
        }

    }
}
