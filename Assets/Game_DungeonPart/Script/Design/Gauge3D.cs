using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gauge3D : MonoBehaviour
{

    GameObject mainCamera;
    public BattleParticipant chara;
    GaugeFill gaugeFill;

    public bool isActive = true;
    [SerializeField]
    SpriteRenderer[] ren;

    // Use this for initialization
    void Start()
    {
        mainCamera = Camera.main.gameObject;
        gaugeFill = GetComponentInChildren<GaugeFill>();
        LookAtCamera();
        ren = GetComponentsInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < ren.Length; i++)
        {
            ren[i].enabled = isActive;
        }
        LookAtCamera();
        float gaugePercentage = (float)chara.HP / chara.MaxHP;
        float left = -1.28f;
        gaugeFill.transform.localPosition = new Vector3(left + 1.28f * gaugePercentage, 0, 0);
        gaugeFill.transform.localScale = new Vector3(gaugePercentage, 1, 1);
    }

    public void LookAtCamera()
    {
        Vector3 cameraDir = mainCamera.transform.forward;
        transform.LookAt(transform.position - cameraDir);
    }
}