using System;
using UnityEngine;

[Serializable]
public class Axle
{
    [SerializeField]
    public bool IsSteeringAxle;

    [SerializeField]
    public Wheel RightWheel;

    [SerializeField]
    public Wheel LeftWheel;

    public Axle(Wheel rightWheel, Wheel leftWheel)
    {
        RightWheel = rightWheel;
        LeftWheel = leftWheel;
    }
}
