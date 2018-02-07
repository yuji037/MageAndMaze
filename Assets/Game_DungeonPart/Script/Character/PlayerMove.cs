using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {

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

	// Use this for initialization
	void Start () {
        parent = GameObject.Find("GameObjectParent");
        player = parent.GetComponentInChildren<Player>();
        mapMn = parent.GetComponentInChildren<MapManager>();
        turnMn = parent.GetComponentInChildren<TurnManager>();
        moveBtMn = parent.GetComponentInChildren<MoveButtonManager>();
        anim = GetComponent<AnimationChanger>();
        uiSwitch = parent.GetComponentInChildren<UISwitch>();

        uiSwitch.SwitchSubUI((int)SubUIType.CHECK_NEXT_FLOOR, false);
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
    public void MoveUpdate () {

        if(moveVec != Vector3.zero )
        {
            Move();
        }
        //if (moveVec == Vector3.zero && resMoveDir != Vector3.zero) // 先行入力があったら
        //{
        //    if (!turnMn.PlayerActionSelected 
        //        && !MoveStart(resMoveDir))
        //        resMoveDir = Vector3.zero;
        //    if (moveVec != Vector3.zero)
        //    {
        //        //Move();
        //    }
        //}
        //if (player.action >= ActionType.ATTACK)
        //{
        //    resMoveDir = Vector3.zero;
        //}
    }

    float actionRate = 0;

    public void Move()
    {
        actionRate += Time.deltaTime * turnMn.moveSpeed;
        //Debug.Log("Player actionRate : " + actionRate);
        if(actionRate >= 1 )
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
        if ( turnMn.PlayerActionSelected ) return true;

        if ( player.abnoState.invincibleTurn > 0) return false;

        // 選択方向を向く
        player.charaDir = dir;
        player.SetObjectDir();

        // マップ外かどうか
        if ( !mapMn.InsideMap(player.pos + dir) ) return false;

        // 移動可能な空間かどうか
        if (!mapMn.CanMoveCheck(player.pos, player.pos + dir)) return false;

        moveBtMn._isActive = false;
        player.sPos = player.pos + dir;
        mapMn.SetCharaExistInfo(player.pos);
        mapMn.SetCharaExistInfo(player.sPos, charaID, true);

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
        if (moveVec != Vector3.zero) return; // 移動中は向きを変えない
        player.charaDir = dir;
        player.SetObjectDir();
    }

    public void FootCheck()
    {
        // 階段、水たまり、回復パネル等のIDチェック
        int onFootType = mapMn.onground_exist2D[(int)player.pos.z, (int)player.pos.x];
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
        if ( onFootType == 301 )
        {
            player.HealByPercent(0.1f);
        }
    }
}
