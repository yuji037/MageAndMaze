using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttack : SkillBase {

    private IEnumerator _coroutine;
    BattleParticipant battleTarget;

	// Update is called once per frame
	void Update () {
		if (shouldActionStart)
        {
            StartCoroutine(Coroutine());
            shouldActionStart = false;
        }
	}

    private IEnumerator Coroutine()
    {
        anim.TriggerAnimator("Attack");
        yield return null;

        var swingSE = Instantiate(effects[0], transform);
        swingSE.transform.position = transform.position;
        Destroy(swingSE, 2.0f);
        yield return null;
        yield return new WaitForSeconds(0.1f);
        if ( battleTarget )
        {
            var hitEff = Instantiate(effects[1], transform);
            hitEff.transform.position = battleTarget.transform.position;
            Destroy(hitEff, 2.0f);
        }
        HitAndParamChange();
        yield return new WaitForSeconds(0.7f);
        player.ActEnd();
        Destroy(gameObject, 2.0f);
        yield return null;

    }

    public override void SetTarget(Vector3 pos) // しかしposは使用されない
    {
        Vector3 targetP = player.pos + player.charaDir;
        int mapChip = mapMn.dung_2D[(int)targetP.z, (int)targetP.x];
        int chara = mapMn.chara_exist2D[(int)targetP.z, (int)targetP.x];

        if ( mapChip == -1) //壁があったら
        {
            hitPos = targetP - player.charaDir * 0.5f;
            return;
        }
        if ( chara != 0 ) //キャラがいたら
        {
            battleTarget = eneMn.GetEnemy(chara);
            if ( !battleTarget ) battleTarget = obsMn.GetObstacle(chara);
            hitPos = targetP;
            return;
        }
        hitPos = targetP;
    }

    public void HitAndParamChange()
    {
        if (battleTarget)
        {
            battleTarget.DamageParameter((int)rowPower, ParamType.HP, element, player);
            OnDecided();
        }
    }
}
