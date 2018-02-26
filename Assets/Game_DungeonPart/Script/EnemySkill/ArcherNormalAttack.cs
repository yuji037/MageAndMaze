using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherNormalAttack : NPCSkill
{
    [SerializeField]
    MeshRenderer arrow;
    [SerializeField]
    GameObject hitEff;
    [SerializeField]
    float hitTime = 1.0f;

    // Update is called once per frame
    void Update()
    {
        if ( anim == null ) return;
        if ( !actionStart ) return;
        if ( !target )
        {
            ActEndImmediately();
            return;
        }

        StartCoroutine(Coroutine());
        actionStart = false;

        if(thisChara.HP <= 0 )
        {
            StopCoroutine(Coroutine());
            Destroy(gameObject);
        }
    }

    IEnumerator Coroutine()
    {
        thisChara.charaDir = target.pos - thisChara.sPos;
        thisChara.SetObjectDir();
        if ( isPlayPerformance )
        {
            //anim.TriggerAnimator("Attack");
            thisChara.GetComponent<Animator>().CrossFade("Attack", 0.3f, 0, 0.5f);
            yield return new WaitForSeconds(hitTime);
        }
        // ヒット時点
        TargetDamage();
        if ( isPlayPerformance && hitEff )
        {
            var hit = Instantiate(hitEff, target.transform.position, target.transform.rotation);
            Destroy(hit, 2.0f);
        }
        if ( isPlayPerformance )
        {
            // 矢がターゲットに刺さった後、弓に戻るまで消す必要がある
            arrow = thisChara.GetComponentInChildren<Arrow>().GetComponent<MeshRenderer>();
            if ( arrow ) arrow.enabled = false;
            yield return new WaitForSeconds(0.3f);
        }
        thisChara.ActEnd();
        if ( isPlayPerformance )
        {
            yield return new WaitForSeconds(0.2f);
            if ( arrow ) arrow.enabled = true;
        }
        Destroy(gameObject);
        yield return null;
    }
}
