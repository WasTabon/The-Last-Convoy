using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [Header("Mouse Settings")]
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Transform playerBody;

    [Header("Camera Limits")]
    [SerializeField] private float minVerticalAngle = -90f;
    [SerializeField] private float maxVerticalAngle = 90f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    void Start()
    {
        // Блокуємо і ховаємо курсор
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Зберігаємо поточне обертання
        Vector3 currentRotation = transform.localEulerAngles;
        xRotation = currentRotation.x;
        yRotation = currentRotation.y;
    }

    void Update()
    {
        // Отримуємо рух миші
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Обертання по вертикалі (вгору-вниз) - вісь X
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);

        // Обертання по горизонталі (ліворуч-праворуч) - вісь Y
        yRotation += mouseX;

        // Застосовуємо обертання до камери по обох осях
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

        // Якщо є playerBody, обертаємо його теж
        if (playerBody != null)
        {
            playerBody.Rotate(Vector3.up * mouseX);
            
            // Компенсуємо обертання гравця в камері
            yRotation -= mouseX;
        }
    }
}