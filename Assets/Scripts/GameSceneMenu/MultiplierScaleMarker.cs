using UnityEngine;

public class MultiplierScaleMarker : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RectTransform scaleRect;   // Scale (Image)
    [SerializeField] private RectTransform markerRect;  // Marker (Image, child of Scale)

    [Header("Motion")]
    [SerializeField] private float speed = 250f; // пикселей в секунду (UI units)

    private float minX;
    private float maxX;

    private float currentX;
    private int direction = 1; // 1 -> вправо, -1 -> влево
    private bool isRunning;

    private void Awake()
    {
        if (!scaleRect) scaleRect = GetComponent<RectTransform>();
        RecalculateBounds();
        ResetToStart();
    }

    private void OnRectTransformDimensionsChange()
    {
        // если канвас/скейл меняют размеры (разные разрешения) — пересчёт границ
        RecalculateBounds();
        currentX = Mathf.Clamp(currentX, minX, maxX);
        SetMarkerX(currentX);
    }

    private void Start()
    {
        StartMoving();
    }
    
    private void OnEnable()
    {
        ResetToStart();
        StartMoving();
    }

    private void Update()
    {
        if (!isRunning) return;

        currentX += direction * speed * Time.unscaledDeltaTime;

        // отражение на границах (движение туда-сюда)
        if (currentX > maxX)
        {
            currentX = maxX;
            direction = -1;
        }
        else if (currentX < minX)
        {
            currentX = minX;
            direction = 1;
        }

        SetMarkerX(currentX);
    }

    private void RecalculateBounds()
    {
        // Ширина шкалы в локальных UI единицах
        float scaleWidth = scaleRect.rect.width;

        // Чтобы маркер не вылезал за края — учтём его ширину
        float markerHalf = markerRect.rect.width * 0.5f;

        // Диапазон по X относительно центра Scale (при pivot 0.5 это идеально)
        minX = -scaleWidth * 0.5f + markerHalf;
        maxX =  scaleWidth * 0.5f - markerHalf;
    }

    private void SetMarkerX(float x)
    {
        var ap = markerRect.anchoredPosition;
        ap.x = x;
        markerRect.anchoredPosition = ap;
    }

    private void ResetToStart()
    {
        // стартуем справа (можешь поменять на minX если нужно)
        currentX = minX;
        direction = 1;
        SetMarkerX(currentX);
    }

    // --- Публичные методы для кнопок/меню ---

    public void StartMoving()
    {
        isRunning = true;
    }

    public void StopMoving()
    {
        isRunning = false;

        float percent = GetCurrentPercent(); // 0..100
        int segment = GetSegmentIndex(percent); // 1..5 (по 20%)

        Debug.Log($"Marker stopped at: {percent:0.0}% (segment {segment})");
    }

    public float GetCurrentPercent()
    {
        // 0% = minX, 100% = maxX
        float t = Mathf.InverseLerp(minX, maxX, markerRect.anchoredPosition.x);
        return t * 100f;
    }

    private int GetSegmentIndex(float percent)
    {
        // 0-20 => 1, 20-40 => 2 ... 80-100 => 5
        // На 100% должно быть 5
        int idx = Mathf.FloorToInt(percent / 20f) + 1;
        return Mathf.Clamp(idx, 1, 5);
    }
}
