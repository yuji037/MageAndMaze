using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceShot : StraightShot
{
    public override void OnDecided()
    {
        base.OnDecided();
        if ( battleTarget )
            battleTarget.CauseAbnormalState(AbnormalStateType.FREEZE, abnoEffectPower);
    }
}
