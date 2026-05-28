using UnityEngine;

public class HelicopterBlades : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 2000f;

    void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}