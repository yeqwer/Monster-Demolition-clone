using UnityEngine;
using VoxelTools;

public class PlazmaController : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Flight settings")]
    public GameObject target;
    public float rotationSpeed = 10f;
    public float power = 100f;

    [Header("Explode settings")]
    public float radius;
    public float force;
    public float overrideMax;
    public ParticleSystem particle;
    public AudioPlazmaCon audioPlazmaCon;

    void Awake()
    {
        audioPlazmaCon = GetComponentInChildren<AudioPlazmaCon>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 direction = target.transform.position - rb.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        rb.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        rb.transform.eulerAngles = rb.transform.eulerAngles + new Vector3(0, 0, 777);
        rb.velocity = this.transform.forward.normalized * power;

        if (Vector3.Distance(this.transform.position, target.transform.position) < 3f) { ExplodeFake(); }
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.tag != "Player")
            return;

        Explode();
    }

    public void Explode()
    {
        audioPlazmaCon.Play();

        particle.Play();
        particle.transform.parent = null;
        Destroy(particle.gameObject, 2f);

        VoxelPhysics.DestroyInRadius(transform.position, radius);

        //if (obj.TryGetComponent(out DestructionCarController destructionCarController))
        //    destructionCarController.Refresh();

        Destroy(gameObject);
        Destroy(target);
    }
    public void ExplodeFake()
    {
        particle.Play();
        particle.transform.parent = null;

        Destroy(particle.gameObject, 2f);
        Destroy(gameObject);
        Destroy(target);
    }
}
