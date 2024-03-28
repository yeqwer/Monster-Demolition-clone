using UnityEngine;
using VoxelTools;

public class MonsterBomb : MonoBehaviour
{
    public float radius;
    public float force;
    public float overrideMax;
    public ParticleSystem particle;
    private AudioBombCon audioBombCon;

    void Awake()
    {
        audioBombCon = GetComponentInChildren<AudioBombCon>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag != "Player")
            return;

        Explode();
    }
    public void Explode()
    {
        audioBombCon.Play();

        particle.Play();
        particle.transform.parent = null;
        Destroy(particle.gameObject, 2f);

        VoxelPhysics.DestroyInRadius(transform.position, radius);

        Destroy(gameObject);

        //obj.GetComponentInParent<DestructionCarController>().Refresh();
    }
}