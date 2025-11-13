using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera; // Перетащи сюда камеру из дочерних объектов

    [Header("Sensitivity")]
    public float mouseSensitivity = 2f;

    [Header("Vertical Look Limits")]
    public float minYAngle = -70f;
    public float maxYAngle = 70f;

    private float verticalRotation = 0f;

    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
            if (playerCamera == null)
            {
                Debug.LogError("No camera assigned and no child camera found!");
                enabled = false;
                return;
            }
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleLook();
    }

    void HandleLook()
    {
        // Получаем движение мыши
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Горизонтальный поворот — тело игрока (весь объект)
        transform.Rotate(Vector3.up * mouseX);

        // Вертикальный поворот — только камера
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, minYAngle, maxYAngle);

        // Применяем поворот к камере
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}
