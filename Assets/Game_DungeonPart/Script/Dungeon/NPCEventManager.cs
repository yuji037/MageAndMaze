using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCEventManager : MonoBehaviour {

    Enemy playerMetEnemy = null;
    EnemyType playerMetEnemyType;

    GameObject parent;
    EventCanvasManager eventSceneMn;
    EnemyManager eneMn;

    // Use this for initialization
    void Start () {
        parent = GameObject.Find("GameObjectParent");
        eventSceneMn = parent.GetComponentInChildren<EventCanvasManager>();
        eneMn = parent.GetComponentInChildren<EnemyManager>();
	}
	
    public void SetEnemyType(Enemy enemy)
    {
        playerMetEnemy = enemy;
        playerMetEnemyType = enemy.type;
    }

    public bool finishedEventOnThisFloor = false;

    public void CauseEvent()
    {
        if ( finishedEventOnThisFloor ) return;
        finishedEventOnThisFloor = true;
        // 1～6までのイベントをランダムで呼び出す
        string _fileName = "Event_" + Random.Range(1, 7);

        eventSceneMn.EventStart(_fileName);
    }

    public void CauseEffectiveEvent(int eventNum)
    {
        int i;
        Debug.Log("eventNum = " + eventNum);
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
                for ( i = 0; i < 2; i++ )
                {
                    eneMn.Spawn(false, (EnemyType)(-1), false);
                }
                for ( i = 2; i < 5; i++ )
                {
                    eneMn.Spawn(false);
                }
                break;
            case 31:
                // 話しかけたNPCの敵種のみポップ
                for ( i = 0; i < 2; i++ )
                {
                    eneMn.Spawn(false, playerMetEnemyType, false);
                }
                for ( i = 2; i < 5; i++ )
                {
                    eneMn.Spawn(false, playerMetEnemyType);
                }
                break;
            case 32:
                // 光源タイプのみポップ
                for ( i = 0; i < 2; i++ )
                {
                    eneMn.Spawn(false, EnemyType.LIGHT, false);
                }
                for ( i = 2; i < 5; i++ )
                {
                    eneMn.Spawn(false, EnemyType.LIGHT);
                }
                break;
            case 33:
                // 豪華タイプのみポップ
                for ( i = 0; i < 2; i++ )
                {
                    eneMn.Spawn(false, EnemyType.TREASURE, false);
                }
                for ( i = 2; i < 5; i++ )
                {
                    eneMn.Spawn(false, EnemyType.TREASURE);
                }
                break;
            default:
                break;
        }

        if (20 <= eventNum && eventNum < 50 )
        {
            KillEnemy();
        }

    }
    
    void KillEnemy()
    {
        if (playerMetEnemy)
        playerMetEnemy.Kill(false);
    }
}
