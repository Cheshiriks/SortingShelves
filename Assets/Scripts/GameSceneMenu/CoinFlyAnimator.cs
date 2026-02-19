using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CoinFlyAnimator : MonoBehaviour
{
    [Header("References")]
    public RectTransform vfxLayer;          // VFXLayer (растянутый на весь экран)
    public RectTransform coinsIconTarget;   // RectTransform иконки монет сверху слева
    public Image coinPrefab;               // prefab CoinFlyUI (Image)

    [Header("Spawn")]
    public int coinsCount = 3;
    public float spawnRadius = 60f;        // радиус разлёта в пикселях

    [Header("Motion")]
    public float flyTime = 0.35f;
    public float arcHeight = 120f;         // высота дуги (пиксели)
    public float startScale = 1.0f;
    public float endScale = 0.5f;

    [Header("Finish")]
    public float popScale = 1.15f;
    public float popTime = 0.08f;
    
    [Header("Delay")]
    public float startDelay = 0.5f;

    public void PlayFrom(RectTransform from, System.Action onArrived = null)
    {
        StartCoroutine(PlayRoutine(from, onArrived));
    }

    private IEnumerator PlayRoutine(RectTransform from, System.Action onArrived)
    {
        if (!vfxLayer || !coinsIconTarget || !coinPrefab) yield break;

        Vector2 start = WorldToCanvasPos(from.position);
        Vector2 end   = WorldToCanvasPos(coinsIconTarget.position);

        int alive = 0;

        for (int i = 0; i < coinsCount; i++)
        {
            alive++;
            var img = Instantiate(coinPrefab, vfxLayer);
            img.raycastTarget = false;

            RectTransform rt = img.rectTransform;
            rt.anchoredPosition = start + Random.insideUnitCircle * spawnRadius;
            rt.localScale = Vector3.one * startScale;

            // небольшой рандом по времени, чтобы не летели идеально синхронно
            float t = flyTime * Random.Range(0.7f, 1.2f);

            StartCoroutine(FlyOne(rt, end, t, () => alive--));
        }

        // ждём пока все долетят
        while (alive > 0) yield return null;

        onArrived?.Invoke();
        
        // небольшой pop на иконке
        yield return PopTarget(coinsIconTarget);
    }

    private IEnumerator FlyOne(RectTransform rt, Vector2 end, float time, System.Action onDone)
    {
        // Пауза перед полётом
        if (startDelay > 0f)
            yield return new WaitForSeconds(startDelay);
        
        Vector2 start = rt.anchoredPosition;

        // контрольная точка для дуги (красивый подъём)
        Vector2 mid = (start + end) * 0.5f + Vector2.up * arcHeight;

        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / time);

            // easing (быстро стартуем, мягко прилетаем)
            float e = EaseOutCubic(k);

            // кривая Безье (квадратичная): start -> mid -> end
            Vector2 p = Bezier(start, mid, end, e);
            rt.anchoredPosition = p;

            float s = Mathf.Lerp(startScale, endScale, e);
            rt.localScale = Vector3.one * s;

            yield return null;
        }

        rt.anchoredPosition = end;
        rt.localScale = Vector3.one * endScale;

        Destroy(rt.gameObject);
        onDone?.Invoke();
    }

    private IEnumerator PopTarget(RectTransform target)
    {
        Vector3 baseScale = target.localScale;
        Vector3 up = baseScale * popScale;

        float t = 0f;
        while (t < popTime)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / popTime);
            target.localScale = Vector3.Lerp(baseScale, up, EaseOutCubic(k));
            yield return null;
        }

        t = 0f;
        while (t < popTime)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / popTime);
            target.localScale = Vector3.Lerp(up, baseScale, EaseInCubic(k));
            yield return null;
        }

        target.localScale = baseScale;
    }

    private Vector2 WorldToCanvasPos(Vector3 worldPos)
    {
        // переводим world->screen->canvas local
        Canvas canvas = vfxLayer.GetComponentInParent<Canvas>();
        Camera cam = (canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : canvas.worldCamera;

        Vector2 screen = RectTransformUtility.WorldToScreenPoint(cam, worldPos);

        RectTransform canvasRT = canvas.transform as RectTransform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRT, screen, cam, out var localPoint);

        // localPoint относительно canvas, а нам нужно относительно vfxLayer (она тоже в canvas), поэтому ок
        return localPoint;
    }

    private static Vector2 Bezier(Vector2 a, Vector2 b, Vector2 c, float t)
    {
        // (1-t)^2 a + 2(1-t)t b + t^2 c
        float u = 1f - t;
        return u * u * a + 2f * u * t * b + t * t * c;
    }

    private static float EaseOutCubic(float t)
    {
        float u = 1f - t;
        return 1f - u * u * u;
    }

    private static float EaseInCubic(float t) => t * t * t;
}
