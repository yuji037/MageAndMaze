using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGroundObject : MonoBehaviour
{
    public enum Type
    {
        WATER,
        HEAL_PANEL,
        STAIRS
    }
    public Type type;
    public int ID;
    public Vector3 pos;
}
