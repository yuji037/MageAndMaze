using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNormalAttack : NPCSkill {

    [SerializeField]
    GameObject hitEff;
    [SerializeField]
    float hitTime = 0.5f;
	
	// Update is called once per frame
	void Update () {
        if (anim == null) return;
        if (!actionStart) return;
        if ( !target )
        {
            ActEndImmediately();
            return;
        }

        StartCoroutine(Coroutine());
        actionStart = false;
    }

    IEnumerator Coroutine()
    {
        thisChara.charaDir = target.pos - thisChara.sPos;
        thisChara.SetObjectDir();
        if ( isPlayPerformance )
        {
            anim.TriggerAnimator("Attack");
            yield return new WaitForSeconds(hitTime);
        }
        // ヒット時点
        TargetDamage();
        if ( isPlayPerformance && hitEff )
        {
            var hit = Instantiate(hitEff, target.transform.position, target.transform.rotation);
            Destroy(hit, 2.0f);
        }
        if ( isPlayPerformance ) yield return new WaitForSeconds(0.5f);

        thisChara.ActEnd();
        Destroy(gameObject);
        yield return null;
    }
}
