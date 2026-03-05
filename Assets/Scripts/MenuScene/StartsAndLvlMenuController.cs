using TMPro;
using UnityEngine;

public class StartsAndLvlMenuController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI starsText;
    [SerializeField] private TextMeshProUGUI lvlText;
    [SerializeField] private TextMeshProUGUI coinsText;
    
    [SerializeField] private CoinsTextCounter coinsCounter;
    [SerializeField] private CoinFlyAnimator coinFly;
    [SerializeField] private RectTransform coinSpawnFrom;
    
    [SerializeField] private CoinsTextCounter starsCounter;
    [SerializeField] private CoinFlyAnimator starsFly;
    [SerializeField] private RectTransform starsSpawnFrom;
    
    void Start()
    {
        if (lvlText) lvlText.text = "УР." + SaveGame.Instance.MaxLevel;

        int coinsBefore = SaveGame.Instance.Coins - SaveGame.Instance.PrivateMenuCoins;
        int coinsAfter = SaveGame.Instance.Coins;
        
        int starsBefore = SaveGame.Instance.Stars - SaveGame.Instance.PrivateMenuStars;
        int starsAfter = SaveGame.Instance.Stars;
        
        if (SaveGame.Instance.PrivateMenuCoins == 0)
        {
            if (coinsText) coinsText.text = coinsAfter.ToString();
        }
        else
        {
            if (coinsText) coinsText.text = coinsBefore.ToString();
            SaveGame.Instance.PrivateMenuCoins = 0;
            
            // запускаем полёт монет, и когда долетели — докручиваем текст
            if (coinFly && coinSpawnFrom)
            {
                coinFly.PlayFrom(coinSpawnFrom, () =>
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
        
        if (SaveGame.Instance.PrivateMenuStars == 0)
        {
            if (starsText) starsText.text = starsAfter.ToString();
        }
        else
        {
            if (starsText) starsText.text = starsBefore.ToString();
            SaveGame.Instance.PrivateMenuStars = 0;
            
            // запускаем полёт монет, и когда долетели — докручиваем текст
            if (starsFly && starsSpawnFrom)
            {
                starsFly.PlayFrom(starsSpawnFrom, () =>
                {
                    // Важно: показ всегда "до истины"
                    starsCounter.AnimateTo(starsBefore, starsAfter);
                });
            }
            else
            {
                // если без VFX — просто докрутим
                starsCounter.AnimateTo(starsBefore, starsAfter);
            }
        }
    }
}
