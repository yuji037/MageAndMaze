using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightShot : SkillBase {
    
    public int rangeZ = 5;
    public BattleParticipant battleTarget;
    // 杖を振ったときに出るエフェクト
    GameObject swingEffect;
    [SerializeField] bool emitSwingEffect = true;
	
	// Update is called once per frame
	void Update () {
        if (shouldActionStart)
        {
            StartCoroutine(Coroutine());
            shouldActionStart = false;
        }
    }
    
    IEnumerator Coroutine()
    {
        anim.TriggerAnimator("Magic_forward");
        swingEffect = Resources.Load<GameObject>("EffectPrefab/magicswing_effect") as GameObject;
        yield return new WaitForSeconds(0.25f);
        var shotEff = Instantiate(effects[0]);
        if ( emitSwingEffect ) { var swingEff = Instantiate(swingEffect, transform.position, transform.rotation); }
        shotEff.transform.position = transform.position + player.charaDir * 0.5f;
        shotEff.transform.rotation = transform.rotation;
        var gso = shotEff.GetComponent<GoFlyingObject>();
        gso.SetTarget(hitPos);
        if (battleTarget) gso.target = battleTarget;
        gso.actionParent = this.gameObject;
        yield return null;
    }
    public override void SetTarget(Vector3 pos) // ※straightShot では posは使用されない
    {
        if ( rangeZ == 0 )
        {
            hitPos = player.pos;
            targetExistPos = player.pos;
        }
        for (int i = 1; i <= rangeZ; i++)
        {
            Vector3 targetP = player.pos + player.charaDir * i;
            int charaID = mapMn.chara_exist2D[(int)targetP.z, (int)targetP.x];
            int mapID = mapMn.dung_2D[(int)targetP.z, (int)targetP.x];

            if ( 200 <= charaID) //キャラがいたら
            {
                battleTarget = eneMn.GetEnemy(charaID);
                if ( !battleTarget ) battleTarget = obsMn.GetObstacle(charaID);
                hitPos = targetP;
                targetExistPos = targetP;
                return;
            }
            if (mapID == -1) //壁があったら
            {
                hitPos = targetP - player.charaDir * 0.5f;
                targetExistPos = targetP;
                return;
            }
            if (i == rangeZ)
            {
                hitPos = targetP;
                targetExistPos = targetP;
            }
        }
    }

    public override void OnDecided()
    {
        base.OnDecided();
        if ( range.Count > 0 )
        {
            foreach ( Vector2 ran in range )
            {
                Vector3 judgePos = targetExistPos + new Vector3(ran.x, 0, ran.y);
                int chara = mapMn.GetCharaExist((int)judgePos.x, (int)judgePos.z);
                if ( chara != -1 )
                {
                    BattleParticipant target = eneMn.GetEnemy(chara);
                    if ( !target ) target = obsMn.GetObstacle(chara);

                    if ( target )
                    {
                        target.perpetrator = player;
                        target.CauseElementEffect(element);
                    }
                }
            }
        }
        else if ( battleTarget )
        {
            battleTarget.perpetrator = player;
            battleTarget.CauseElementEffect(element);
        }
    }

    public virtual void HitAndParamChange()
    {
        if ( range.Count > 0 )
        {
            foreach ( Vector2 ran in range )
            {
                Vector3 judgePos = targetExistPos + new Vector3(ran.x, 0, ran.y);
                int chara = mapMn.GetCharaExist((int)judgePos.x, (int)judgePos.z);
                if ( chara != -1 )
                {
                    BattleParticipant target = eneMn.GetEnemy(chara);
                    if ( !target ) target = obsMn.GetObstacle(chara);

                    if ( target ) target.DamageParameter((int)calcPower, ParamType.HP, element, player);
                }
            }
        }
        else if (battleTarget)
        {
            battleTarget.DamageParameter((int)calcPower, ParamType.HP, element, player);
            battleTarget.UpdateAbnoEffect();
        }
        Debug.Log(battleTarget + " Hit");
        player.ActEnd();
        Destroy(gameObject);
    }

    public override List<Vector3> GetRange()
    {
        SetTarget(Vector3.zero);
        List<Vector3> rangeList = new List<Vector3>();
        if (range.Count != 0)   // 範囲魔法の場合
        {
            foreach (Vector2 ran in range)
            {
                Vector3 pos = targetExistPos + new Vector3(ran.x, 0, ran.y);
                rangeList.Add(pos);
            }
        }
        else rangeList.Add(targetExistPos);     // 着弾点のみ範囲の場合

        return rangeList;
    }
}
