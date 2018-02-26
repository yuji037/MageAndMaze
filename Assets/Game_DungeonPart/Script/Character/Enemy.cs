using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    NORMAL,
    ARCHER,
    UFO,
    TREASURE,
    SWORD,
    GHOST,
    MAKEWALL,
    LIGHT,
    BOSS1,
}

public class Enemy : BattleParticipant
{
    // エネミーのステータス、状態異常、行動選択を管理

    public EnemyType type = 0;
    public bool isSpeakable = false;
    AImove thisAImove = null;
    protected List<GameObject> myBodys = new List<GameObject>();

    // 倍速モンスターは 0.5f
    float needActGauge = 1;

    [SerializeField]
    List<GameObject> skills;
    // 行動選択してターンが来るまでスキルを準備しておくリスト
    List<GameObject> prepareSkills;
    List<ActionData> thisTurnAction = new List<ActionData>();
    AnimationChanger anim = null;
    SpecialActAI spAI;

    public int RewardExp = 0;

    DungeonPartManager dMn;

    public override bool isPlayPerformance
    {
        get
        {
            if ( prepareSkills[0] )
            {
                return prepareSkills[0].GetComponent<NPCSkill>().isPlayPerformance;
            }
            return true;
        }
    }

    public override void Init()
    {
        base.Init();

        thisAImove = GetComponent<AImove>();
        thisAImove.Init();

        Body[] bodys = GetComponentsInChildren<Body>();
        foreach ( Body body in bodys )
        {
            myBodys.Add(body.gameObject);
        }

        spAI = GetComponent<SpecialActAI>();
        if ( spAI ) spAI.Init();
        if ( type == EnemyType.SWORD )
        {
            needActGauge = 0.5f;
        }
        prepareSkills = new List<GameObject>();

        anim = GetComponent<AnimationChanger>();
        anim.Init();

        dMn = parent.GetComponentInChildren<DungeonPartManager>();
    }

    public void PlusActionGauge()
    {
        float plusActionGauge = 1;
        // プレイヤー倍速時
        if ( player.abnoState.spdUpTurn > 0 ) plusActionGauge = 0.5f;

        actionGauge += plusActionGauge;

        // 行動選択に影響を及ぼす状態異常の処理
        if ( abnoState.freezeTurn > 0 )
        {
            actionGauge = 0;
            return;
        }
        if ( abnoState.paralizeTurn > 0 )
        {
            actionGauge -= 0.5f;
        }

    }

    public void SelectAction()
    {
        int sequence = 0;

        // 倍速モンスターなどの行動後半かどうか
        if ( needActGauge <= 0.5f && actionGauge <= needActGauge * 1 )
        {
            sequence = 1;
        }

        // actionGauge を必要分消費して行動可能とする
        if ( actionGauge < needActGauge )
        {
            // 行動不可
            pos = transform.position;
            return;
        }
        else actionGauge -= needActGauge;




        if ( isSpeakable )
        {
            // NPCなら何もせず
            return;
        }

        // 特殊行動の判定
        bool specialAct = false;
        if (spAI != null)
        {
            specialAct = spAI.ShouldSpecialAct();
            if (specialAct)
            {
                Debug.Log("特殊行動しました");
                if (!skills[1])
                {
                    // 特殊攻撃持たないのに特殊攻撃選択
                    Debug.Log("エラー：特殊攻撃スキルが設定されていません");
                    return;
                }
                var sp = Instantiate(skills[1], transform);
                prepareSkills.Add(sp);
                NPCSkill speSkill = sp.GetComponent<NPCSkill>();
                speSkill.thisChara = this;
                speSkill.target = GetComponent<SpecialActAI>().targetChara;
                speSkill.Init();
                speSkill.Prepare();
                ActionData addAction = new ActionData(this, ActionType.ENEMY_SPECIAL, 0, sequence, Vector3.zero);
                thisTurnAction.Add(addAction);
                turnMn.AddAction(addAction);
            }
        }

        if (!specialAct) // falseだったので移動または通常攻撃を選択
        {
            // 付属のAImoveが通常攻撃すべきと判断すれば false それ以外なら true
            if (thisAImove.GetMoveVec() && !mapMn.IsBreakableObstacle(pos + moveVec))
            {
                if (moveVec == Vector3.zero )
                {
                    // 何もしない
                    return;
                }
                // 移動
                sPos = pos + moveVec;
                charaDir = moveVec;
                mapMn.SetCharaExistInfo(pos);
                mapMn.SetCharaExistInfo(sPos, idNum, true);
                // 倍速時の一手先行動選択のため pos に sPos を入れる
                pos = sPos;
                ActionData addAction = new ActionData(this, ActionType.MOVE, 0, sequence, sPos);
                thisTurnAction.Add(addAction);
                turnMn.AddAction(addAction);
            }
            else // falseだったので通常攻撃を選択
            {
                if ( !skills[0] )
                {
                    Debug.Log("エラー：通常攻撃スキルが設定されていません");
                    return;
                }
                var atk = Instantiate(skills[0], transform);
                prepareSkills.Add(atk);
                var norSkill = atk.GetComponent<NPCSkill>();
                norSkill.thisChara = this;
                norSkill.target = target;
                norSkill.Init();
                norSkill.Prepare();
                ActionData addAction = new ActionData(this, ActionType.ATTACK, 0, sequence, Vector3.zero);
                thisTurnAction.Add(addAction);
                turnMn.AddAction(addAction);
            }
        }
        transform.LookAt(pos + charaDir);
        ParamChangeByTurn();
    }

