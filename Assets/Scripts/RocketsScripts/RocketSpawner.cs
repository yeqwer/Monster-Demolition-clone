using System.Linq;
using UnityEngine;
using VoxelTools;

public class RocketSpawner : MonoBehaviour
{
    private Rigidbody rb;
    private GameObject[] monsterParts;
    private float timer;
    private AudioRocketsCon audioRocketCon;

    [Header("Flight settings")]
    public GameObject target;
    public float rotationSpeed = 10f;
    public float power = 100f;

    [Header("Explode settings")]
    public float radius;
    public float force;
    public float overrideMax;
    public ParticleSystem particle;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        VoxelRootObject rootObject = GameObject.FindGameObjectWithTag("MonsterStart").GetComponentInChildren<VoxelRootObject>();
        GameObject[] modelObjects = rootObject.gameObject.GetComponentsInChildren<VoxelModelObject>().Select(x => x.gameObject).ToArray();
        GameObject[] fragmentedObjects = rootObject.gameObject.GetComponentsInChildren<VoxelFragmentedObject>().Select(x => x.gameObject).ToArray();
        monsterParts = modelObjects.Concat(fragmentedObjects).ToArray();

        audioRocketCon = GetComponentInChildren<AudioRocketsCon>();
        timer = (200 - power) / 10;
    }

    void Start()
    {
        target = monsterParts[Random.Range(0, monsterParts.Length - 1)].gameObject.transform.parent.parent.parent.gameObject;
    }
    void Update()
    {
        Vector3 direction = target.transform.position - rb.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        rb.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        rb.transform.eulerAngles = rb.transform.eulerAngles + new Vector3(0, 0, 777);
        rb.velocity = this.transform.forward.normalized * power;

        Timer();
    }

    void Timer()
    {
        timer -= Time.deltaTime * 3;

        if (timer <= 0)
        {
            Explode();
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    public void Explode()
    {
        audioRocketCon.Play();

        particle.Play();
        particle.transform.parent = null;
        Destroy(particle.gameObject, 2f);

        Destroy(gameObject);

        VoxelPhysics.DestroyInRadius(transform.position, radius);
    }
}
