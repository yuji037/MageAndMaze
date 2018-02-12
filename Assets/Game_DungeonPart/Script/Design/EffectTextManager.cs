using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTextManager : MonoBehaviour {

    GameObject parent;
    GameObject camera;
    [SerializeField]
    GameObject effectTextPre;


	// Use this for initialization
	void Start () {
        parent = GameObject.Find("GameObjectParent");
        camera = Camera.main.gameObject;
	}

    public void CreateEffectText(Vector3 pos, int val)
    {
        var obj = Instantiate(effectTextPre, parent.transform);
        obj.transform.position = pos;
        obj.transform.LookAt(camera.transform.position);
        obj.GetComponentInChildren<TextMesh>().text = val.ToString();
    }
}
