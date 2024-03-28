using UnityEngine;

[CreateAssetMenu(fileName = "NewEquipmentObject", menuName = "Scriptable Objects/Equipment Object")]
public class EquipmentObject : ScriptableObject
{
    [field: SerializeField]
    public EquipmentType EquipmentType { get; private set; }

    [field: SerializeField]
    public string Name { get; private set; }

    [field: SerializeField]
    public Sprite Icon { get; private set; }
}
