using UnityEngine;

[CreateAssetMenu(fileName = nameof(CarProgressionObject), menuName = "Scriptable Objects/" + nameof(CarProgressionObject))]
public class CarProgressionObject : ScriptableObject
{
    [field: SerializeField]
    public CarUpgradeTier[] CarUpgradeTiers { get; private set; }
}
