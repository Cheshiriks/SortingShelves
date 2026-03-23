using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using YG;

public class CollectionRewardMenuController : MonoBehaviour
{
    [SerializeField] private GameObject menuCollectionPresent;
    [SerializeField] private Image itemImage;
    [SerializeField] private Image itemImageBlack;
    [SerializeField] private TextMeshProUGUI progressText;

    [Header("Animation")]
    [SerializeField] private float fillAnimTime = 0.5f;
    [SerializeField] private float fillDelay = 0.35f;
    
    [Header("Complete animation")]
    [SerializeField] private float completeScale = 1.2f;
    [SerializeField] private float completeScaleUpTime = 0.2f;
    [SerializeField] private float completeScaleDownTime = 0.2f;

    private Coroutine fillRoutine;

    private void Awake()
    {
        menuCollectionPresent.SetActive(false);
    }
    
    public void Show(CollectionItemData item, int currentCount)
    {
        menuCollectionPresent.SetActive(true);

        int newCount = currentCount;
        int oldCount = currentCount - 1;
        
        itemImageBlack.sprite = item.sprite;
        itemImage.sprite = item.sprite;

        float from = Mathf.Clamp01(oldCount / 4f);
        float to = Mathf.Clamp01(newCount / 4f);

        itemImage.fillAmount = from;
        if (newCount < 4)
        {
            progressText.text = "ЧАСТЬ " + newCount + "/4";
        }
        else
        {
            progressText.text = "ПРЕДМЕТ СОБРАН!";
        }

        if (fillRoutine != null)
            StopCoroutine(fillRoutine);

        fillRoutine = StartCoroutine(AnimateFill(from, to, currentCount ));
    }
    
    private IEnumerator AnimateFill(float from, float to, int newCount)
    {
        if (fillDelay > 0f)
            yield return new WaitForSeconds(fillDelay);
        
        float t = 0f;

        while (t < fillAnimTime)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / fillAnimTime);

            // плавность
            float eased = 1f - Mathf.Pow(1f - k, 3f);

            itemImage.fillAmount = Mathf.Lerp(from, to, eased);

            yield return null;
        }

        itemImage.fillAmount = to;
        
        // Если собрали 4/4 — делаем pop анимацию
        if (newCount >= 4)
        {
            yield return PlayCompletePop();
        }
        
        fillRoutine = null;
    }
    
    private IEnumerator PlayCompletePop()
    {
        RectTransform rt = itemImage.rectTransform;
        Vector3 baseScale = Vector3.one;
        Vector3 targetScale = Vector3.one * completeScale;

        float t = 0f;
        while (t < completeScaleUpTime)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / completeScaleUpTime);
            float eased = 1f - Mathf.Pow(1f - k, 3f);

            rt.localScale = Vector3.Lerp(baseScale, targetScale, eased);
            yield return null;
        }

        t = 0f;
        while (t < completeScaleDownTime)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / completeScaleDownTime);
            float eased = 1f - Mathf.Pow(1f - k, 2f);

            rt.localScale = Vector3.Lerp(targetScale, baseScale, eased);
            yield return null;
        }

        rt.localScale = baseScale;
    }

    public void Hide()
    {
        // показываем рекламу
        YG2.InterstitialAdvShow();
        
        menuCollectionPresent.SetActive(false);
    }

}
