using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherSpecialAttack : NPCSkill {

    float _needTime = 0;
    [SerializeField]
    float _arrowSpeed = 10.0f;
    [SerializeField]
    GameObject effect;
    MeshRenderer bodyArrow;

    [SerializeField]
    GameObject hitEff;

	// Use this for initialization
	protected override void Start () {

        base.Start();
        SetNeedTime();
	}
	
	// Update is called once per frame
	void Update () {
		if (actionStart)
        {
            StartCoroutine(Coroutine());
            actionStart = false;

            if ( thisChara.HP <= 0 )
            {
                StopCoroutine(Coroutine());
                Destroy(gameObject);
            }
        }
	}

    IEnumerator Coroutine()
    {
        int d_x = (int)( target.pos.x - thisChara.pos.x );
        int d_z = (int)( target.pos.z - thisChara.pos.z );
        if ( d_x != 0 ) d_x = ( d_x > 0 ) ? 1 : -1;
        if ( d_z != 0 ) d_z = ( d_z > 0 ) ? 1 : -1;
        thisChara.charaDir = new Vector3(d_x, 0, d_z);
        thisChara.SetObjectDir();

        if ( isPlayPerformance )
        {
            anim.TriggerAnimator("Special");
            yield return new WaitForSeconds(1.3f);
            bodyArrow = thisChara.GetComponentInChildren<Arrow>().GetComponent<MeshRenderer>();
            if ( bodyArrow ) bodyArrow.enabled = false;
            var arrow = Instantiate(effect, transform);

            parent.GetComponentInChildren<SESet>().PlaySE(SESet.Type.WIND_CUT);

            for ( float t = 0; t < _needTime; t += Time.deltaTime )
            {
                // 矢が飛んでく
                Vector3 dis = target.pos - thisChara.pos;
                arrow.transform.position = thisChara.pos + dis * ( t / _needTime );
                yield return null;
            }
        }
        // ターゲットにヒット
        // ダメージ
        TargetDamage();
        if ( isPlayPerformance && hitEff )
        {
            var hit = Instantiate(hitEff, target.transform.position, target.transform.rotation);
            Destroy(hit, 2.0f);
        }
        // 行動完了フラグ
        thisChara.ActEnd();
        // 後始末
        if ( isPlayPerformance )
        {
            yield return new WaitForSeconds(1);
            if ( bodyArrow ) bodyArrow.enabled = true;
        }
        Destroy(gameObject);
        yield return null;
    }

    public void SetNeedTime()
    {
        Vector3 dis = target.pos - thisChara.pos;
        _needTime = dis.magnitude / _arrowSpeed;
    }
}
