using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicSelectButtonManager : MonoBehaviour {

    GameObject parent;
    GameObject magicSelectWindow;
    MagicButtonManager magicBtnMn;

    private void Awake()
    {
        parent = GameObject.Find("GameObjectParent");
        magicBtnMn = parent.GetComponentInChildren<MagicButtonManager>();
        magicSelectWindow = magicBtnMn.gameObject;
    }

    // Use this for initialization
    void Start () {
        magicSelectWindow.SetActive(false);
	}
	
    public void Tap()
    {
        magicSelectWindow.SetActive(!magicSelectWindow.activeSelf);
    }

    private void OnEnable()
    {
        magicSelectWindow.SetActive(false);
    }
}
