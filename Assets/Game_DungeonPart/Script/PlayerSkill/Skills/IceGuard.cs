using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceGuard : StraightShot {

    public override void OnDecided()
    {
        base.OnDecided();
        player.m_cAbnoState.SetTurn(AbnoStateType.Invincible, abnoEffectPower);
    }
}
