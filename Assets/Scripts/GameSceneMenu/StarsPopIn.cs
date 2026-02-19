using System.Collections;
using UnityEngine;

public class StarsPopIn : MonoBehaviour
{
    [SerializeField] private RectTransform[] stars;   // Star_01, Star_02, Star_03
    
    [SerializeField] private RectTransform menuButton; // кнопка МЕНЮ
    [SerializeField] private float buttonDelayAfterStars = 1f;
    
    [SerializeField] private float popDuration = 0.25f;
    [SerializeField] private float delayBetween = 0.12f;
    [SerializeField] private AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Coroutine _routine;
    private Vector3 _menuScale;
    private float _popMenuDuration = 0.5f;

    private void OnEnable()
    {
        Play();
    }
    
    private void OnDisable()
    {
        menuButton.localScale = _menuScale;
    }

    public void Play(int starsToShow = -1)
    {
        if (_routine != null) StopCoroutine(_routine);
        _menuScale = menuButton.localScale;
        _routine = StartCoroutine(PlayRoutine(starsToShow));
    }

    private IEnumerator PlayRoutine(int starsToShow)
    {
        if (starsToShow < 0) starsToShow = stars.Length;

        // подготовка звезд
        for (int i = 0; i < stars.Length; i++)
        {
            if (!stars[i]) continue;
            stars[i].localScale = Vector3.zero;
            stars[i].gameObject.SetActive(i < starsToShow);
        }

        // подготовка кнопки
        if (menuButton)
        {
            menuButton.localScale = Vector3.zero;
            menuButton.gameObject.SetActive(true);
        }

        // звезды по очереди
        for (int i = 0; i < starsToShow && i < stars.Length; i++)
        {
            yield return StartCoroutine(Pop(stars[i], popDuration));
            yield return new WaitForSecondsRealtime(delayBetween);
        }

        // задержка после последней звезды
        yield return new WaitForSecondsRealtime(buttonDelayAfterStars);

        // появление кнопки
        if (menuButton)
            yield return StartCoroutine(Pop(menuButton, _popMenuDuration, true));
    }

    private IEnumerator Pop(RectTransform target, float duration, bool isButton = false)
    {
        float t = 0f;
        target.localScale = Vector3.zero;

        Vector3 targetVector;
        
        if (isButton)
        {
            targetVector = _menuScale;
        } else
        {
            targetVector = Vector3.one;
        }
        
        while (t < duration)
        {
            t += Time.unscaledDeltaTime; // чтобы работало даже если Time.timeScale = 0 в меню
            float k = Mathf.Clamp01(t / duration);
            float s = ease.Evaluate(k);
            target.localScale = targetVector * s;
            yield return null;
        }

        target.localScale =targetVector;
    }
}
