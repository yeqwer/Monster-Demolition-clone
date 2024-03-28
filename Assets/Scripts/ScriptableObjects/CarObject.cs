using UnityEngine;

[CreateAssetMenu(fileName = "NewCarObject", menuName = "Scriptable Objects/Car Object")]
public class CarObject : ScriptableObject
{
    [field: Header("Common")]
    [field: SerializeField]
    public string Name { get; private set; }

    [field: SerializeField]
    public Sprite Icon { get; private set; }

    //[field: SerializeField]
    //public int Cost { get; private set; }

    [field: Header("Camera settings")]
    [field: SerializeField]
    public Vector3 CameraOffset { get; private set; }

    [field: Header("Progression")]
    [field: SerializeField]
    public int UnlockAtProgressionLevel { get; private set; }
}
