using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkAndDef : MonoBehaviour {

    GameObject parent;
    BattleParticipant thisChara;
    MapManager mapMn;
    OnGroundObjectManager ogoMn;

    [SerializeField]
    float base_magic_power = 1.0f;
    public float BaseMagicPower { get { return base_magic_power; } set { base_magic_power = value; } }

    [SerializeField]
    float normal_power = 1.0f;
    public float NormalPower { get { return normal_power; } set { normal_power = value; } }
    [SerializeField]
    float flame_power = 1.0f;
    public float FlameMagicPower { get { return flame_power; } set { flame_power = value; } }
    [SerializeField]
    float light_power = 1.0f;
    public float LightMagicPower { get { return light_power; } set { light_power = value; } }
    [SerializeField]
    float ice_power = 1.0f;
    public float IceMagicPower { get { return ice_power; } set { ice_power = value; } }

    [SerializeField]
    float normal_resist = 1.0f;
    [SerializeField]
    float flame_resist = 1.0f;
    [SerializeField]
    float light_resist = 1.0f;
    [SerializeField]
    float ice_resist = 1.0f;

    [SerializeField]
    float freeze_resist = 1.0f;
    [SerializeField]
    float paralize_resist = 1.0f;

    // Use this for initialization
    void Start () {
        parent = GameObject.Find("GameObjectParent");
        thisChara = GetComponent<BattleParticipant>();
        mapMn = parent.GetComponentInChildren<MapManager>();
        ogoMn = parent.GetComponentInChildren<OnGroundObjectManager>();
	}
	
    // ダメージ与える側の計算
    public float CalcPower(SkillBase.TYPE type, float row_power)
    {
        float _powerF = row_power;
        if (thisChara.abnoState.atkUpTurn > 0 )
        {
            _powerF *= 2.0f;
        }
        // 属性耐性
        switch ( type )
        {
            case SkillBase.TYPE.NO_ELEMENT:
                _powerF *= normal_power;
                break;
            case SkillBase.TYPE.FLAME:
                _powerF *= base_magic_power;
                _powerF *= Mathf.Log(flame_power) + 1;
                break;
            case SkillBase.TYPE.LIGHTNING:
                _powerF *= base_magic_power;
                _powerF *= Mathf.Log(light_power) + 1;
                break;
            case SkillBase.TYPE.ICE:
                _powerF *= base_magic_power;
                _powerF *= Mathf.Log(ice_power) + 1;
                break;
        }
        // 地形効果

        // float のまま返す
        return _powerF;
    }

    // ダメージ受ける側の計算
    public int CalcDamage(SkillBase.TYPE type, float row_damage)
    {
        float _damageF = row_damage;
        if ( thisChara.abnoState.defUpTurn > 0 )
        {
            _damageF *= 0.5f;
        }
        if (thisChara.abnoState.invincibleTurn > 0 )
        {
            _damageF = 0;
        }
        // 属性耐性
        switch (type)
        {
            case SkillBase.TYPE.NO_ELEMENT:
                _damageF *= normal_resist;
                break;
            case SkillBase.TYPE.FLAME:
                _damageF *= flame_resist;
                break;
            case SkillBase.TYPE.LIGHTNING:
                _damageF *= light_resist;
                break;
            case SkillBase.TYPE.ICE:
                _damageF *= ice_resist;
                break;
        }
        // 地形効果
        int onFootNum = mapMn.onground_exist2D[(int)thisChara.pos.z, (int)thisChara.pos.x];
        var onGroundObj = ogoMn.GetOnGroundObject(onFootNum);
        // 水たまりは雷ダメージ大
        if( onGroundObj && onGroundObj.type == OnGroundObject.Type.WATER && type == SkillBase.TYPE.LIGHTNING)
        {
            _damageF *= 2;
            Debug.Log("水たまり効果で雷倍増！");
        }

        // 四捨五入して返す
        return (int)Mathf.Round(_damageF);
    }

    public int CalcAbnormalTurn(AbnormalStateType type, float effectValue)
    {
        switch ( type )
        {
            case AbnormalStateType.FREEZE:
                effectValue *= freeze_resist;
                break;
            case AbnormalStateType.PARALIZE:
                effectValue *= paralize_resist;
                break;
        }
        return Mathf.FloorToInt(effectValue);
    }
    
    [System.Serializable]
    public class SavableAtkAndDef
    {
        public float base_magic_power;
        public float normal_power;
        public float flame_power;
        public float light_power;
        public float ice_power;
        
        public float normal_resist;
        public float flame_resist;
        public float light_resist;
        public float ice_resist;

        public SavableAtkAndDef()
        {
            base_magic_power = 1;
            normal_power = 1;
            flame_power = 1;
            light_power = 1;
            ice_power = 1;

            normal_resist = 1;
            flame_resist = 1;
            light_resist = 1;
            ice_resist = 1;
        }

        public SavableAtkAndDef(AtkAndDef data)
        {
            base_magic_power = data.base_magic_power;
            normal_power = data.normal_power;
            flame_power = data.flame_power;
            light_power = data.light_power;
            ice_power = data.ice_power;

            normal_resist = data.normal_resist;
            flame_resist = data.flame_resist;
            light_resist = data.light_resist;
            ice_resist = data.ice_resist;
        }

        public void Load(AtkAndDef data_in)
        {
            data_in.base_magic_power = base_magic_power;
            data_in.normal_power = normal_power;
            data_in.flame_power = flame_power;
            data_in.light_power = light_power;
            data_in.ice_power = ice_power;

            data_in.normal_resist = normal_resist;
            data_in.flame_resist = flame_resist;
            data_in.light_resist = light_resist;
            data_in.ice_resist = ice_resist;
        }
    }

    public void SaveValue(string key)
    {
        SavableAtkAndDef data = new SavableAtkAndDef(this);
        SaveData.SetClass(key, data);
    }

    public void LoadValue(string key)
    {
        SavableAtkAndDef savedData = SaveData.GetClass(key, new SavableAtkAndDef());
        savedData.Load(this);
    }
}
