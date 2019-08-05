using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 戦闘する全キャラのターン管理クラス。
/// タイミングに合わせて、各Actionの開始を指示する
/// </summary>
public class TurnManager : MonoBehaviour {
    
    [SerializeField]
    public bool PlayerActionSelected { get; private set; }
    [SerializeField]
    public float moveSpeed = 3.0f;

    [SerializeField]
    private bool allActFinish = false;

    private int turnActNum = 0;
    List<ActionData> turnTable;

    Player player;
    PlayerMove playerMove;
    MapManager mapMn;
    EnemyManager eneMn;
    UIMiniMap miniMap;
    ObstacleManager obsMn;
    OnGroundObjectManager ogoMn;
    InactiveFarManager inactiveFarMn;
    TutorialManager tutorialMn;

    [SerializeField]
    bool outDebugLog = false;

    int saveTurnCount = 4;

    // （30Fでは）ステージボスのHPを監視
    [SerializeField]
    Enemy boss = null;

    private void Awake()
    {
        GameObject oParent	= GameObject.Find("GameObjectParent");
        player				= oParent.GetComponentInChildren<Player>();
        playerMove			= oParent.GetComponentInChildren<PlayerMove>();
        eneMn				= oParent.GetComponentInChildren<EnemyManager>();
        miniMap				= oParent.GetComponentInChildren<UIMiniMap>();
        mapMn				= oParent.GetComponentInChildren<MapManager>();
        obsMn				= oParent.GetComponentInChildren<ObstacleManager>();
        ogoMn				= oParent.GetComponentInChildren<OnGroundObjectManager>();
        inactiveFarMn		= oParent.GetComponentInChildren<InactiveFarManager>();
        tutorialMn			= oParent.GetComponentInChildren<TutorialManager>();
        turnTable			= new List<ActionData>();
    }

    public void PlayerActSelect()
    {
        // すでに行動選択が終わっている場合は無意味
        if ( PlayerActionSelected ) return;
        PlayerActionSelected = true;

        // ステージボスがいる場合はボスを検出
        foreach(Enemy enemy in eneMn.enemys )
        {
            var		_boss = enemy.gameObject.GetComponent<StageBoss>();
            if (	_boss ) boss = enemy;
        }

        eneMn.EnemyActionSelect();

        // デバッグ用に行動内容を参照する
        foreach ( ActionData turn in turnTable )
        {
            //Debug.Log(turn.actType.ToString());
        }

        // ターンコルーチンの開始
        turnActNum = 0;
        StartCoroutine(TurnTableCoroutine());
    }

