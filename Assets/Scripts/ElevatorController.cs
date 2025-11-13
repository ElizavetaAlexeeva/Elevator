using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ElevatorController : MonoBehaviour
{
    [Header("World Movement")]
    public Transform worldMover;
    public float floorHeight = 3.05f;
    public float moveSpeed = 2f;
    public int totalFloors = 5;

    [Header("Doors (Optional)")]
    public ElevatorDoors doors;

    [Header("Timing")]
    public float minWaitAfterArrival = 2.5f; // ← НОВОЕ: сколько ждать на этаже

    [Header("UI & Feedback")]
    public Text floorDisplay;
    public AudioClip arrivalSound;
    [Range(0, 1)] public float arrivalVolume = 1f;

    public AudioClip elevatorMoveSound;
    [Range(0, 1)] public float elevatorMoveVolume = 1f;

    private int currentFloor = 0;
    private bool isMoving = false;
    private float nextAllowedInteractionTime = 0f; // ← НОВОЕ
    private AudioSource audioSource;

    public AudioSource flooSound;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        UpdateFloorDisplay();
    }

    // Проверка: можно ли сейчас запросить этаж?
    public bool CanRequestFloor()
    {
        return !isMoving && Time.time >= nextAllowedInteractionTime;
    }

    public void GoToFloor(int targetFloor)
    {
        if (targetFloor == currentFloor || !CanRequestFloor() || worldMover == null)
            return;

        StartCoroutine(MoveToWorldFloor(targetFloor));
    }

    IEnumerator MoveToWorldFloor(int targetFloor)
    {
        isMoving = true;

        // Закрыть двери (если есть)
        doors?.OpenDoors();
        if (arrivalSound != null)
            audioSource.PlayOneShot(arrivalSound, arrivalVolume);
        yield return new WaitForSeconds(4.5f);

        // Движение
        if (elevatorMoveSound != null)
            audioSource.PlayOneShot(elevatorMoveSound, elevatorMoveVolume);

        float targetY = -targetFloor * floorHeight;
        Vector3 startPos = worldMover.position;
        while (Vector3.Distance(worldMover.position, new Vector3(startPos.x, targetY, startPos.z)) > 0.01f)
        {
            worldMover.position = Vector3.MoveTowards(
                worldMover.position,
                new Vector3(startPos.x, targetY, startPos.z),
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        worldMover.position = new Vector3(startPos.x, targetY, startPos.z);
        currentFloor = targetFloor;
        UpdateFloorDisplay();
     //   audioSource.Stop(elevatorMoveSound);
        // Звук прибытия
        if (arrivalSound != null)
            audioSource.PlayOneShot(arrivalSound, arrivalVolume);

        // Открыть двери
        if (doors != null)
        {
            doors.CloseDoors();
            
            yield return new WaitForSeconds(2f);
           // doors.OpenDoors();
        }

        // 🔒 Устанавливаем время, после которого можно снова нажимать
        flooSound.Play();
        nextAllowedInteractionTime = Time.time + minWaitAfterArrival;

        isMoving = false;
    }
    public void GoToNextFloor()
    {
        flooSound.Stop();
        GoToFloor(Mathf.Min(currentFloor + 1, totalFloors - 1));
      //  doors.OpenDoors();

    }
    public bool IsMoving()
    {
        return isMoving;
    }
    public int GetCurrentFloor()
    {
        return currentFloor;
    }
    public void GoToPreviousFloor()
    {
        GoToFloor(Mathf.Max(currentFloor - 1, 0));
    }
    void UpdateFloorDisplay()
    {
        if (floorDisplay != null)
        {
            floorDisplay.text = $"Этаж: {currentFloor}";
        }
    }
}
