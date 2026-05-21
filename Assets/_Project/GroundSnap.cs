using UnityEngine;

public class GroundSnap : MonoBehaviour
{
    [SerializeField] private float _raycastHeight = 5f;
    [SerializeField] private float _raycastDistance = 10f;
    [SerializeField] private LayerMask _groundLayers = -1;
    [SerializeField] private float _heightOffset = 0f;
    [SerializeField] private bool _alignToSurface = true;
    [SerializeField] private float _alignmentSpeed = 10f;

    private void LateUpdate()
    {
        SnapToGround();
    }

    private void SnapToGround()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * _raycastHeight;
        Ray ray = new Ray(rayOrigin, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, _raycastDistance, _groundLayers))
        {
            Vector3 newPosition = transform.position;
            newPosition.y = hit.point.y + _heightOffset;
            transform.position = newPosition;

            if (_alignToSurface)
            {
                AlignToSurface(hit.normal);
            }
        }
    }

    private void AlignToSurface(Vector3 surfaceNormal)
    {
        Vector3 forward = transform.forward;
        Vector3 right = Vector3.Cross(surfaceNormal, forward).normalized;
        Vector3 alignedForward = Vector3.Cross(right, surfaceNormal).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(alignedForward, surfaceNormal);

        float currentYaw = transform.eulerAngles.y;
        Quaternion yawRotation = Quaternion.Euler(0f, currentYaw, 0f);

        Quaternion finalRotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.Euler(targetRotation.eulerAngles.x, currentYaw, targetRotation.eulerAngles.z),
            Time.deltaTime * _alignmentSpeed
        );

        transform.rotation = finalRotation;
    }
}
