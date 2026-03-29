using System.Collections;
using TMPro;
using UnityEngine;

public class ShelfLock : MonoBehaviour
{
    [SerializeField] private int lockCount = 0;
    [SerializeField] private GameObject lockObject;   // объект Look
    [SerializeField] private GameObject goldLockObject;   // объект замок
    [SerializeField] private GameObject textObject;  
    [SerializeField] private TextMeshPro textNum;     // TextNum внутри замка

    [Header("Shake")]
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float maxShakeOffsetX = 0.1f; // максимальный сдвиг по X
    [SerializeField] private float shakeFrequency = 7f; // сколько колебаний в секунду
    
    [Header("Unlock Fall")]
    [SerializeField] private float unlockFallDuration = 0.35f;
    [SerializeField] private float unlockFallDistance = 0.35f;

    public bool IsLocked => lockCount > 0;

    private Coroutine animRoutine;
    private Vector3 lockBaseLocalPos;
    private SpriteRenderer[] spriteRenderers;
    private Color[] baseColors;

    private void Awake()
    {
        if (lockObject != null)
        {
            lockBaseLocalPos = lockObject.transform.localPosition;

            spriteRenderers = lockObject.GetComponentsInChildren<SpriteRenderer>(true);
            baseColors = new Color[spriteRenderers.Length];

            for (int i = 0; i < spriteRenderers.Length; i++)
                baseColors[i] = spriteRenderers[i].color;
        }

        RefreshViewImmediate();
    }

    public void SetLockCount(int value)
    {
        lockCount = Mathf.Max(0, value);
        RefreshViewImmediate();
    }

    public void DecreaseLock(int amount = 1)
    {
        if (lockCount <= 0) return;

        lockCount = Mathf.Max(0, lockCount - amount);

        if (textNum)
            textNum.text = lockCount.ToString();

        if (animRoutine != null)
            StopCoroutine(animRoutine);

        if (lockCount > 0)
        {
            animRoutine = StartCoroutine(ShakeLockRoutine());
        }
        else
        {
            animRoutine = StartCoroutine(UnlockFallRoutine());
        }
    }

    private IEnumerator ShakeLockRoutine()
    {
        if (goldLockObject == null) yield break;

        Transform t = goldLockObject.transform;
        t.localPosition = lockBaseLocalPos;

        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;

            float normalized = Mathf.Clamp01(elapsed / shakeDuration);
            float damping = 1f - normalized;

            float offsetX = Mathf.Sin(elapsed * shakeFrequency * Mathf.PI * 2f) * maxShakeOffsetX * damping;

            t.localPosition = lockBaseLocalPos + new Vector3(offsetX, 0f, 0f);

            yield return null;
        }

        t.localPosition = lockBaseLocalPos;
        animRoutine = null;
    }

    private IEnumerator UnlockFallRoutine()
    {
        if (goldLockObject == null) yield break;
        textObject.SetActive(false);

        Transform t = goldLockObject.transform;
        t.localPosition = lockBaseLocalPos;
        SetSpritesAlpha(1f);

        Vector3 from = lockBaseLocalPos;
        Vector3 to = lockBaseLocalPos + Vector3.down * unlockFallDistance;

        float elapsed = 0f;
        while (elapsed < unlockFallDuration)
        {
            elapsed += Time.deltaTime;
            float k = Mathf.Clamp01(elapsed / unlockFallDuration);
            float eased = 1f - Mathf.Pow(1f - k, 3f);

            t.localPosition = Vector3.Lerp(from, to, eased);
            SetSpritesAlpha(1f - eased);

            yield return null;
        }

        // возвращаем в исходное состояние для следующего использования
        t.localPosition = lockBaseLocalPos;
        RestoreSpriteColors();

        lockObject.SetActive(false);
        animRoutine = null;
    }

    private void RefreshViewImmediate()
    {
        if (lockObject)
        {
            lockObject.SetActive(lockCount > 0);
            lockObject.transform.localPosition = lockBaseLocalPos;
        }

        if (textNum)
            textNum.text = lockCount.ToString();

        RestoreSpriteColors();
    }

    private void SetSpritesAlpha(float alpha)
    {
        if (spriteRenderers == null) return;

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            Color c = baseColors[i];
            c.a *= alpha;
            spriteRenderers[i].color = c;
        }
    }

    private void RestoreSpriteColors()
    {
        if (spriteRenderers == null || baseColors == null) return;

        for (int i = 0; i < spriteRenderers.Length; i++)
            spriteRenderers[i].color = baseColors[i];
    }
}