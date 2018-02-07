using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceRoom : RoomMagic {

    OnGroundObjectManager ogoMn;

    public override IEnumerator Coroutine()
    {
        cameraMn.SetIsBigMagicCamera(true);
        anim.TriggerAnimator("Magic_forward2");
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(0.5f);
        cameraMn.SetIsBigMagicCamera(false);
        if (player.existRoomNum < mapMn.max_room)
        {
            for (int p = 0; p < _particleMax; p++)
            {
                int num = Random.Range(0, range.Count);
                var eff = Instantiate(effects[0]);
                if ( eff ) eff.transform.position = new Vector3(range[num].x, 0, range[num].y);
                yield return new WaitForSeconds(0.2f);
            }
        }
        else
        {
            foreach (Vector2 ran in range)
            {
                var eff = Instantiate(effects[0]);
                if ( eff ) eff.transform.position = new Vector3(ran.x, 0, ran.y);
                yield return new WaitForSeconds(0.2f);
            }
        }
        HitAndParamChange();
        yield return null;
    }

    public override void HitAndParamChange()
    {
        base.HitAndParamChange();

        ogoMn = parent.GetComponentInChildren<OnGroundObjectManager>();

        foreach (Vector2 ran in range )
        {
            // 水たまり設置
            var ogo = ogoMn.AddOnGroundObject(OnGroundObject.Type.WATER, targetExistPos + new Vector3(ran.x, 0, ran.y));
        }
    }
}
