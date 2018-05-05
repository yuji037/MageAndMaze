using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleParticipant : MonoBehaviour {

    // HP、状態異常、行動を管理
    // TurnManager に従って行動する
    // 破壊可能な爆発オブジェクトなどもこれで管理する

    public int idNum;
    protected bool isAlive = true;
    public bool IsAlive { get { return isAlive; } }
    public bool init = false;
    // 移動前の位置
    public Vector3 pos;
    // 移動後の位置
    public Vector3 sPos;
    public int Level;
    public int HP;
    public int MaxHP;
    public ActionType action = ActionType.NON_ACTION;
    public int skillNum = -1;
    [SerializeField] public float actionGauge = 0;
    Gauge3D hpGauge;
    [SerializeField]
    float hpGaugeShowTimeMax = 2.0f;
    public Vector3 moveVec;
    protected Vector3 moveDir;
    public Vector3 charaDir = new Vector3(0, 0, -1);
    protected GameObject parent;
    protected MapManager mapMn;
    protected TurnManager turnMn;
    [SerializeField] public int existRoomNum { get { return mapMn.dung_room_info2D[(int)sPos.z, (int)sPos.x]; } }
    [SerializeField] protected ActionData nowAct;
    public BattleParticipant target;
    public bool actStarted = false;
    public AtkAndDef atkAndDef = null;
    public AbnormalState abnoState = new AbnormalState();
    protected GameObject[] abnoEffect = new GameObject[5];
    [SerializeField] protected List<GameObject> deadObjPrefab;

    protected EffectTextManager dmgEffMn;

    protected Player player;

    // 自分を攻撃したキャラを覚えておく
    public BattleParticipant perpetrator;

    public virtual bool isPlayPerformance { get
        {
            return true;
        } }

    /// <summary>
    /// ID, pos, transform.pos の設定
    /// </summary>
    /// <param name="_num"></param>
    /// <param name="_pos"></param>
    public void SetStartParam(int _num, Vector3 _pos)
    {
        idNum = _num;
        pos = _pos;
        init = false;

        transform.position = _pos;
    }

    IEnumerator HPGaugeShow()
    {
        hpGauge.isActive = true;
        yield return new WaitForSeconds(hpGaugeShowTimeMax);
        hpGauge.isActive = false;
    }

    public virtual void CharaUpdate () {
		if(false == init)
        {
            Init();
        }
	}

    public virtual void DamageParameter( float value
        , ParamType para = ParamType.HP, SkillBase.TYPE element = SkillBase.TYPE.NO_ELEMENT, BattleParticipant attacker = null)
    {
        perpetrator = attacker;

        int damage;
        // 属性耐性等の計算があれば実行
        if ( atkAndDef ) damage = atkAndDef.CalcDamage(element, value);
        else damage = (int)Mathf.Round(value);

        switch (para)
        {
            case ParamType.HP:
                HP -= damage;
                //if (hp <= 0) action = ActionType.DEAD;
                if (hpGauge) StartCoroutine(HPGaugeShow());
                break;
            default:
                break;
        }
        dmgEffMn.CreateEffectText(pos, damage);
        DeathCheck();
    }

    public virtual void Kill(bool emitDeathEffect = true)
    {
        HP = 0;
        DeathCheck(emitDeathEffect);
    }

    public virtual void Init()
    {
        if ( hpGauge = GetComponentInChildren<Gauge3D>() ) { hpGauge.chara = this; }
        atkAndDef = GetComponent<AtkAndDef>();

        parent = GameObject.Find("GameObjectParent");
        mapMn = parent.GetComponentInChildren<MapManager>();
        turnMn = parent.GetComponentInChildren<TurnManager>();
        dmgEffMn = parent.GetComponentInChildren<EffectTextManager>();
        pos = transform.position;
        sPos = pos;
        if ( hpGauge ) hpGauge.isActive = false;
        mapMn.SetCharaExistInfo(sPos, idNum, true);

        player = parent.GetComponentInChildren<Player>();
        init = true;
    }

    public virtual void ParamChangeByTurn()
    {

    }

    public virtual void UpdateAbnoEffect()
    {

    }

    public virtual void UpdateAbnoParam()
    {

    }

    public void SetObjectDir()
    {
        transform.LookAt(pos + charaDir);
    }

    public virtual bool ActStart(int _turnActNum, ActionData _data)
    {
        return true;
    }
    public virtual void ActEnd()
    {

    }

    protected virtual void DeathCheck(bool emitDeathEffect = true)
    {
        if ( HP <= 0 && isAlive )
        {
            mapMn.SetCharaExistInfo(sPos);
            //foreach ( GameObject aliveBody in myBodys )
            //{
            //    Destroy(aliveBody);
            //}
            isAlive = false;
        }
    }

    public void HealByPercent(float rate)
    {
        int healValue = (int)Mathf.Floor(MaxHP * rate);
        Debug.Log("Heal : " + healValue);
        HP = Mathf.Min(HP + healValue, MaxHP);
    }

    public void HealByValue(int value)
    {
        HP = Mathf.Min(HP + value, MaxHP);
    }
    
    /// <summary>
    /// 属性攻撃を受けた時の反応処理
    /// </summary>
    public virtual void CauseElementEffect(SkillBase.TYPE type)
    {

    }

    public void CauseAbnormalState(AbnormalStateType type, float effectValue)
    {
        int calcTurn = 0;
        if ( atkAndDef )
        {
            calcTurn = atkAndDef.CalcAbnormalTurn(type, effectValue);
        }
        else calcTurn = Mathf.FloorToInt(effectValue);

        switch ( type )
        {
            case AbnormalStateType.FREEZE:
                abnoState.freezeTurn = calcTurn;
                break;
            case AbnormalStateType.PARALIZE:
                abnoState.paralizeTurn = calcTurn;
                break;
        }
    }
}
