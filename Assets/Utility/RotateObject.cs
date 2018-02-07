using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour {

    [SerializeField]
    private Vector3 angle;

    // Update is called once per frame
    void Update () {
        transform.Rotate(angle * Time.deltaTime);
	}
}
