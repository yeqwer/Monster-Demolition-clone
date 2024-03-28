using UnityEngine;
using VoxelTools;

public class RockCollider : MonoBehaviour
{
    public float radius;
    public float force;
    public float overrideMax;
    public ParticleSystem particle;
    public AudioRocksCon audioRocksCon;

    private void Awake()
    {
        audioRocksCon = GetComponentInParent<RocksController>().GetComponentInChildren<AudioRocksCon>();
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.tag != "Player")
            return;

        Explode();
    }

    public void Explode()
    {
        particle.Play();
        particle.transform.parent = null;
        Destroy(particle.gameObject, 2f);

        VoxelPhysics.DestroyInRadius(transform.position, radius);

        audioRocksCon.Play();
        Destroy(GetComponentInParent<RocksController>().gameObject);
    }
}