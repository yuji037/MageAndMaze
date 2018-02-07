using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchScene : MonoBehaviour {

    [SerializeField]
    string _sceneName;

    public void Scene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(_sceneName);
    }
}
