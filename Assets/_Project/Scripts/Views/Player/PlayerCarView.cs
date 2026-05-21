using UnityEngine;
using Zenject;

[RequireComponent(typeof(Rigidbody))]
public class PlayerCarView : MonoBehaviour
{
    [Header("Hover Settings")]
    [SerializeField] private float _hoverHeight = 0.5f;
    [SerializeField] private float _hoverForce = 50f;
    [SerializeField] private float _hoverDamping = 5f;

    [Header("Ground Detection")]
    [SerializeField] private float _groundCheckDistance = 3f;
    [SerializeField] private LayerMask _groundMask = ~0;
    [SerializeField] private float _alignToGroundSpeed = 8f;

    [Header("Physics")]
    [SerializeField] private float _extraGravity = 20f;

    private PlayerCarModel _model;
    private Rigidbody _rigidbody;
    private bool _isInjected;
    private bool _isGrounded;
    private Vector3 _groundNormal = Vector3.up;
    private float _groundDistance;

    [Inject]
    public void Construct(PlayerCarModel model)
    {
        _model = model;
        _isInjected = true;
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        _rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void FixedUpdate()
    {
        if (!_isInjected) return;

        CheckGround();
        ApplyHover();

        if (_isGrounded)
        {
            ApplyMovement();
            ApplyRotation();
            AlignToGround();
        }
        else
        {
            ApplyExtraGravity();
        }
    }

    private void CheckGround()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, _groundCheckDistance, _groundMask))
        {
            _isGrounded = true;
            _groundNormal = hit.normal;
            _groundDistance = hit.distance - 0.1f;
        }
        else
        {
            _isGrounded = false;
            _groundNormal = Vector3.up;
            _groundDistance = _groundCheckDistance;
        }
    }

    private void ApplyHover()
    {
        if (!_isGrounded) return;

        float heightError = _hoverHeight - _groundDistance;
        float verticalVelocity = _rigidbody.velocity.y;

        float hoverAcceleration = (heightError * _hoverForce) - (verticalVelocity * _hoverDamping);

        _rigidbody.AddForce(Vector3.up * hoverAcceleration, ForceMode.Acceleration);
    }

    private void ApplyMovement()
    {
        Vector3 moveDirection = Vector3.ProjectOnPlane(transform.forward, _groundNormal).normalized;

        Vector3 targetVelocity = moveDirection * _model.CurrentSpeed;
        Vector3 currentHorizontalVelocity = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);
        Vector3 velocityDifference = new Vector3(targetVelocity.x, 0f, targetVelocity.z) - currentHorizontalVelocity;

        _rigidbody.AddForce(velocityDifference, ForceMode.VelocityChange);
    }

    private void ApplyRotation()
    {
        float turnAmount = _model.GetTurnAmount(Time.fixedDeltaTime);

        if (Mathf.Abs(turnAmount) > 0.01f)
        {
            Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);
            _rigidbody.MoveRotation(_rigidbody.rotation * turnRotation);
        }
    }

    private void AlignToGround()
    {
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, _groundNormal).normalized;

        if (forward.sqrMagnitude < 0.01f)
        {
            forward = transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(forward, _groundNormal);

        float currentYaw = _rigidbody.rotation.eulerAngles.y;
        Quaternion yawOnly = Quaternion.Euler(targetRotation.eulerAngles.x, currentYaw, targetRotation.eulerAngles.z);

        Quaternion newRotation = Quaternion.Slerp(_rigidbody.rotation, yawOnly, Time.fixedDeltaTime * _alignToGroundSpeed);
        _rigidbody.MoveRotation(newRotation);
    }

    private void ApplyExtraGravity()
    {
        _rigidbody.AddForce(Vector3.down * _extraGravity, ForceMode.Acceleration);
    }
}