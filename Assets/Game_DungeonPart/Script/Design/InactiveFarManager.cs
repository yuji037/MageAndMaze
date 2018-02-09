using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InactiveFarManager : MonoBehaviour {

    public List<InactiveFarFromPlayer> objects = new List<InactiveFarFromPlayer>();

    private void Start()
    {
        UpdateInactivateObjects();
    }

    public void UpdateInactivateObjects()
    {
        foreach(InactiveFarFromPlayer obj in objects )
        {
            if ( obj )
                obj.UpdateInactivate();
        }
    }
}
