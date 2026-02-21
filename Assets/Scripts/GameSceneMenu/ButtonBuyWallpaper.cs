using TMPro;
using UnityEngine;

public class ButtonBuyWallpeper : MonoBehaviour
{
    [SerializeField] private SelectTheme theme;
    [SerializeField] private int cost;
    [SerializeField] private TextMeshProUGUI buttonCostText;
    [SerializeField] private TextMeshProUGUI userCoinsText;
    [SerializeField] private ButtonsWallpaperController buttonsWallpaperController;

    private void Start()
    {
        buttonCostText.text = cost.ToString();
        if (SaveGame.Instance.BuyButtonId < theme.Id)
        {
            gameObject.SetActive(false);
        }
        else if (SaveGame.Instance.BuyButtonId == theme.Id)
        {
            theme.ChangeColor(Color.red);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    
    public void BuyWallpaper()
    {
        if (cost <= SaveGame.Instance.Coins)
        {
            int newCoins = SaveGame.Instance.MinusCoin(cost);
            userCoinsText.text = newCoins.ToString();
            SaveGame.Instance.BuyButtonId = theme.Id + 1;
            theme.ChangeColor(Color.black);

            buttonsWallpaperController.ActiveButton(SaveGame.Instance.BuyButtonId);
            
            gameObject.SetActive(false);
        }
    }
}
