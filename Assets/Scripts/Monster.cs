using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("Floor Settings")]
    public int monsterFloor = 0;

    [Header("Movement")]
    public float normalStep = 1f;
    public float forcedStep = 2.5f;

    private Transform player;
    private Transform worldMover; // ← НОВОЕ: ссылка на движущийся мир

    void Start()
    {
        

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        // Найдём worldMover один раз
        ElevatorController controller = FindObjectOfType<ElevatorController>();
        if (controller != null)
        {
            worldMover = controller.worldMover;
        }
    }

    // Вызывается только ПОСЛЕ полной остановки лифта
    public void OnPlayerBlinked(bool isForced)
    {
        ElevatorController elevator = FindObjectOfType<ElevatorController>();
        if (elevator == null) return;

        // Дополнительная защита: не двигаться, если лифт ещё движется
        if (elevator.IsMoving())
        {
            Debug.LogWarning("Monster tried to move while elevator is moving!");
            return;
        }

        int playerFloor = elevator.GetCurrentFloor();
        if (monsterFloor != playerFloor) return;

        float step = isForced ? forcedStep : normalStep;
        MoveCloserToPlayer(step);
    }

    void MoveCloserToPlayer(float distance)
    {
        if (player == null || worldMover == null)
        {
            Debug.LogError("Monster: player or worldMover is null!");
            return;
        }

        // 1. Считаем разницу в позициях
        Vector3 direction = player.position - transform.position;

        // 2. ИГНОРИРУЕМ высоту (Y) — ДО нормализации!
        direction.z = 0;
        direction.x = 0;

        // 3. Если игрок прямо над/под — не двигаемся
        if (direction.magnitude < 0.1f)
            return;

        // 4. Нормализуем
        direction = direction.normalized;

        // 5. Преобразуем в локальное пространство worldMover
        Vector3 localOffset = worldMover.InverseTransformDirection(direction * distance);

        // 6. Двигаем в локальных координатах
        transform.localPosition += localOffset;
    }
}

