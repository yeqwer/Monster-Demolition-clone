using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RealTimeSettingsToRigger : MonoBehaviour
{
    public bool IsFlyingObject;

    [Header("Legs")]
    public List<Element> successLegs;

    [Header("Arms")]
    public List<Element> successArms;

    [Header("Body")]
    public List<Element> successBody;

    [Header("Heads")]
    public List<Element> successHead;
}

[System.Serializable]
public class Element
{
    public List<GameObject> list;
    public bool itIsBody;
}