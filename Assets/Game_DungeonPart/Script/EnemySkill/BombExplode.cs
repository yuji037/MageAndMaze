using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExplode : NPCSkill {

    [SerializeField] List<Vector2> range;
    [SerializeField] List<GameObject> effects;

    private void Start()
    {
        if ( range != null )
            StartCoroutine(CheckCoroutine());
    }

    IEnumerator CheckCoroutine()
    {
        while ( true )
        {
            if ( actionStart )
            {
                yield return StartCoroutine(Action());
            }
            yield return null;
        }
    }

    IEnumerator Action()
    {
        player = parent.GetComponentInChildren<Player>();
        Debug.Log("BombExplode実行");
        if (effects.Count > 0 )
        {
            var eff = Instantiate(effects[0], transform);
            eff.transform.position = thisChara.pos;
        }

        foreach(Vector2 ran in range )
        {
            Vector3 judgePos = thisChara.pos + new Vector3(ran.x, 0, ran.y);
            int chara = mapMn.chara_exist2D[(int)judgePos.z, (int)judgePos.x];
            BattleParticipant target = eneMn.GetEnemy(chara);
            if ( !target ) target = obsMn.GetObstacle(chara);
            if ( !target && player ) target = ( chara == player.idNum ) ? player : null;

            if ( target ) target.DamageParameter(calcPower, ParamType.HP, element, thisChara);
        }
        thisChara.ActEnd();
        yield return new WaitForSeconds(4);
        Destroy(gameObject);
        yield return null;
    }
}
