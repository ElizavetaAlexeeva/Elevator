using UnityEngine;
using UnityEngine.UI;

public class ButtonDetect : MonoBehaviour
{

    [Header("Raycast Settings")]
    public float raycastDistance = 10f;
    public LayerMask buttonLayer; // Опционально: слой кнопок

    private Camera playerCamera;
    private ElevatorButton currentButton = null;

    void Start()
    {
        playerCamera = GetComponent<Camera>();
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    void Update()
    {
        HandleRaycast();
    }

    void HandleRaycast()
    {
        ElevatorController controller = FindObjectOfType<ElevatorController>();
        bool canInteract = controller == null || controller.CanRequestFloor();

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (canInteract && Physics.Raycast(ray, out hit, raycastDistance, buttonLayer))
        {
            ElevatorButton button = hit.collider.GetComponent<ElevatorButton>();
            if (button != null)
            {
                if (currentButton != button)
                {
                    currentButton?.OnDeselect();
                    currentButton = button;
                    currentButton.OnSelect();
                }

                if (Input.GetMouseButtonDown(0))
                {
                    currentButton.OnClick();
                }
                return;
            }
        }

        if (currentButton != null)
        {
            currentButton.OnDeselect();
            currentButton = null;
        }

    }
}

