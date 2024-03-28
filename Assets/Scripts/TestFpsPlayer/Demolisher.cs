using System;
using System.Collections;
using UnityEngine;
using VoxelTools;

//[RequireComponent(typeof(Rigidbody))]
public class Demolisher : MonoBehaviour
{
    private Rigidbody _rigidbody;

    private BombManager bombManager;

    private AudioCarCon audioCarCon;

    [SerializeField]
    private float _destructionRadius = 5f;

    [SerializeField]
    private bool _useOncePerMatch = true;

    private bool _isUsedInThisMatch = false;

    private int _destructibleLayer;

    public event Action<GameObject> OnHitDestructible;

    public bool IsDirectedDestruction;

    private void Awake()
    {
        _destructibleLayer = LayerMask.GetMask("Destructible");

        _rigidbody = GetComponent<Rigidbody>();
        bombManager = GetComponent<BombManager>();
        audioCarCon = GetComponentInChildren<AudioCarCon>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_useOncePerMatch && _isUsedInThisMatch)
            return;

        VoxelModelObject modelObject = collision.gameObject.GetComponent<VoxelModelObject>();
        VoxelFragmentObject fragmentObject = collision.gameObject.GetComponent<VoxelFragmentObject>();

        if (modelObject == null && fragmentObject == null)
            return;

        OnHitDestructible?.Invoke(collision.gameObject);

        ApplyDestruction(collision.contacts[0].point);

        audioCarCon.Play();

        if (bombManager.bombs.Count > 0)
            StartCoroutine(BobmExplosion());

        _isUsedInThisMatch = true;
    }

    private void ApplyDestruction(Vector3 position)
    {
        if (!IsDirectedDestruction)
        {
            VoxelPhysics.DestroyInRadius(position, _destructionRadius);
        }
        else
        {
#if UNITY_EDITOR
            Debug.DrawLine(position, position + transform.forward * _rigidbody.velocity.magnitude, Color.red, 5f);
#endif
            VoxelPhysics.DestroyInCapsule(position, position + transform.forward * _rigidbody.velocity.magnitude, _destructionRadius);
        }
    }

    IEnumerator BobmExplosion()
    {
        yield return new WaitForSeconds(0.5f);
        if (bombManager.bombSocet.transform.childCount > 0)
        {
            bombManager.bombSocet.transform.GetChild(0).GetComponent<BombExplosion>().Explode();
            bombManager.RemoveBomb(false);
        }
    }
}
