﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーのステータス、状態異常の管理
/// ターン制キャラとしても振る舞う
/// </summary>
public class Player : BattleParticipant {

    // プレイヤーのステータス、状態異常を管理

    public int MP;
    public int MaxMP;
    public int Stamina;

    public int Exp = 0;
    ExpTable expTable = new ExpTable();

    float hpRegene = 0;
    float mpRegene = 0;

    AnimationChanger anim = null;

    DungeonPartManager dMn;
    PlayerItem playerItem;
    PlayCharaSwitch playCharaSwitch;
    [SerializeField]
    Light _plSpotLight;
    [SerializeField]
    Light _plPointLight;

    public List<ActionData> thisTurnAction = new List<ActionData>();

    [SerializeField] GameObject effect_LevelUp;

    public override void Init()
    {
        idNum = 1;
        base.Init();
        //Time.timeScale = 2;
        ExpGet(0);
        UpdateLightRange();
        anim = GetComponent<AnimationChanger>();
        dMn = parent.GetComponentInChildren<DungeonPartManager>();
        playerItem = GetComponent<PlayerItem>();
        playCharaSwitch = parent.GetComponentInChildren<PlayCharaSwitch>();
    }

    public IEnumerator ToGameoverScene()
    {
        dMn.SaveDataReset();
        anim.TriggerAnimator("Dead");
        yield return new WaitForSeconds(2);
        var sceneTransitionManager = parent.GetComponentInChildren<SceneTransitionManager>();
        if ( sceneTransitionManager )
            yield return StartCoroutine(sceneTransitionManager.FadeOut());
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
    }

    public override void DamageParameter(float value
    , ParamType para = ParamType.HP, SkillBase.TYPE element = SkillBase.TYPE.NO_ELEMENT, BattleParticipant attacker = null)
    {
        int damage = atkAndDef.CalcDamage(element, value);
        switch (para)
        {
            case ParamType.HP:
                HP -= damage;
                break;
            case ParamType.MP:
                MP -= damage;
                break;
            case ParamType.STAMINA:
                Stamina -= damage;
                break;
            default:
                break;
        }
        if ( HP < 0 ) HP = 0;
        anim.TriggerAnimator("Damaged");
        dmgEffMn.CreateEffectText(sPos, damage);
    }

    int preLevel = 0;

    public void ExpGet(int point)
    {
        int calcPoint = Mathf.RoundToInt(point * ( playCharaSwitch.isAngel ? 1.3f : 0.7f ));
        Debug.Log("獲得経験値 " + calcPoint);

        preLevel = Level;
        Exp += calcPoint;
        Level = expTable.GetLevel(Exp);
        if (preLevel != Level)
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        MaxHP = expTable.GetHP(Level);
        MaxMP = expTable.GetMP(Level);
        StartCoroutine(LevelUpEffect());
    }

    IEnumerator LevelUpEffect()
    {
        effect_LevelUp.SetActive(true);
        anim.TriggerAnimator("Pleasure");
        yield return new WaitForSeconds(1);

    }

    public bool MpUseSkill(int useMp)
    {
        if (MP < useMp) return false;
        MP -= useMp;
        return true;
    }
    public override void ParamChangeByTurn()
    {
        hpRegene += MaxHP * 0.02f;
        mpRegene += MaxMP * 0.06f;

        float hpRege = Mathf.Floor(hpRegene);
        float mpRege = Mathf.Floor(mpRegene);

        HP = Mathf.Min(MaxHP, HP + (int)hpRege);
        MP = Mathf.Min(MaxMP, MP + (int)mpRege);

        hpRegene -= hpRege;
        mpRegene -= mpRege;
        base.ParamChangeByTurn();
    }
    public void LoadPlayerInfo()
    {
        parent = GameObject.Find("GameObjectParent");

        HP = SaveData.GetInt("HP", 15);
        MaxHP = SaveData.GetInt("MaxHP", 15);
        MP = SaveData.GetInt("MP", 15);
        MaxMP = SaveData.GetInt("MaxMP", 15);
        Exp = SaveData.GetInt("EXP", 0);
        Level = SaveData.GetInt("Level", 1);
        Stamina = SaveData.GetInt("Stamina", 100);
        if ( !atkAndDef ) atkAndDef = GetComponent<AtkAndDef>();
        atkAndDef.LoadValue("PlayerAtkAndDef");
        if ( !playCharaSwitch ) playCharaSwitch = parent.GetComponentInChildren<PlayCharaSwitch>();
        playCharaSwitch.LoadChara();
    }

    public void SavePlayerInfo()
    {
        SaveData.SetInt("HP", HP);
        SaveData.SetInt("MaxHP", MaxHP);
        SaveData.SetInt("MP", MP);
        SaveData.SetInt("MaxMP", MaxMP);
        SaveData.SetInt("EXP", Exp);
        SaveData.SetInt("Level", Level);
        SaveData.SetInt("Stamina", Stamina);
        SavePlayerPos();
        playerItem.SaveItemData();
        atkAndDef.SaveValue("PlayerAtkAndDef");
        playCharaSwitch.SaveChara();
    }
    
    public class PosData
    {
        public int PosX;
        public int PosZ;
        public int DirX;
        public int DirZ;
    }

    public void SavePlayerPos()
    {
        PosData _data = new PosData();
        _data.PosX = (int)pos.x;
        _data.PosZ = (int)pos.z;
        _data.DirX = (int)charaDir.x;
        _data.DirZ = (int)charaDir.z;
        SaveData.SetClass("PlayerPosData", _data);
    }

    public void UpdateLightRange()
    {
        _plSpotLight.range = 23;
        StartCoroutine(ChangeLightRange());
    }

