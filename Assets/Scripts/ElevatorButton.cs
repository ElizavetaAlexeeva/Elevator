using UnityEngine;

public class ElevatorButton : MonoBehaviour
{
    [Header("Button Settings")]
    public int targetFloor = 0;

    [Header("Visual")]
    public Color highlightColor = Color.yellow;
    public Color defaultColor = Color.white;

    [Header("Audio")]
    public AudioClip clickSound;
    public float volume = 1f;

    private Material originalMaterial;
    private MeshRenderer renderer;
    private AudioSource audioSource;

    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            originalMaterial = renderer.material;
            SetColor(defaultColor);
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
    }

    public void OnSelect()
    {
        SetColor(highlightColor);
    }

    public void OnDeselect()
    {
        SetColor(defaultColor);
    }

    public void OnClick()
    {
        // 🔒 Проверяем, не движется ли лифт
        ElevatorController controller = FindObjectOfType<ElevatorController>();
        if (controller != null && !controller.CanRequestFloor())
        {
            // Опционально: проиграть "заблокированный" звук
            // Debug.Log("Лифт уже в движении!");
            return;
        }

        if (clickSound != null)
            audioSource.PlayOneShot(clickSound, volume);

        Debug.Log($"Выбран этаж {targetFloor}");

        if (controller != null)
        {
            controller.GoToNextFloor();
        }
    }

    void SetColor(Color color)
    {
        if (renderer != null && renderer.material != null)
        {
            renderer.material.color = color;
        }
    }
}
