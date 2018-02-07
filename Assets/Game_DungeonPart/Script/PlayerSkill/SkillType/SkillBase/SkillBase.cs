using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBase : MonoBehaviour{
    
    public List<Vector2> range;
    public int useMp = 0;
    [SerializeField]
    protected float rowPower = 0;
    protected float calcPower = 0;
    [SerializeField]
    protected float abnoEffectPower = 0;
    public bool shouldActionStart = false;

    public enum TYPE
    {
        NO_ELEMENT,
        FLAME,
        LIGHTNING,
        ICE
    }
    [SerializeField]
    protected TYPE element;

    protected GameObject parent;
    protected GameObject playerObj;
    protected Player player;
    protected PlayerMove playerMove;
    protected MapManager mapMn;
    protected AnimationChanger anim;
    protected CameraManager cameraMn;
    protected RawImage icon;
    [SerializeField] protected List<GameObject> effects;
    protected Vector3 hitPos; // 当たる予定の場所
    protected Vector3 targetExistPos;  // 当たる対象の居る場所
    protected float timeCount = 0;
    protected TurnManager turnMn;

    protected EnemyManager eneMn;
    protected ObstacleManager obsMn;


    /// <summary>
    /// この行動を選択した段階で呼ぶ関数（未決定）
    /// </summary>
    public void OnSelected()
    {
        parent = GameObject.Find("GameObjectParent");
        player = parent.GetComponentInChildren<Player>();
        playerObj = player.gameObject;
        playerMove = parent.GetComponentInChildren<PlayerMove>();
        mapMn = parent.GetComponentInChildren<MapManager>();
        anim = playerObj.GetComponent<AnimationChanger>();
        cameraMn = parent.GetComponentInChildren<CameraManager>();
        turnMn = parent.GetComponentInChildren<TurnManager>();
        eneMn = parent.GetComponentInChildren<EnemyManager>();
        obsMn = parent.GetComponentInChildren<ObstacleManager>();

        calcPower = player.GetComponent<AtkAndDef>().CalcPower(element, rowPower);
        SetTarget(Vector3.zero);
    }

    private void Update()
    {

    }

    public virtual void SetTarget(Vector3 pos)
    {

    }

    /// <summary>
    /// この行動に決定した時呼ぶ関数
    /// </summary>
    public virtual void OnDecided()
    {
        GetRange();
    }

    public virtual List<Vector3> GetRange()
    {
        Debug.Log("return null");
        return null;
    }
}
