using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveButtonManager : MonoBehaviour {

    public bool _isActive;
    [SerializeField] float pushIntervalMax = 0.4f;
    [SerializeField] float pushInterval = 0;
    Vector3 inputMoveDir = Vector3.zero;
    GameObject parent;
    PlayerMove playerMove;

	// Use this for initialization
	void Start () {
        _isActive = true;
        parent = GameObject.Find("GameObjectParent");
        playerMove = parent.GetComponentInChildren<PlayerMove>();
	}
	
	// Update is called once per frame
	void Update () {
        pushInterval += Time.deltaTime;

        if (inputMoveDir != Vector3.zero)
        {
            playerMove.MoveStart(inputMoveDir);
            if (false == _isActive && pushInterval >= pushIntervalMax)
            {
                //playerMove.resMoveDir = moveDir;
                pushInterval = 0;
            }
        }
    }

    public void MoveButtonPointerDown(int dir)
    {
        inputMoveDir = Vector3.zero;
        if (dir <= 1 || dir >= 7) inputMoveDir.z = 1;
        if (dir >= 3 && dir <= 5) inputMoveDir.z = -1;
        if (dir >= 1 && dir <= 3) inputMoveDir.x = 1;
        if (dir >= 5 && dir <= 7) inputMoveDir.x = -1;

        if (false == _isActive) return;
        playerMove.MoveStart(inputMoveDir);
        pushInterval = 0;
    }

    public void MoveButtonPointerUp()
    {
        inputMoveDir = Vector3.zero;
    }
}