    // Update is called once per frame
    //void Update () {
    IEnumerator TurnTableCoroutine() {

        while ( true )
        {
            // 行動をさせる番号を進める処理
            for ( ; turnActNum < turnTable.Count - 1; )
            {
                // 移動以外のアクションは１つずつ順番に行うが
                // 移動アクションは一斉に行う

                // 今アクション「移動」
                // 次アクション「移動」
                // なら次も行動可
                if (		turnTable[turnActNum	].actType == ActionType.MOVE
                    &&		turnTable[turnActNum + 1].actType == ActionType.MOVE )
                {
                    ++turnActNum;
                    continue;
                }
                // 今アクション「移動」
                // 次アクション「移動じゃない」
                // なら今アクションが完了したら次が行動可
                else if (	turnTable[turnActNum	].actType == ActionType.MOVE
                    &&		turnTable[turnActNum + 1].actType != ActionType.MOVE )
                {
                    if ( turnTable[turnActNum].finish )
                    {
                        ++turnActNum;
                        continue;
                    }
                }
                // 今アクション「移動じゃない」
                // なら今アクションが完了したら次が行動可
                else if (	turnTable[turnActNum].actType != ActionType.MOVE )
                {
                    if ( turnTable[turnActNum].finish )
                    {
                        ++turnActNum;
                        continue;
                    }
                }

                // 今アクションがプレイヤーから遠かったら演出を省略
                // 次アクションが移動だったら行動可とする、または、
                // 次アクションが省略だったら行動可とする
                else if (	false == turnTable[	turnActNum		].actChara.isPlayPerformance )
                {
                    if ( turnTable[turnActNum + 1].actType == ActionType.MOVE )
                    {
                        ++turnActNum;
                        continue;
                    }
                    if (	false == turnTable[	turnActNum + 1	].actChara.isPlayPerformance )
                    {
                        ++turnActNum;
                        continue;
                    }
                }
                break;
            }

            // 0番から所定の番号までの行動を許可する
            for ( int n = 0; n <= turnActNum; ++n )
            {
                //Debug.Log("turnNum = " + n);
                turnTable[n].allowed = true;
            }

            // プレイヤーの死亡判定
            if ( player.HP <= 0 ) yield return StartCoroutine(player.ToGameoverScene());

            player.CharaUpdate();
            playerMove.MoveUpdate();
            foreach ( Enemy ene in eneMn.enemys )
            {
                ene.CharaUpdate();
            }
            foreach (Obstacle obs in obsMn.obstacles )
            {
                obs.CharaUpdate();
            }

            int lastActNum = turnTable.Count - 1;
            if ( turnTable[lastActNum].finish )
            {
                // 全ての行動をfinishしたかチェック
                allActFinish = true;
                for ( int n = 0; n <= lastActNum - 1; ++n )
                {
                    if ( !turnTable[n].finish )
                    {
                        allActFinish = false;
                        break;
                    }
                }
            }

            // turnTableデバッグ表示
            if ( turnTable.Count > 0 )
            {
                DebugMessage.Print("turnActNum : " + turnActNum);
                int count = 0;
                foreach ( ActionData act in turnTable )
                {
                    DebugMessage.Print("Turn " + count + " : " + act.actChara + act.sequence + act.actType + act.pos + act.finish);
                    ++count;
                }
                DebugMessage.UpdateText();
            }

            // 全ターン終了処理
            // プレイヤーが死んでいればセーブしない
            if ( player.HP > 0 && allActFinish )
            {
                allActFinish = false;
                Debug.Log("全ターン終了");

                PlayerActionSelected = false;

                eneMn.EnemyTurnReset();
                turnTable.Clear();


                // 足元にアイテム、階段などあるかチェック
                playerMove.FootCheck();
                // 死亡した敵がいるか確認
                DeathCheck();
                // ターン経過での敵スポーン
                if ( !tutorialMn.IsTutorialON )
                {
                    eneMn.SpawnCounterPlus();
                }

                miniMap.MiniMapUpdate();
                inactiveFarMn.UpdateInactivateObjects();

                // セーブ
                SaveTurnCheck();

                yield break;
            }

            yield return null;
        }
    }

    private void Update()
    {
        if ( boss )
        {
            if (boss.HP <= 0 )
            {
                // プレイヤー行動不可
                PlayerActionSelected = true;
            }
        }
    }

    public void SaveTurnCheck()
    {
        // チュートリアル中はセーブしない
        if ( tutorialMn.IsTutorialON ) return;

        saveTurnCount++;
        if ( saveTurnCount >= 5 )
        {
            saveTurnCount = 0;
            DungeonSave();
        }
    }

    public void DungeonSave()
    {
        // セーブ
        Debug.Log("セーブ（ターン経過による）");
        player.SavePlayerInfo();
        player.SaveSkill();
        mapMn.Save(1);
        miniMap.SaveRevealedMap();
        eneMn.SaveEnemys();
        obsMn.SaveObstacleData();
        ogoMn.SaveOnGroundObjectData();

        SaveData.Save();
    }

    public void DeathCheck()
    {
        eneMn.DestroyCheck();
        obsMn.DestroyCheck();
    }

    public void AddAction(ActionData addAction)
    {
        addAction.turnMn = this;
        turnTable.Add(addAction);
    }

}