    IEnumerator ChangeLightRange()
    {
        while ( true )
        {
            bool isPlayerInRoom = (existRoomNum < mapMn.max_room);
            float postSpotLightAngle = isPlayerInRoom ? 50 : 35;
            float postPointLightRange = isPlayerInRoom ? 7 : 6;

            float diff = postPointLightRange - _plSpotLight.spotAngle;
            if ( Mathf.Abs(diff) < 0.1f ) yield break;

            _plSpotLight.spotAngle = Mathf.Lerp(_plSpotLight.spotAngle, postSpotLightAngle, 0.1f);
            _plPointLight.range = Mathf.Lerp(_plPointLight.range, postPointLightRange, 0.1f);
            yield return null;
        }

    }

    public void DebugExpGet()
    {
        ExpGet(300);
    }

    public void DebugHeal()
    {
        HP = MaxHP;
        MP = MaxMP;
    }

    public void DebugSoulStone()
    {
        ItemGet itemGet = GetComponent<ItemGet>();
        itemGet.AcquireSoulStone(0, Random.Range(1, 30));
        itemGet.AcquireSoulStone(1, Random.Range(1, 30));
        itemGet.AcquireSoulStone(2, Random.Range(1, 30));
    }

    public void PlayerActSelect()
    {
        ParamChangeByTurn();
        UpdateLightRange();
        turnMn.PlayerActSelect();
    }

    public override void ActEnd()
    {
        nowAct = null;
        action = ActionType.NON_ACTION;
        thisTurnAction[0].finish = true;
        thisTurnAction.RemoveAt(0);

        UpdateAbnoParam();
        UpdateAbnoEffect();
        Debug.Log("プレイヤー行動終了");
    }

    PlayerSkillTree pst = null;

    public void SaveSkill()
    {
        if ( !pst ) pst = GetComponent<PlayerSkillTree>();
        pst.SaveSkillData();
    }

    public void ElementLevelUp(int type)
    {

        switch( (SkillBase.TYPE) type )
        {
            case SkillBase.TYPE.FLAME:
                atkAndDef.FlameMagicPower *= 1.1f;
                break;
            case SkillBase.TYPE.LIGHTNING:
                atkAndDef.LightMagicPower *= 1.1f;
                break;
            case SkillBase.TYPE.ICE:
                atkAndDef.IceMagicPower *= 1.1f;
                break;
        }
    }

    public override void UpdateAbnoEffect()
    {
        // 凍結
        if ( abnoState.freezeTurn > 0 && abnoEffect[0] == null )
        {
            abnoEffect[0] = Instantiate(parent.GetComponentInChildren<AbnormalEffect>().abnoEffect[0]);
            abnoEffect[0].transform.position = this.transform.position;
            abnoEffect[0].transform.parent = this.transform;
            Debug.Log("Freeze!");
        }
        if ( abnoState.freezeTurn <= 0 )
        {
            if ( abnoEffect[0] ) Destroy(abnoEffect[0]);
        }

        // 感電
        if ( abnoState.paralizeTurn > 0 && abnoEffect[1] == null )
        {
            abnoEffect[1] = Instantiate(parent.GetComponentInChildren<AbnormalEffect>().abnoEffect[1]);
            abnoEffect[1].transform.position = this.transform.position;
            abnoEffect[1].transform.parent = this.transform;
            Debug.Log("Paralize!");
        }
        if ( abnoState.paralizeTurn <= 0 )
        {
            if ( abnoEffect[1] ) Destroy(abnoEffect[1]);
        }

        // 攻撃力アップ
        if ( abnoState.atkUpTurn > 0 && abnoEffect[2] == null )
        {
            abnoEffect[2] = Instantiate(parent.GetComponentInChildren<AbnormalEffect>().abnoEffect[2]);
            abnoEffect[2].transform.position = this.transform.position;
            abnoEffect[2].transform.parent = this.transform;
            Debug.Log("AttackUp!");
        }
        if ( abnoState.atkUpTurn <= 0 )
        {
            if ( abnoEffect[2] ) Destroy(abnoEffect[2]);
        }

        // 無敵化
        if ( abnoState.invincibleTurn > 0 && abnoEffect[3] == null )
        {
            abnoEffect[3] = Instantiate(parent.GetComponentInChildren<AbnormalEffect>().abnoEffect[3]);
            abnoEffect[3].transform.position = this.transform.position;
            abnoEffect[3].transform.parent = this.transform;
            Debug.Log("InvincibleTime!");
        }
        if ( abnoState.invincibleTurn <= 0 )
        {
            if ( abnoEffect[3] ) Destroy(abnoEffect[3]);
        }

        // 倍速
        if ( abnoState.spdUpTurn > 0 && abnoEffect[4] == null )
        {
            abnoEffect[4] = Instantiate(parent.GetComponentInChildren<AbnormalEffect>().abnoEffect[4]);
            abnoEffect[4].transform.position = this.transform.position;
            abnoEffect[4].transform.parent = this.transform;
            Debug.Log("SpeedUp!");
        }
        if ( abnoState.spdUpTurn <= 0 )
        {
            if ( abnoEffect[4] ) Destroy(abnoEffect[4]);
        }
    }

    public override void UpdateAbnoParam()
    {
        if ( abnoState.freezeTurn > 0 ) abnoState.freezeTurn--;
        if ( abnoState.paralizeTurn > 0 ) abnoState.paralizeTurn--;
        if ( abnoState.atkUpTurn > 0 ) abnoState.atkUpTurn--;
        if ( abnoState.defUpTurn > 0 ) abnoState.defUpTurn--;
        if ( abnoState.invincibleTurn > 0 ) abnoState.invincibleTurn--;
        if ( abnoState.spdUpTurn > 0 ) abnoState.spdUpTurn--;
    }
}

