using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventTriggerCameraRotater : EventTrigger{

    GameObject parent;
    GameObject cameraParent;
    CameraManager cameraMn;
    GameObject moveButtonsParent;
    GameObject minimapCenter;
    public float selectRotateY = 0;
    float[] cameraPosRotateY = { 0,90,180,270,360 };
    bool isDrag;
    TutorialManager tutorialMn;

    // Use this for initialization
    void Start () {
        if ( parent ) return;
        parent = GameObject.Find("GameObjectParent");
        cameraParent = parent.GetComponentInChildren<MainCameraParent>().gameObject;
        cameraMn = parent.GetComponentInChildren<CameraManager>();
        moveButtonsParent = parent.GetComponentInChildren<MoveButtonsParent>().gameObject;
        minimapCenter = parent.GetComponentInChildren<MinimapCenter>().gameObject;
        minimapCenter.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        if (!isDrag)
        {
            if (selectRotateY == -360)
            {
                // 固定角度を決定
                float _minDeltaR = 3600;
                foreach (int rotY in cameraPosRotateY)
                {
                    float _deltaR = cameraParent.transform.eulerAngles.y - rotY;
                    _deltaR = Mathf.Abs(_deltaR);
                    if (_deltaR < _minDeltaR)
                    {
                        //Debug.Log(_deltaR);
                        selectRotateY = rotY;
                        _minDeltaR = _deltaR;
                    }
                    RotateMoveButtonsAndMiniMap(selectRotateY);
                }
            }
            else
            {
                float _leftRotateY = selectRotateY - cameraParent.transform.eulerAngles.y;
                _leftRotateY *= 10.0f * Time.deltaTime;
                cameraParent.transform.Rotate(0, _leftRotateY, 0);
                if (cameraParent.transform.eulerAngles.y < 10 && selectRotateY == 360)
                {
                    selectRotateY = 0;
                }
            }
        }

        if (Input.touchCount >= 2 )
        {
            StartCoroutine(ZoomInOut());
        }
	}

    public void RotateMoveButtonsAndMiniMap(float _selectRotateY)
    {
        if ( !parent ) Start();
        selectRotateY = _selectRotateY;
        float _moveButtonRotate = _selectRotateY - moveButtonsParent.transform.eulerAngles.z;
        moveButtonsParent.transform.Rotate(0, 0, _moveButtonRotate);
        minimapCenter.transform.Rotate(0, 0, _moveButtonRotate);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        isDrag = false;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        Vector2 delta = eventData.delta;

        cameraParent.transform.Rotate(0, delta.x * 0.15f, 0);
        cameraMn.SetCameraHeight(delta.y * -0.01f);
        selectRotateY = -360;
        isDrag = true;
    }
    
    IEnumerator ZoomInOut()
    {
        Touch t1 = Input.GetTouch(0);
        Touch t2 = Input.GetTouch(1);
        float magStartDist = ( t2.position - t1.position ).magnitude;

        while (Input.touchCount >= 2 )
        {
            t1 = Input.GetTouch(0);
            t2 = Input.GetTouch(1);
            float magNowDist = ( t2.position - t1.position ).magnitude;

            float diff = magNowDist - magStartDist;
            diff *= -0.0001f;

            cameraMn.SetCameraZoom(1 + diff);

            magStartDist = ( t2.position - t1.position ).magnitude;
            yield return null;
        }
    }

}
