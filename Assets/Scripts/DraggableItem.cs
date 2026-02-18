using System;
using System.Collections;
using UnityEngine;

public class DraggableItem : MonoBehaviour
{
    public ItemType Type;
    public Slot CurrentSlot { get; set; }

    [Header("Drop & Bounce")]
    public float flyTime = 0.08f;          // долёт к слоту
    public float dropDownTime = 0.10f;     // падение вниз
    public float bounceUpTime = 0.12f;     // отскок вверх в слот

    public float dropDownUnits = 0.35f;    // насколько ниже слота “провалится”
    public float bounceOvershootUnits = 0.08f; // чуть выше слота (опционально)

    private Coroutine moveRoutine;

    public void MoveToSlot(Slot slot, Action onArrived = null)
    {
        if (slot == null) return;

        // фиксируем связь предмет<->слот сразу
        slot.SetItem(this);

        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(DropBounceRoutine(slot, onArrived));
    }

    private IEnumerator DropBounceRoutine(Slot slot, Action onArrived)
    {
        // на время анимации можно выключить коллайдер, чтобы не пере-схватили
        SetCollidersEnabled(false);

        Vector3 startPos = transform.position;
        Vector3 endPos   = slot.SnapPosition;

        // Точки траектории
        Vector3 belowPos     = endPos + Vector3.down * dropDownUnits;
        Vector3 overshootPos = endPos + Vector3.up   * bounceOvershootUnits;

        // 1) Быстро долетаем к слоту (ease-out)
        yield return Move(startPos, endPos, flyTime, EaseOutCubic);

        // 2) “Проваливаемся” ниже (ease-in — ускорение вниз)
        yield return Move(endPos, belowPos, dropDownTime, EaseInCubic);

        // 3) Подскок вверх: сначала в overshoot, потом обратно в end
        // 3a) вверх с ease-out (быстро старт, мягкая остановка)
        yield return Move(belowPos, overshootPos, bounceUpTime * 0.65f, EaseOutCubic);

        // 3b) обратно в слот (коротко)
        yield return Move(overshootPos, endPos, bounceUpTime * 0.35f, EaseInOutCubic);

        // финальная фиксация
        transform.position = new Vector3(slot.SnapPosition.x, slot.SnapPosition.y-0.1f);
        transform.SetParent(slot.transform);
        //transform.localPosition = Vector3.zero;

        SetCollidersEnabled(true);

        moveRoutine = null;
        onArrived?.Invoke();
    }

    private IEnumerator Move(Vector3 from, Vector3 to, float time, Func<float, float> ease)
    {
        if (time <= 0f)
        {
            transform.position = to;
            yield break;
        }

        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / time);
            float e = ease != null ? ease(k) : k;

            transform.position = Vector3.LerpUnclamped(from, to, e);
            yield return null;
        }

        transform.position = to;
    }

    // Easing
    private float EaseInCubic(float t) => t * t * t;
    private float EaseOutCubic(float t)
    {
        float u = 1f - t;
        return 1f - u * u * u;
    }
    private float EaseInOutCubic(float t)
        => t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;

    private void SetCollidersEnabled(bool enabled)
    {
        foreach (var c in GetComponentsInChildren<Collider2D>(true))
            c.enabled = enabled;
    }
}
