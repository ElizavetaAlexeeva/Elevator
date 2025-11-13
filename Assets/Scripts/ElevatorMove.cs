using System.Collections;
using UnityEngine;

public class ElevatorMove : MonoBehaviour
{
    [Header("Floors")]
    public float floorHeight = 3.05f;        // Высота одного этажа
    public int totalFloors = 5;           // Всего этажей (0..4)

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float waitTime = 1f;

    private int currentFloor = 0;
    public ElevatorDoors doors;

    // Вызывается из UI, кнопки или другого скрипта
    public void GoToFloor(int targetFloor)
    {
        if (targetFloor < 0 || targetFloor >= totalFloors || targetFloor == currentFloor)
            return;

        StartCoroutine(MoveTo(targetFloor));
    }

    IEnumerator MoveTo(int targetFloor)
    {
        float startY = transform.position.y;
        float targetY = -targetFloor * floorHeight; // минус, потому что мир движется вниз

        yield return new WaitForSeconds(waitTime);

        while (Mathf.Abs(transform.position.y - targetY) > 0.01f)
        {
            float newY = Mathf.MoveTowards(transform.position.y, targetY, moveSpeed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            yield return null;
        }

        transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
        currentFloor = targetFloor;
        doors.CloseDoors();

        yield return new WaitForSeconds(waitTime);
    }

    // Удобный метод для кнопок (например, в UI)
    public void GoToNextFloor()
    {
        GoToFloor(Mathf.Min(currentFloor + 1, totalFloors - 1));
        doors.OpenDoors();
        
    }

    public void GoToPreviousFloor()
    {
        GoToFloor(Mathf.Max(currentFloor - 1, 0));
    }
}

