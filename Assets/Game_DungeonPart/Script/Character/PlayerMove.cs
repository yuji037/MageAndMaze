using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    GameObject parent;
    Player player;
    public Vector3 resMoveDir;
    MapManager mapMn;
    public TurnManager turnMn;
    MoveButtonManager moveBtMn;

    GameObject checkNextFloorWindow;

    public int charaID = 0;
    public Vector3 moveVec;
    AnimationChanger anim;
    bool init = false;

    UISwitch uiSwitch;
    EventCanvasManager eventSceneMn;

    TutorialManager tutorialMn;
    EnemyManager eneMn;
    OnGroundObjectManager ogoMn;

    private void Awake()
    {
        parent = GameObject.Find("GameObjectParent");
        player = parent.GetComponentInChildren<Player>();
        mapMn = parent.GetComponentInChildren<MapManager>();
        turnMn = parent.GetComponentInChildren<TurnManager>();
        moveBtMn = parent.GetComponentInChildren<MoveButtonManager>();
        anim = GetComponent<AnimationChanger>();
        uiSwitch = parent.GetComponentInChildren<UISwitch>();
        eventSceneMn = parent.GetComponentInChildren<EventCanvasManager>();
        tutorialMn = parent.GetComponentInChildren<TutorialManager>();
        eneMn = parent.GetComponentInChildren<EnemyManager>();
        ogoMn = parent.GetComponentInChildren<OnGroundObjectManager>();
    }

    // Use this for initialization
    void Start()
    {
        uiSwitch.SwitchSubUI((int)SubUIType.CHECK_NEXT_FLOOR, false);
        uiSwitch.SwitchSubUI((int)SubUIType.INTERACT, false);
    }

    private void Update()
    {
        if ( !init )
        {
            player.pos = transform.position;
            player.sPos = player.pos;
            player.SetObjectDir();
            init = true;
        }
    }

    // Update is called once per frame
    public void MoveUpdate()
    {
        if ( moveVec != Vector3.zero )
        {
            Move();
        }
    }

    float actionRate = 0;

    public void Move()
    {
        actionRate += Time.deltaTime * turnMn.moveSpeed;
        //Debug.Log("Player actionRate : " + actionRate);
        if ( actionRate >= 1 )
        {
            // 移動終了
            transform.position = player.pos + moveVec;
            player.pos += moveVec;
            moveVec = Vector3.zero;
            anim.TriggerAnimator("Move", false);
            moveBtMn._isActive = true;
            Debug.Log("プレイヤー移動終了");
            player.ActEnd();

            actionRate = 0;
        }
        else
        {
            Vector3 distance = moveVec * actionRate;
            transform.position = player.pos + distance;
        }
    }

    public bool MoveStart(Vector3 dir)   // true ならば 先行入力が有効
    {
        // チュートリアル中の操作制限
        if ( tutorialMn.IsTutorialON )
        {
            if ( (2 <= tutorialMn.TutorialNumber
                && tutorialMn.TutorialNumber <= 3)
                || ( 5 <= tutorialMn.TutorialNumber
                && tutorialMn.TutorialNumber <= 7 )
                ) { return false; }
        }

        if ( turnMn.PlayerActionSelected ) return true;

        if ( player.abnoState.invincibleTurn > 0 ) return false;

        // 選択方向を向く
        player.charaDir = dir;
        player.SetObjectDir();

        Debug.Log("マップ外かどうか");
        // マップ外かどうか
        if ( !mapMn.InsideMap(player.pos + dir) ) return false;

        Debug.Log(" 移動可能な空間かどうか");
        // 移動可能な空間かどうか
        if ( !mapMn.CanMoveCheck(player.pos, player.pos + dir) ) return false;
        Debug.Log(" 移動可能な空間");

        moveBtMn._isActive = false;
        player.sPos = player.pos + dir;
        mapMn.SetCharaExistInfo(player.pos);
        mapMn.SetCharaExistInfo(player.sPos, charaID, true);
        Debug.Log(player.sPos + "にプレイヤー移動選択");

        // プレイヤーの移動すべきベクトルを設定
        moveVec = dir;

        anim.TriggerAnimator("Move", true);
        player.action = ActionType.MOVE;
        ActionData addAction = new ActionData(player, player.action, 0, 0, Vector3.zero);
        player.thisTurnAction.Add(addAction);
        turnMn.AddAction(addAction);
        player.PlayerActSelect();
        return false;
    }

    public void SetCharaDir(Vector3 dir)
    {
        if ( moveVec != Vector3.zero ) return; // 移動中は向きを変えない
        player.charaDir = dir;
        player.SetObjectDir();
    }

    public void FootCheck()
    {
        // 階段、水たまり、回復パネル等のIDチェック
        int onFootType = mapMn.onground_exist2D[(int)player.pos.z, (int)player.pos.x];
        OnGroundObject ogo = ogoMn.GetOnGroundObject(onFootType);
        // 階段
        if ( onFootType == 100 )
        {
            uiSwitch.SwitchSubUI((int)SubUIType.CHECK_NEXT_FLOOR, true);
        }
        else
        {
            uiSwitch.SwitchSubUI((int)SubUIType.CHECK_NEXT_FLOOR, false);
        }

        // 回復パネル
        if ( ogo && ogo.type == OnGroundObject.Type.HEAL_PANEL )
        {
            player.HealByPercent(0.1f);
        }

        // 周囲にNPCがいないかチェック
        uiSwitch.SwitchSubUI((int)SubUIType.INTERACT, false);

        if ( eventSceneMn.IsThisEventFinished("EventTextNPC1") ) return;

        for ( int z = (int)player.pos.z - 1; z <= (int)player.pos.z + 1; z++ )
        {
            for ( int x = (int)player.pos.x - 1; x <= (int)player.pos.x + 1; x++ )
            {
                Vector3 checkPos = new Vector3(x, 0, z);
                if ( !mapMn.InsideMap(checkPos) ) continue;
                int chara = mapMn.chara_exist2D[z, x];
                // その場所に居るのが敵キャラならば
                if ( 500 <= mapMn.chara_exist2D[z, x] )
                {
                    var enemy = eneMn.GetEnemy(mapMn.chara_exist2D[z, x]);
                    if ( !enemy.isSpeakable ) continue;

                    // その敵に話しかけ可能な場合
                    var npcEventMn = parent.GetComponentInChildren<NPCEventManager>();

                    // このフロアでもうイベントが終わっている場合
                    if ( npcEventMn.finishedEventOnThisFloor ) continue;

                    npcEventMn.SetEnemyType(enemy);

                    // 「話しかける」ボタンの表示
                    uiSwitch.SwitchSubUI((int)SubUIType.INTERACT, true);
                    return;
                }
            }
        }

    }
}
