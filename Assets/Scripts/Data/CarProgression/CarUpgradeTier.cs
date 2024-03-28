using System;
using UnityEngine;

[Serializable]
public class CarUpgradeTier
{
    [field: SerializeField]
    public int UpgradeCost { get; private set; }
}
