using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventItemPopUp : EventTrigger {

    GameObject parent;
    InventryInfo inventryInfo;
    public int cellNum = -1;

	// Use this for initialization
	void Start () {
        parent = GameObject.Find("GameObjectParent");
        inventryInfo = parent.GetComponentInChildren<InventryInfo>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void OnPointerClick(PointerEventData eventData)
    {
        //Vector3 pos = transform.localPosition;
        //Debug.Log(transform.localPosition);
        //int y = Mathf.RoundToInt( ( pos.y + 75 ) / -150 );
        //int x = Mathf.RoundToInt( ( pos.x - 75 ) / 150 );
        //Debug.Log(x + " " + y);
        inventryInfo.PopUpItemDescription(true, cellNum);
    }

}
