using UnityEngine;
using VoxelTools;

public class LazerCollision : MonoBehaviour
{
    public float collisionScale = 1;
    [HideInInspector] public bool oneTime = false;
    public GameObject parent;
    public float radius;
    public float force;
    public float overrideMax;
    public ParticleSystem particle;
    private DestructionCarController destructionCarController;
    public AudioLazerCon audioLazerCon;
    void Awake()
    {
        oneTime = true;
        parent = GetComponentInParent<LazerStarter>().gameObject;
        audioLazerCon = GetComponentInChildren<AudioLazerCon>();
    }
    void Update()
    {
        var i = parent.transform.localScale;
        this.transform.localScale = new Vector3(1 / i.x, 1 / i.y, 1 / i.z);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.tag != "Player")
            return;

        Explode();
    }
    void Explode()
    {
        destructionCarController = FindObjectOfType<DestructionCarController>();

        particle.Play();

        VoxelPhysics.DestroyInRadius(transform.position, radius);

        //destructionCarController.Refresh();
        audioLazerCon.Play();
    }
}
