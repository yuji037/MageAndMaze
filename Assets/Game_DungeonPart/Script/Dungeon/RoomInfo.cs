using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 多重にシリアライズしてあると良くないらしい
[System.Serializable]
public class RoomInfo {
    
    public Vector3[] gatewayPos;

    public RoomInfo()
    {
        gatewayPos = new Vector3[4];
        for (int i = 0; i < gatewayPos.Length; i++)
        {
            gatewayPos[i] = new Vector3(-1, 0, -1);
        }
    }

    public void SetGateWayPos(Vector3 pos)
    {
        for (int i = 0; i < gatewayPos.Length; i++)
        {
            if (gatewayPos[i].x == -1)
            {
                gatewayPos[i] = pos;
                return;
            }
        }
        Debug.Log("Road:SetGateWay:Error");
        return;
    }
}
