using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightShot : StraightShot
{
    public override void OnDecided()
    {
        base.OnDecided();
        if (battleTarget )
            battleTarget.CauseAbnormalState(AbnormalStateType.PARALIZE, abnoEffectPower);
    }
}