    public override void CharaUpdate()
    {
        base.CharaUpdate();

        DeathCheck();
        if ( thisAImove == null ) thisAImove = GetComponent<AImove>();
        //if (anim == null) anim = GetComponent<AnimationChanger>();
        if ( thisTurnAction.Count <= 0 ) return;
        if ( !thisTurnAction[0].allowed ) return;

        // 行動処理
        if ( HP <= 0 )
        {
            ActEnd();
            return;
        }
        if ( !actStarted )
        {
            Debug.Log(this + "行動開始");
        }
        switch ( thisTurnAction[0].actType )
        {
            case ActionType.NON_ACTION:
                ActEnd();
                break;
            case ActionType.MOVE:
                ActionData act = thisTurnAction[0];

                // 1ターンに2回移動するなら倍速になる
                if ( thisTurnAction.Count >= 2
                && thisTurnAction[1].actType == ActionType.MOVE
                && thisTurnAction[1].allowed )
                {
                    act.speedRate = 2;
                    thisTurnAction[1].speedRate = 2;
                }
                // プレイヤーからかなり遠い所でのActionなら描写を早く終わらせる
                if ( act.actionRate == 0 )
                {
                    Vector3 disToPlayer = act.pos - player.pos;
                    if ( disToPlayer.sqrMagnitude > 10 * 10 )
                    {
                        act.speedRate = 100;
                    }
                    anim.TriggerAnimator("Move");
                    actStarted = true;
                }
                // 経時変化
                act.actionRate += Time.deltaTime * turnMn.moveSpeed * act.speedRate;
                //Debug.Log("Enemy actionRate : " + act.actionRate);
                if ( act.actionRate >= 1 )
                {
                    // 移動終了
                    act.finish = true;
                    transform.position = act.pos;
                    pos = act.pos;
                    anim.TriggerAnimator("Move", false);
                    ActEnd();
                }
                else
                {
                    // 移動の途中
                    Vector3 distance = ( act.pos - pos ) * act.actionRate;
                    transform.position = pos + distance;
                }
                break;
            case ActionType.ATTACK:
                if ( actStarted ) return;
                prepareSkills[0].GetComponent<NPCSkill>().actionStart = true;
                actStarted = true;
                break;
            case ActionType.ENEMY_SPECIAL:
                if ( actStarted ) return;
                prepareSkills[0].GetComponent<NPCSkill>().actionStart = true;
                actStarted = true;
                break;
            default:
                break;
        }
    }
    public override void UpdateAbnoEffect()
    {
        // 凍結
        if ( abnoState.freezeTurn > 0 && abnoEffect[0] == null )
        {
            abnoEffect[0] = Instantiate(parent.GetComponentInChildren<AbnormalEffect>().abnoEffect[0]);
            abnoEffect[0].transform.position = this.transform.position;
            abnoEffect[0].transform.parent = this.transform;
            Debug.Log("Freeze!");
        }
        if ( abnoState.freezeTurn <= 0 )
        {
            if ( abnoEffect[0] ) Destroy(abnoEffect[0]);
        }

        // 感電
        if ( abnoState.paralizeTurn > 0 && abnoEffect[1] == null )
        {
            abnoEffect[1] = Instantiate(parent.GetComponentInChildren<AbnormalEffect>().abnoEffect[1]);
            abnoEffect[1].transform.position = this.transform.position;
            abnoEffect[1].transform.parent = this.transform;
            Debug.Log("Paralize!");
        }
        if ( abnoState.paralizeTurn <= 0 )
        {
            if ( abnoEffect[1] ) Destroy(abnoEffect[1]);
        }
    }

    public override void UpdateAbnoParam()
    {
        if ( abnoState.freezeTurn > 0 ) abnoState.freezeTurn--;

        if ( abnoState.paralizeTurn > 0 ) abnoState.paralizeTurn--;
    }

    protected override void DeathCheck()
    {
        if ( HP <= 0 && isAlive )
        {
            KillReward();
            mapMn.SetCharaExistInfo(sPos);
            foreach ( GameObject deadObj in deadObjPrefab )
            {
                var obj = Instantiate(deadObj);
                obj.transform.position = transform.position;
                obj.transform.rotation = transform.rotation;
                Destroy(obj, 5.0f);
            }
            foreach ( GameObject aliveBody in myBodys )
            {
                Destroy(aliveBody);
            }
            parent.GetComponentInChildren<EnemyManager>().EmitDeathEffect(transform.position);
            isAlive = false;
        }
    }

    void KillReward()
    {
        if ( type == EnemyType.LIGHT ) return;

        Player player = parent.GetComponentInChildren<Player>();
        player.ExpGet(RewardExp);
        ItemGet itemGet = parent.GetComponentInChildren<ItemGet>();

        float plus = 1 + (dMn.floor / 6.0f);
        if ( type == EnemyType.TREASURE ) plus *= 10;

        itemGet.AcquireSoulStone(0, plus);
        itemGet.AcquireSoulStone(1, plus);
        itemGet.AcquireSoulStone(2, plus);
    }

    public override void ActEnd()
    {
        if ( thisTurnAction[0].actType >= ActionType.ATTACK )
        {
            if ( prepareSkills[0] )
                prepareSkills.RemoveAt(0);
        }
        thisTurnAction[0].finish = true;
        thisTurnAction.RemoveAt(0);

        actStarted = false;
        actAllowed = false;

    }
    
}