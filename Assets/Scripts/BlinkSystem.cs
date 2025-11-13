using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class BlinkSystem : MonoBehaviour
{
    [Header("Blink Settings")]
    public float minAutoBlinkInterval = 8f;   // Начальный интервал авто-моргания
    public float maxAutoBlinkInterval = 3f;   // Минимальный интервал (со временем уменьшается)
    public float blinkDuration = 0.3f;        // Сколько длится моргание (экран чёрный)
    public float forcedBlinkPenalty = 1.5f;   // Во сколько раз ближе подбираются монстры при вынужденном моргании

    [Header("UI References")]
    public Image topMask;   // Чёрная полоса сверху
    public Image bottomMask;// Чёрная полоса снизу

    private float nextBlinkTime = 0f;
    private bool isBlinking = false;
    private float currentInterval;

    public static BlinkSystem Instance; // Singleton для доступа из монстров

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        currentInterval = minAutoBlinkInterval;
        ScheduleNextBlink();
    }

    void Update()
    {
        // Ручное моргание
        if (Input.GetKeyDown(KeyCode.Space) && !isBlinking)
        {
            StartCoroutine(Blink(false)); // false = ручное моргание
        }

        // Авто-моргание
        if (Time.time >= nextBlinkTime && !isBlinking)
        {
            StartCoroutine(Blink(true)); // true = вынужденное моргание
        }
    }

    IEnumerator Blink(bool isForced)
    {
        isBlinking = true;

        // Сигнал всем монстрам
        NotifyMonstersAboutBlink(isForced);

        // Анимация закрытия глаз
        float elapsed = 0f;
        while (elapsed < blinkDuration / 2f)
        {
            float progress = elapsed / (blinkDuration / 2f);
            SetMaskFillAmount(progress);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Полный чёрный экран (глаза "закрыты")
        SetMaskFillAmount(1f);
        yield return new WaitForSeconds(0.05f); // мгновенная пауза в закрытом состоянии

        // Анимация открытия глаз
        elapsed = 0f;
        while (elapsed < blinkDuration / 2f)
        {
            float progress = 1f - (elapsed / (blinkDuration / 2f));
            SetMaskFillAmount(progress);
            elapsed += Time.deltaTime;
            yield return null;
        }

        SetMaskFillAmount(0f); // глаза открыты
        isBlinking = false;

        // Обновляем интервал (частота растёт!)
        DecreaseBlinkInterval();
        ScheduleNextBlink();
    }

    void SetMaskFillAmount(float fill)
    {
        if (topMask != null) topMask.fillAmount = fill;
        if (bottomMask != null) bottomMask.fillAmount = fill;
    }

    void ScheduleNextBlink()
    {
        nextBlinkTime = Time.time + currentInterval;
    }

    void DecreaseBlinkInterval()
    {
        // Постепенно уменьшаем интервал (но не меньше maxAutoBlinkInterval)
        currentInterval = Mathf.Max(maxAutoBlinkInterval, currentInterval * 0.92f);
    }

    void NotifyMonstersAboutBlink(bool isForced)
    {
        Monster[] monsters = FindObjectsOfType<Monster>();
        foreach (var monster in monsters)
        {
            monster.OnPlayerBlinked(isForced); // ✅ передаём только флаг
        }
    }
}
