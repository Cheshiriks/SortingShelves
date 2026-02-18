using System;
using System.Collections;
using UnityEngine;

public partial class Shelf : MonoBehaviour
{
    public Slot[] slots; // 3
    
    [Header("VFX")]
    public ParticleSystem confettiPrefab;

    public bool HasTripleMatch()
    {
        if (slots == null || slots.Length < 3) return false;
        if (slots[0].IsEmpty || slots[1].IsEmpty || slots[2].IsEmpty) return false;

        var t0 = slots[0].Item.Type;
        return slots[1].Item.Type == t0 && slots[2].Item.Type == t0;
    }

    public void ClearMatchedTripleAnimated(Action onComplete = null,
        float scaleUp = 1.3f, float upTime = 0.12f, float downTime = 0.12f)
    {
        if (!HasTripleMatch())
        {
            onComplete?.Invoke();
            return;
        }

        StartCoroutine(ClearAnimRoutine(onComplete, scaleUp, upTime, downTime));
    }

    private IEnumerator ClearAnimRoutine(Action onComplete, float scaleUp, float upTime, float downTime)
    {
        var a = slots[0].Item;
        var b = slots[1].Item;
        var c = slots[2].Item;

        // На всякий: если вдруг что-то не так
        if (!a || !b || !c)
        {
            onComplete?.Invoke();
            yield break;
        }

        // Отключаем коллайдеры на время анимации
        DisableItemInteraction(a, false);
        DisableItemInteraction(b, false);
        DisableItemInteraction(c, false);

        var sa = a.transform.localScale;
        var sb = b.transform.localScale;
        var sc = c.transform.localScale;

        // 1) Scale up
        float t = 0f;
        while (t < upTime)
        {
            t += Time.deltaTime;
            float k = (upTime <= 0f) ? 1f : Mathf.Clamp01(t / upTime);

            a.transform.localScale = Vector3.Lerp(sa, sa * scaleUp, k);
            b.transform.localScale = Vector3.Lerp(sb, sb * scaleUp, k);
            c.transform.localScale = Vector3.Lerp(sc, sc * scaleUp, k);

            yield return null;
        }

        // 2) Collapse to zero
        t = 0f;
        Vector3 saUp = sa * scaleUp;
        Vector3 sbUp = sb * scaleUp;
        Vector3 scUp = sc * scaleUp;

        while (t < downTime)
        {
            t += Time.deltaTime;
            float k = (downTime <= 0f) ? 1f : Mathf.Clamp01(t / downTime);

            a.transform.localScale = Vector3.Lerp(saUp, Vector3.zero, k);
            b.transform.localScale = Vector3.Lerp(sbUp, Vector3.zero, k);
            c.transform.localScale = Vector3.Lerp(scUp, Vector3.zero, k);

            yield return null;
        }
        
        // 2.5) Confetti
        // SpawnConfettiAt(GetConfettiPosition(a, b, c));

        // 3) Сначала чистим слоты, потом уничтожаем объекты
        foreach (var s in slots) s.ClearItem();

        if (a) Destroy(a.gameObject);
        if (b) Destroy(b.gameObject);
        if (c) Destroy(c.gameObject);

        onComplete?.Invoke();
    }
    
    private Vector3 GetConfettiPosition(DraggableItem a, DraggableItem b, DraggableItem c)
    {
        // центр трёх предметов
        return (a.transform.position + b.transform.position + c.transform.position) / 3f;
    }

    private void SpawnConfettiAt(Vector3 pos)
    {
        if (!confettiPrefab) return;

        var ps = Instantiate(confettiPrefab, pos, Quaternion.identity);
        ps.Play();

        // удалить после завершения
        float life = ps.main.duration + ps.main.startLifetime.constantMax + 0.2f;
        Destroy(ps.gameObject, life);
    }

    private void DisableItemInteraction(DraggableItem item, bool enabled)
    {
        foreach (var col in item.GetComponentsInChildren<Collider2D>(true))
            col.enabled = enabled;
    }
}
