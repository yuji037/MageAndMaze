using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSkill : MonoBehaviour
{

    [SerializeField]
    protected SkillBase.TYPE element = SkillBase.TYPE.NO_ELEMENT;
    [SerializeField]
    int rowPower = 3;
    protected float calcPower = 0;
    protected bool init = false;
    public BattleParticipant thisChara;
    public BattleParticipant target = null;
    public float timeCount = 0;
    public bool actionStart = false;

    public GameObject parent;
    public MapManager mapMn;
    protected EnemyManager eneMn;
    protected ObstacleManager obsMn;
    protected Player player;
    public AnimationChanger anim = null;

    [SerializeField]
    int rangeZ = 1;

    // フラグ：プレイヤーから遠い場合演出を簡略化する
    public bool isPlayPerformance { get
        {
            Vector3 dis = player.pos - thisChara.sPos;
            return dis.sqrMagnitude <= 4 * 4;
        } }

    // Use this for initialization
    protected virtual void Start()
    {

    }

    public virtual void Init()
    {
        parent = GameObject.Find("GameObjectParent");
        mapMn = parent.GetComponentInChildren<MapManager>();
        eneMn = parent.GetComponentInChildren<EnemyManager>();
        obsMn = parent.GetComponentInChildren<ObstacleManager>();
        player = parent.GetComponentInChildren<Player>();
        anim = thisChara.GetComponent<AnimationChanger>();
        init = true;

        AtkAndDef atkAndDef = thisChara.GetComponent<AtkAndDef>();
        // 自身の攻撃力などで補正
        if ( atkAndDef ) calcPower = thisChara.GetComponent<AtkAndDef>().CalcPower(element, rowPower);
        else calcPower = (int)Mathf.Round(rowPower);
    }

    public virtual void Prepare()
    {
        if ( !target ) SetTarget();
    }

    public virtual void TargetDamage()
    {
        // 計算した power を対象者へ
        if( target ) target.DamageParameter(calcPower, ParamType.HP, element);
    }

    public virtual void SetTarget()
    {
        obsMn = parent.GetComponentInChildren<ObstacleManager>();

        for ( int i = 1; i <= rangeZ; i++ )
        {
            Vector3 targetP = thisChara.pos + thisChara.charaDir * i;
            int charaID = mapMn.chara_exist2D[(int)targetP.z, (int)targetP.x];
            int mapID = mapMn.dung_2D[(int)targetP.z, (int)targetP.x];

            if ( 500 <= charaID ) //キャラがいたら
            {
                target = eneMn.GetEnemy(charaID);
                return;
            }
            else if ( 200 <= charaID && charaID < 500 ) // 障害物
            {
                target = obsMn.GetObstacle(charaID);
                return;
            }
            if ( mapID == -1 ) //壁があったら
            {
                return;
            }
            if ( i == rangeZ )
            {

            }
        }
    }

    public void ActEndImmediately()
    {
        thisChara.ActEnd();
        Destroy(gameObject);
    }
}

