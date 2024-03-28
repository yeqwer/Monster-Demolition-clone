using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(ConfigurableJoint))]
public class Wheel : MonoBehaviour
{
    private Transform _transform;

    private Rigidbody _rigidbody;

    private SphereCollider _sphereCollider;

    private ConfigurableJoint _joint;

    [SerializeField]
    private Transform _wheelSteeringTransform;

    [SerializeField]
    private Transform _wheelRotatingTransform;

    public bool IsGrounded { get; private set; }

    private float _radius => _sphereCollider.radius;

    private float _rotationSpeed
    {
        get
        {
            Vector3 accelerationDirection = _wheelSteeringTransform.forward;

            Vector3 accelerationVelocity = Vector3.Project(_rigidbody.velocity, accelerationDirection);

            float sign = Mathf.Sign(Vector3.Dot(accelerationDirection, accelerationVelocity));

            float angularSpeed = accelerationVelocity.magnitude / (2 * Mathf.PI * _radius);

            return sign * angularSpeed;
        }
    }

    public float SteerAngle
    {
        get => _wheelSteeringTransform.localEulerAngles.y;
        set => _wheelSteeringTransform.localEulerAngles = new Vector3(_wheelSteeringTransform.localEulerAngles.x, value, _wheelSteeringTransform.localEulerAngles.z);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        _joint = GetComponent<ConfigurableJoint>();

        _joint.connectedAnchor = transform.localPosition;
    }
#endif

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _rigidbody = GetComponent<Rigidbody>();
        _sphereCollider = GetComponent<SphereCollider>();
    }

    private void Update()
    {
        HandleWheelAnimation();
    }

    private void OnCollisionStay(Collision collision)
    {
        IsGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        IsGrounded = false;
    }

    private void HandleWheelAnimation()
    {
        _wheelRotatingTransform.Rotate(Vector3.right, _rotationSpeed, Space.Self);
    }
}
