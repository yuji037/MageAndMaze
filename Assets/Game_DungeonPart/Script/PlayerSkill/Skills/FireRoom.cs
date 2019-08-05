using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRoom : RoomMagic {

    public override IEnumerator Coroutine()
    {
        cameraMn.SetIsBigMagicCamera(true);
        anim.TriggerAnimator("Magic_forward2");
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(0.5f);
        cameraMn.SetIsBigMagicCamera(false);
        if (player.ExistRoomNum < mapMn.max_room)
        {
            for (int p = 0; p < _particleMax; p++)
            {
                int num = Random.Range(0, range.Count);
                var eff = Instantiate(effects[0]);
                eff.transform.position = new Vector3(range[num].x, 0, range[num].y);
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(1);
        }
        else
        {
            foreach (Vector2 ran in range)
            {
                var eff = Instantiate(effects[0]);
                eff.transform.position = new Vector3(ran.x, 0, ran.y);
                yield return new WaitForSeconds(0.2f);
            }
        }

        HitAndParamChange();
        yield return null;
    }
}
