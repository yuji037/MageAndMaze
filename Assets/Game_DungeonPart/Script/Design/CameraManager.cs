using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    GameObject _parent;
    public GameObject _target;
    public float _targetHeight;
    GameObject playerObj;
    public Vector3 pos;
    Vector3 shake;
    Vector3 shakeSpeed;
    [SerializeField] Vector3 shakeAcc = new Vector3(5, 5, 0);
    float shakeTime = 0;
    [SerializeField] float shakeTimeMax = 2.0f;
    [SerializeField] bool _isBigMagic = false;
    [SerializeField] Vector3 bigMagicCameraPos;
    AudioSource se;
	// Use this for initialization
	void Start () {
        _parent = GameObject.Find("GameObjectParent");
        //_target = GameObject.FindWithTag("Player");
        _target = _parent.GetComponentInChildren<MainCameraParent>().gameObject;
        playerObj = _parent.GetComponentInChildren<Player>().gameObject;
        se = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        if (shakeTime < shakeTimeMax)
        {
            if (shake.x > 0)
            {
                shakeSpeed.x -= shakeAcc.x;
                shakeSpeed.x *= 0.6f;
            }
            else if (shake.x < 0)
            {
                shakeSpeed.x += shakeAcc.x;
                shakeSpeed.x *= 0.6f;
            }
            if (shake.y > 0)
            {
                shakeSpeed.y -= shakeAcc.x;
                shakeSpeed.y *= 0.6f;
            }
            else if (shake.y < 0)
            {
                shakeSpeed.y += shakeAcc.x;
                shakeSpeed.y *= 0.6f;
            }
            shake += shakeSpeed * Time.deltaTime;
            shakeTime += Time.deltaTime;
        }
        else shake = Vector3.zero;

        if (_isBigMagic)
        {
            Vector3 _pos = Calc.RotateY(bigMagicCameraPos, (int)playerObj.transform.eulerAngles.y); 
            transform.localPosition = _pos + shake;
        }
        else transform.localPosition = pos + shake;

        transform.LookAt(_target.transform.position + new Vector3(0, _targetHeight, 0) + shake);
    }

    public void SetIsBigMagicCamera(bool on)
    {
        _isBigMagic = on;
        if (on) se.Play();
    }

    public void CameraShake(float mag)
    {
        Vector3 dir = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0);
        if (dir.magnitude == 0) dir.y = 1f;
        dir = dir.normalized;
        shakeSpeed = dir * mag;
        shakeTime = 0;
    }

    public void SetCameraHeight(float deltaY)
    {
        pos.y = Mathf.Clamp(pos.y + deltaY, 2, 6);
    }

    [SerializeField] float minDistance = 3;
    [SerializeField] float maxDistance = 10;

    public void SetCameraZoom(float scale)
    {
        pos *= scale;
        float sqrLength = pos.sqrMagnitude;
        if ( sqrLength > maxDistance * maxDistance ) pos *= maxDistance / pos.magnitude;
        if ( sqrLength < minDistance * minDistance ) pos *= minDistance / pos.magnitude;
    }

    public void MoveInOneFrame()
    {
        _target.GetComponent<Seek>().MoveInOneFrame();
    }
}
