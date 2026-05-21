using UnityEngine;
using Zenject;

[RequireComponent(typeof(Rigidbody))]
public class PlayerCarView : MonoBehaviour
{
    [SerializeField] private float _extraGravity = 30f;
    [SerializeField] private float _groundCheckDistance = 1.5f;
    [SerializeField] private LayerMask _groundMask = ~0;

    private PlayerCarModel _model;
    private Rigidbody _rigidbody;
    private bool _isInjected;

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

        bool isGrounded = IsGrounded();

        if (isGrounded)
        {
            ApplyMovement();
            ApplyRotation();
        }

        ApplyExtraGravity();
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, _groundCheckDistance, _groundMask);
    }

    private void ApplyMovement()
    {
        Vector3 horizontalForward = transform.forward;
        horizontalForward.y = 0f;
        horizontalForward.Normalize();

        Vector3 targetHorizontalVelocity = horizontalForward * _model.CurrentSpeed;
        Vector3 currentHorizontalVelocity = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);
        Vector3 velocityDifference = targetHorizontalVelocity - currentHorizontalVelocity;

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

    private void ApplyExtraGravity()
    {
        _rigidbody.AddForce(Vector3.down * _extraGravity, ForceMode.Acceleration);
    }
}