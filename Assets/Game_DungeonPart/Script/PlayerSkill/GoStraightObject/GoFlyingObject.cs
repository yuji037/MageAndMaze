using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoFlyingObject : MonoBehaviour
{
    float timeCount = 0;
    float needTime = 0;
    public Vector3 targetPos;
    Vector3 moveDir;
    GameObject parent;
    public GameObject actionParent;
    public BattleParticipant target;

    public float moveSpeed = 3.0f;
    [SerializeField]
    GameObject hitEffect;
    [SerializeField]
    float hitEffExistTime = 2.0f;
    [SerializeField] bool cameraShake = false;
    [SerializeField] float shakeMagnitude = 1.0f;
    CameraManager _camera;

    [SerializeField] Vector3 flyingCurveScale = new Vector3(0, 0, 0);

    // Use this for initialization
    void Start()
    {
        parent = GameObject.Find("GameObjectParent");
        if (cameraShake)
            _camera = parent.GetComponentInChildren<CameraManager>();
        StartCoroutine(MoveCoroutine());
    }

    IEnumerator MoveCoroutine()
    {
        if ( needTime == 0 )
        {
            // 起こり得ない
            needTime = 10;
            timeCount = 15;
        }
        Vector3 startPos = transform.position;
        for ( ; timeCount < needTime; timeCount += Time.deltaTime )
        {
            transform.position = startPos + moveDir * timeCount * moveSpeed + flyingCurveScale * Mathf.Sin((timeCount / needTime) * Mathf.PI);
            yield return null;
        }

        // ヒット時
        if ( cameraShake ) _camera.CameraShake(shakeMagnitude);
        actionParent.GetComponent<StraightShot>().HitAndParamChange();
        if ( hitEffect )
        {
            var hitEff = Instantiate(hitEffect);
            hitEff.transform.position = transform.position;
            if ( target ) hitEff.GetComponent<Seek>().target = target.gameObject;
            Destroy(hitEff, hitEffExistTime);
        }
        Destroy(this.gameObject);
    }

    public void SetTarget(Vector3 pos)
    {
        targetPos = pos;
        moveDir = pos - transform.position;
        if (moveSpeed == 0 )
        {
            needTime = 0;
            return;
        }
        needTime = moveDir.magnitude / moveSpeed;
        moveDir = moveDir.normalized;
    }
}
