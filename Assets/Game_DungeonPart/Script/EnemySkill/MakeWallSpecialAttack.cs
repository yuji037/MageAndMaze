using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeWallSpecialAttack : NPCSkill
{
    [SerializeField]
    GameObject hitEff;
    [SerializeField]
    float hitTime = 0.5f;

    // Update is called once per frame
    void Update()
    {
        if ( anim == null ) return;
        if ( !actionStart ) return;

        StartCoroutine(Coroutine());
        actionStart = false;
    }

    IEnumerator Coroutine()
    {
        if ( isPlayPerformance )
        {
            anim.TriggerAnimator("Special");
            //thisChara.charaDir = target.pos - thisChara.sPos;
            //thisChara.SetObjectDir();

            yield return new WaitForSeconds(hitTime);

            // ヒット時点
            if ( target != null )
            {
                if ( hitEff )
                {
                    var hit = Instantiate(hitEff, target.transform.position, target.transform.rotation);
                    Destroy(hit, 2.0f);
                }
            }
        }
        // 岩ブロック置く
        obsMn = parent.GetComponentInChildren<ObstacleManager>();
        obsMn.AddObstacle(Obstacle.Type.ROCK, thisChara.pos + thisChara.charaDir);

        if ( isPlayPerformance ) yield return new WaitForSeconds(1);

        thisChara.ActEnd();
        Destroy(gameObject);
        yield return null;
    }
}
