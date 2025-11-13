using System.Collections;
using UnityEngine;

public class ElevatorDoors : MonoBehaviour
{
    [Header("Door References")]
    public Transform leftDoor;
    public Transform rightDoor;

    [Header("Animation Settings")]
    public float openDistance = 0.6f;   // На сколько сдвигать двери (в метрах)
    public float moveSpeed = 1.5f;      // Скорость открытия/закрытия
    public float openDuration = 2f;     // Сколько двери остаются открытыми

    private Vector3 leftClosedPos;
    private Vector3 rightClosedPos;
    private bool isOpen = false;

    void Start()
    {
        if (leftDoor == null || rightDoor == null)
        {
            Debug.LogError("Assign Left and Right Doors in Inspector!");
            enabled = false;
            return;
        }

        // Сохраняем позиции "закрыто"
        leftClosedPos = leftDoor.localPosition;
        rightClosedPos = rightDoor.localPosition;
    }

    // Вызывается извне (например, после остановки лифта)
    public void OpenDoors()
    {
        if (!isOpen)
        {
            isOpen = true;
            StartCoroutine(MoveDoors(true));
        }
    }

    public void CloseDoors()
    {
        if (isOpen)
        {
            isOpen = false;
            StartCoroutine(MoveDoors(false));
        }
    }

    // Автоматическое открытие → пауза → закрытие
    public IEnumerator AutoOpenAndClose()
    {
        OpenDoors();
        yield return new WaitForSeconds(openDuration);
        CloseDoors();
    }

    IEnumerator MoveDoors(bool open)
    {
        Vector3 leftTarget = open ? leftClosedPos + Vector3.left * openDistance : leftClosedPos;
        Vector3 rightTarget = open ? rightClosedPos + Vector3.right * openDistance : rightClosedPos;

        float elapsed = 0f;
        Vector3 leftStart = leftDoor.localPosition;
        Vector3 rightStart = rightDoor.localPosition;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * moveSpeed;
            leftDoor.localPosition = Vector3.Lerp(leftStart, leftTarget, elapsed);
            rightDoor.localPosition = Vector3.Lerp(rightStart, rightTarget, elapsed);
            yield return null;
        }

        // Точно выравниваем
        leftDoor.localPosition = leftTarget;
        rightDoor.localPosition = rightTarget;
    }
}

