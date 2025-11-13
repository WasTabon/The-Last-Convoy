using UnityEngine;

public class HelicopterBlades : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 2000f; // Швидкість обертання в градусах за секунду

    void Update()
    {
        // Обертання навколо осі Z
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}