using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCenterButtonManager : MonoBehaviour {

    // プレイヤーの向き変え
    [SerializeField]
    private Vector3 pos;
    private Vector3 sPos;
    private PlayerMove playerMove;
    public GameObject charaDirection;
    GameObject parent;
    GameObject moveButtonsParent;
    Vector3[] fixedDirection = new Vector3[8];

    TurnManager turnMn;

	// Use this for initialization
	void Start () {
        parent = GameObject.Find("GameObjectParent");
        moveButtonsParent = parent.GetComponentInChildren<MoveButtonsParent>().gameObject;
        playerMove = parent.GetComponentInChildren<PlayerMove>();
        charaDirection = parent.GetComponentInChildren<CharaDirection_Player>().gameObject;
        turnMn = parent.GetComponentInChildren<TurnManager>();
        charaDirection.SetActive(false);
        fixedDirection[0] = new Vector3(0, 1, 0);
        fixedDirection[1] = new Vector3(1, 1, 0);
        fixedDirection[2] = new Vector3(1, 0, 0);
        fixedDirection[3] = new Vector3(1, -1, 0);
        fixedDirection[4] = new Vector3(0, -1, 0);
        fixedDirection[5] = new Vector3(-1, -1, 0);
        fixedDirection[6] = new Vector3(-1, 0, 0);
        fixedDirection[7] = new Vector3(-1, 1, 0);
    }
	
    public void DragStart()
    {
        pos = Input.mousePosition;
        charaDirection.SetActive(true);
    }
    public void DragStay()
    {
        if ( turnMn.PlayerActionSelected ) return;

        sPos = Input.mousePosition;
        for (int i = 0; i < 8; i++)
        {
            float angle = 0;
            angle = Vector3.Angle(sPos - pos, fixedDirection[i]);
            if (angle <= 22.5f)
            {
                Vector3 selectDir;
                int _moveButtonsRotationZ = (int)moveButtonsParent.transform.eulerAngles.z;
                selectDir = Calc.RotateZ(fixedDirection[i], -_moveButtonsRotationZ);
                selectDir = new Vector3(selectDir.x, 0, selectDir.y);
                playerMove.SetCharaDir(selectDir);
                break;
            }
        }
    }
    public void DragEnd()
    {
        charaDirection.SetActive(false);

    }
}
