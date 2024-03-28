using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TriggerEventSender : MonoBehaviour
{
    [SerializeField]
    public bool ProcessEvents;

    public event Action<Collider> onTriggerEnter;

    public event Action<Collider> onTriggerStay;

    public event Action<Collider> onTriggerExit;

    private void Awake()
    {
        if (!GetComponent<Collider>().isTrigger)
            Debug.LogError("IsTrigger flag is disabled on Collider. Events will not be sent.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!ProcessEvents)
            return;

        onTriggerEnter?.Invoke(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!ProcessEvents)
            return;

        onTriggerStay?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!ProcessEvents)
            return;

        onTriggerExit?.Invoke(other);
    }
}
