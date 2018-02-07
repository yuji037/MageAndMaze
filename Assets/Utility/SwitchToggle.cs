using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchToggle : MonoBehaviour {

    [SerializeField]
    bool _isActive = false;
    [SerializeField]
    GameObject _gameObject;

	public void SwitchActivate()
    {
        _isActive = (!_isActive);
        _gameObject.SetActive(_isActive);
    }
}
