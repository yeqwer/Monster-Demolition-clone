using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    private Transform _transform;

    private Rigidbody _rigidbody;

    [SerializeField]
    private Transform _centerOfMass;

    [SerializeField]
    private Axle[] _axles;

    [Header("Common settings")]
    [SerializeField]
    private float _topSpeed = 50f;

    [SerializeField]
    private float _sidewaysSpeed = 10f;

    [SerializeField]
    [Tooltip("Max rotation of wheels and car when steering (Visual only)")]
    private float _maxRotation = 20f;

    [Header("Air movement")]
    [SerializeField]
    [Tooltip("Minimum XZ velocity magnitude that car must have to handle air movement")]
    private float _airMovementThreshold = 10f;

    [SerializeField]
    [Tooltip("Speed of vertical movement when not gruonded")]
    private float _airVerticalAcceleration = 250f;

    [SerializeField]
    [Tooltip("Speed of sideways movement when not grounded")]
    private float _airSideAcceleration = 700f;

    [SerializeField]
    [Tooltip("Minimum XZ velocity magnitude that car must have to look forward when not grounded")]
    private float _airLookForwardThreshold = 5f;

    [SerializeField]
    [Tooltip("How fast car will look at the direction of velocity (Look forward when not grounded)")]
    private float _airLookForwardSpeed = 5f;

    public bool IsGrounded => _axles.Any(x => x.RightWheel.IsGrounded || x.LeftWheel.IsGrounded);

    public bool IsInputActive;

    private float _verticalInput;

    private float _horizontalInput;

    public event Action OnInputPressed;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.centerOfMass = _centerOfMass.localPosition;
    }

    private void FixedUpdate()
    {
        if (IsGrounded)
        {
            HandleForwardMovement();
            HandleSideMovement();
            HandleBraking();
        }
        else
        {
            HandleAirMovement();
            HandleAirLookForward();
        }
    }

    public void UpdateInputs()
    {
        if (IsInputActive)
        {
            _verticalInput = Input.GetAxisRaw("Vertical");
            _horizontalInput = Input.GetAxisRaw("Horizontal");

            if (Mathf.Abs(_verticalInput) >= 0.1f || Mathf.Abs(_horizontalInput) >= 0.1f)
                OnInputPressed?.Invoke();
        }
        else
        {
            _verticalInput = 0;
            _horizontalInput = 0;
        }
    }

    private void HandleForwardMovement()
    {
        if (_verticalInput < 0.1f)
            return;

        float currentForwardSpeed = _rigidbody.velocity.z;
        float desiredForwardSpeed = _verticalInput * _topSpeed;

        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, _rigidbody.velocity.y,
            Mathf.Lerp(currentForwardSpeed, desiredForwardSpeed, Time.fixedDeltaTime));
    }

    private void HandleBraking()
    {
        if (_verticalInput >= 0.1f)
            return;

        float currentForwardSpeed = _rigidbody.velocity.z;
        float desiredForwardSpeed = 0;

        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, _rigidbody.velocity.y,
            Mathf.Lerp(currentForwardSpeed, desiredForwardSpeed, Time.fixedDeltaTime));
    }

    private void HandleSideMovement()
    {
        // Rotating wheels
        foreach (Axle axle in _axles)
        {
            if (!axle.IsSteeringAxle)
                continue;

            axle.RightWheel.SteerAngle = _horizontalInput * _maxRotation;
            axle.LeftWheel.SteerAngle = _horizontalInput * _maxRotation;
        }

        Vector3 forwardMovement = Vector3.Project(_rigidbody.velocity, Vector3.forward);
        if (forwardMovement.magnitude < 1f)
        {
            _rigidbody.velocity = new Vector3(0f, _rigidbody.velocity.y, _rigidbody.velocity.z);
            return;
        }

        float smoothSpeed = 5f;

        float yRotation = _maxRotation * _horizontalInput;

        float rateMultiplier = 4f;
        float sideMovementRate = Mathf.Clamp01(forwardMovement.magnitude / _topSpeed * rateMultiplier);

        // Rotating car
        _rigidbody.rotation = Quaternion.Lerp(_transform.rotation,
            Quaternion.Euler(_transform.eulerAngles.x, sideMovementRate * yRotation, _transform.eulerAngles.z),
            smoothSpeed * Time.fixedDeltaTime);

        float horizontalMovement = _horizontalInput * sideMovementRate * _sidewaysSpeed;
        _rigidbody.velocity = new Vector3(horizontalMovement, _rigidbody.velocity.y, _rigidbody.velocity.z);
    }

    private void HandleAirMovement()
    {
        Vector2 horizontalVelocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.z);
        if (horizontalVelocity.magnitude < _airMovementThreshold)
            return;

        Vector3 motion = new Vector3(
            _horizontalInput * _airSideAcceleration * Mathf.Pow(Time.fixedDeltaTime, 2),
            _verticalInput * _airVerticalAcceleration * Mathf.Pow(Time.fixedDeltaTime, 2),
            0f);

        _rigidbody.velocity += motion;
    }

    private void HandleAirLookForward()
    {
        Vector2 horizontalVelocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.z);
        if (horizontalVelocity.magnitude < _airLookForwardThreshold)
            return;

        _transform.forward = Vector3.Slerp(_transform.forward, _rigidbody.velocity.normalized, _airLookForwardSpeed * Time.deltaTime);
    }
}
