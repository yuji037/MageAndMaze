using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageFloorCount : MonoBehaviour {

    [SerializeField]
    float _fastSpeed = 200.0f;
    [SerializeField]
    float _slowSpeed = 100.0f;
    Text text;
    GameObject parent;
    DungeonPartManager _dPM;


    // Use this for initialization
    void Start () {
        text = GetComponentInChildren<Text>();
        parent = GameObject.Find("GameObjectParent");
        _dPM = parent.GetComponentInChildren<DungeonPartManager>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 pos = transform.localPosition;

        if(pos.x < -100)
        {
            transform.localPosition += new Vector3(_fastSpeed * Time.deltaTime, 0, 0);
        }
        else if (pos.x < 100)
        {
            transform.localPosition += new Vector3(_slowSpeed * Time.deltaTime, 0, 0);
        }
        else if (pos.x < 2000)
        {
            transform.localPosition += new Vector3(_fastSpeed * Time.deltaTime, 0, 0);
        }
        else
        {
            Destroy(gameObject);
        }

        text.text = "地下" + _dPM.floor + "階";
    }
}
