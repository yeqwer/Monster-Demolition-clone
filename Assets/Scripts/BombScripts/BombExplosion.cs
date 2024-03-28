using UnityEngine;
using VoxelTools;

public class BombExplosion : MonoBehaviour
{
    [Header("Explode settings")]
    public float radius;
    public float force;
    public float overrideMax;
    public ParticleSystem particle;
    private AudioBombCon audioBombCon;

    void Awake()
    {
        audioBombCon = GetComponentInChildren<AudioBombCon>();
    }

    public void Explode()
    {
        audioBombCon.Play();

        particle.Play();
        particle.transform.parent = null;
        Destroy(particle.gameObject, 2f);

        Destroy(gameObject);

        VoxelPhysics.DestroyInRadius(transform.position, radius);
    }
}

