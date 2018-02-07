using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffectManager : MonoBehaviour {

    GameObject parent;
    GameObject camera;
    [SerializeField]
    GameObject damageTextPre;


	// Use this for initialization
	void Start () {
        parent = GameObject.Find("GameObjectParent");
        camera = Camera.main.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CreateDamagerText(Vector3 pos, int damage)
    {
        var dmg = Instantiate(damageTextPre, parent.transform);
        dmg.transform.position = pos;
        dmg.transform.LookAt(camera.transform.position);
        dmg.GetComponentInChildren<TextMesh>().text = damage.ToString();
    }
}
