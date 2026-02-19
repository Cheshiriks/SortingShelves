using System.Collections;
using TMPro;
using UnityEngine;

public class CoinsMenuTextCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private int startCoins = 50; 

    [Header("Animation")]
    [SerializeField] private float maxAnimTime = 0.6f;   // максимум времени анимации
    [SerializeField] private float minStepDelay = 0.005f;
    [SerializeField] private int stepCoins = 1;

    private Coroutine _routine;
    private int _shownValue;

    private void Awake()
    {
        if (!coinsText) coinsText = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        SetInstant(startCoins);
    }

    public void SetInstant(int value)
    {
        _shownValue = value;
        if (coinsText) coinsText.text = _shownValue.ToString();
    }

    public void AnimateTo(int targetValue)
    {
        if (!coinsText) return;

        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(AnimateRoutine(targetValue));
    }

    private IEnumerator AnimateRoutine(int target)
    {
        int start = _shownValue;
        int delta = target - start;

        if (delta == 0)
        {
            SetInstant(target);
            yield break;
        }

        int step = delta > 0 ? stepCoins : -stepCoins;
        int stepsCount = Mathf.Abs(delta);

        float stepDelay = Mathf.Clamp(maxAnimTime / stepsCount, minStepDelay, maxAnimTime);

        while (_shownValue != target)
        {
            _shownValue += step;
            coinsText.text = _shownValue.ToString();
            yield return new WaitForSeconds(stepDelay);
        }

        _routine = null;
    }
}
