using UnityEngine;

public class PresentMenuTimer : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject presentMenu; // сам PresentMenu (панель)
    [SerializeField] private UIBlocker uiBlocker;
    [SerializeField] private CoinsTextCounter coinsCounter;
    
    [SerializeField] private CoinFlyAnimator coinFly;
    [SerializeField] private RectTransform spawnFrom;

    [Header("Cooldown")]
    [SerializeField] private float cooldownSeconds = 60f;
    
    [Header("Cooldown")]
    [SerializeField] private int presentCoins = 50;

    private float _nextAllowedTime; // Time.time, когда можно показывать снова

    private void Awake()
    {
        presentMenu.SetActive(false);
        _nextAllowedTime = Time.time + cooldownSeconds;
    }

    public void TryShow()
    {
        if (Time.time < _nextAllowedTime) return;

        presentMenu.SetActive(true);
        uiBlocker.SetBlocked(true);
    }

    public void Hide()
    {
        presentMenu.SetActive(false);
        uiBlocker.SetBlocked(false);
    }

    // повесь этот метод на кнопку "ПОЛУЧИТЬ"
    public void GetCoins()
    {
        _nextAllowedTime = Time.time + cooldownSeconds;
        Debug.Log("nextAllowedTime " + _nextAllowedTime + " now " + Time.time);
        
        // выдать награду
        int coinsBefore = SaveGame.Instance.Coins;
        int coinsAfter = SaveGame.Instance.PlusCoin(presentCoins);
        Hide();
        
        // запускаем полёт монет, и когда долетели — докручиваем текст
        if (coinFly && spawnFrom)
        {
            coinFly.PlayFrom(spawnFrom, () =>
            {
                // Важно: показ всегда "до истины"
                coinsCounter.AnimateTo(coinsBefore, coinsAfter);
            });
        }
        else
        {
            // если без VFX — просто докрутим
            coinsCounter.AnimateTo(coinsBefore, coinsAfter);
        }
    }

    public float SecondsLeft()
    {
        return Mathf.Max(0f, _nextAllowedTime - Time.time);
    }
}
