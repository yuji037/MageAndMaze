using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapEffectManager : MonoBehaviour {

    public GameObject tapEffectPrefab;
    [SerializeField]
    GameObject canvas;
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = Input.mousePosition;
            Instantiate(tapEffectPrefab,canvas.transform).transform.position = pos;
            
        }
	}
}
