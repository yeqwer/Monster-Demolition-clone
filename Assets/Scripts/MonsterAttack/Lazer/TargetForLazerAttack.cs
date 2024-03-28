using UnityEngine;

public class TargetForLazerAttack : MonoBehaviour
{
    private GameObject car;
    private Vector3 targetPosition;
    public float offsetSpawnByCar = 20f;
    public float lazerMoveSpeed = 10f;

    void Awake()
    {
        car = GameObject.FindGameObjectWithTag("Player");
    }
    void Start()
    {
        this.gameObject.transform.position = new Vector3(car.transform.position.x, 0, car.transform.position.z + offsetSpawnByCar);
        var i = this.gameObject.transform.position;
        targetPosition = new Vector3(i.x, i.y, i.z - offsetSpawnByCar * 2);
    }
    void Update()
    {
        if (Vector3.Distance(this.transform.position, targetPosition) > 2)
        {
            this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position, targetPosition, Time.deltaTime * lazerMoveSpeed);
        }
        else { Destroy(this.gameObject); }
    }
}