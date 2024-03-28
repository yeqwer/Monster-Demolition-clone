using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollisionEventSender : MonoBehaviour
{
    [SerializeField]
    public bool ProcessEvents;

    public Action<Collision> onCollisionEnter;

    public Action<Collision> onCollisionStay;

    public Action<Collision> onCollisionExit;

    private void Awake()
    {
        if (GetComponent<Collider>().isTrigger)
            Debug.LogError("IsTrigger flag is enabled on Collider. Events will not be sent.");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!ProcessEvents)
            return;

        onCollisionEnter?.Invoke(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!ProcessEvents)
            return;

        onCollisionStay?.Invoke(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!ProcessEvents)
            return;

        onCollisionExit?.Invoke(collision);
    }
}
