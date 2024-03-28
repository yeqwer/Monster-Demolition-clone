using UnityEngine;
using Zenject;

public class CameraController : MonoBehaviour
{
    private Transform _transform;

    [SerializeField]
    private Transform _target;

    [SerializeField]
    private Vector3 _followOffset = new Vector3(0, 3, -7);

    [SerializeField]
    private Vector3 _lookOffset = new Vector3(0, 2, 0);

    [Inject]
    private void Construct(CarSpawnManager carSpawnManager)
    {
        carSpawnManager.OnCarSpawned += AssignCarAsTarget;
    }

    private void AssignCarAsTarget(CarController carController)
    {
        _target = carController.transform;
    }

    private void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    private void Update()
    {
        _transform.position = _target.position + _followOffset;
        _transform.LookAt(_target.position + _lookOffset);
    }
}
