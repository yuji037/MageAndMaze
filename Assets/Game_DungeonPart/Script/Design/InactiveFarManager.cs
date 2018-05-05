using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InactiveFarManager : MonoBehaviour {

    public bool IsActive = true;
    public List<InactiveFarFromPlayer> objects = new List<InactiveFarFromPlayer>();

    private void Start()
    {
        UpdateInactivateObjects();
    }

    public void UpdateInactivateObjects()
    {
        if ( !IsActive ) return;

        foreach(InactiveFarFromPlayer obj in objects )
        {
            if ( obj )
                obj.UpdateInactivate();
        }
    }
}
