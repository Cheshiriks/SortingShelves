using UnityEngine;
using YG;

public class SaveGame : MonoBehaviour
{

    public bool soundOn = true;

    //[SerializeField] private int gameLevel = 1;
    //[SerializeField] private int maxLevel = 1;
    //[SerializeField] private int coins = 0;
    //[SerializeField] private int stars = 0;
    //[SerializeField] private int score = 0;
    
    //[SerializeField] private int selectedThemeId = -1;
    //[SerializeField] private int buyButtonId = 0;
    
    // Для анимации 
    private int _plusMenuCoins = 0;
    private int _plusMenuStars = 0;
    
    public static SaveGame Instance;
    
    public int Coins => YG2.saves.coins;
    public int Stars => YG2.saves.stars;
    public int MaxLevel => YG2.saves.maxLevel;
    public int GameLevel => YG2.saves.gameLevel;
    public int Score => YG2.saves.score;
    public int BonusDestroy => YG2.saves.bonusDestroy;
    public int BonusChange => YG2.saves.bonusChange;
    
    public int SelectedThemeId
    {
        get { return YG2.saves.selectedThemeId; }
        set
        {
            YG2.saves.selectedThemeId = value;
            YG2.SaveProgress();
        }
    }
    
    public int BuyButtonId
    {
        get { return YG2.saves.buyButtonId; }
        set
        {
            YG2.saves.buyButtonId = value;
            YG2.SaveProgress();
        }
    }
    
    public int PrivateMenuCoins
    {
        get { return _plusMenuCoins; }
        set { _plusMenuCoins = value; }
    }
    public int PrivateMenuStars
    {
        get { return _plusMenuStars; }
        set { _plusMenuStars = value; }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
            Instance = this;
            // LoadDate();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /*public void SaveData()
    {
        PlayerPrefs.SetInt("gameLevel", gameLevel);
        PlayerPrefs.SetInt("maxLevel", maxLevel);
        
        PlayerPrefs.SetInt("coins", coins);
        PlayerPrefs.SetInt("stars", stars);
        
        PlayerPrefs.SetInt("buyButtonId", buyButtonId);
        PlayerPrefs.SetInt("selectedThemeId", selectedThemeId);
    }

    public void LoadDate()
    {
        if (PlayerPrefs.HasKey("gameLevel"))
        {
            gameLevel = PlayerPrefs.GetInt("gameLevel");
        }
        if (PlayerPrefs.HasKey("maxLevel"))
        {
            maxLevel = PlayerPrefs.GetInt("maxLevel");
        }
        
        if (PlayerPrefs.HasKey("coins"))
        {
            coins = PlayerPrefs.GetInt("coins");
        }
        if (PlayerPrefs.HasKey("stars"))
        {
            stars = PlayerPrefs.GetInt("stars");
        }
        
        if (PlayerPrefs.HasKey("buyButtonId"))
        {
            buyButtonId = PlayerPrefs.GetInt("buyButtonId");
        }
        if (PlayerPrefs.HasKey("selectedThemeId"))
        {
            selectedThemeId = PlayerPrefs.GetInt("selectedThemeId");
        }
    }*/

    public int AddScore(int scoreToAdd = 20)
    {
        YG2.saves.score += scoreToAdd;
        YG2.SaveProgress();
        return YG2.saves.score;
    }
    
    public int PlusCoin(int addCoins)
    {
        YG2.saves.coins += addCoins;
        YG2.SaveProgress();
        return YG2.saves.coins;
    }
    
    public int MinusCoin(int minusCoins)
    {
        if (YG2.saves.coins >= minusCoins)
        {
            YG2.saves.coins -= minusCoins;
        }
        YG2.SaveProgress();
        return YG2.saves.coins;
    }
    
    public int PlusStars(int addCoins)
    {
        YG2.saves.stars += addCoins;
        YG2.SaveProgress();
        return YG2.saves.stars;
    }
    
    public int MinusStars(int minusCoins)
    {
        if (YG2.saves.stars >= minusCoins)
        {
            YG2.saves.stars -= minusCoins;
        }
        YG2.SaveProgress();
        return YG2.saves.stars;
    }

    public void WinLevel()
    {
        PlusStars(3);
        _plusMenuStars = 3;
        
        PlusCoin(50);
        _plusMenuCoins = 50;
        
        YG2.saves.maxLevel++;
        if (YG2.saves.gameLevel >= 50)
        {
            YG2.saves.gameLevel = 30;
        }
        else
        {
            YG2.saves.gameLevel++;
        }
        YG2.SaveProgress();
    }
    
    // секция бустеров
    public int AddBonusDestroy(int addBonus)
    {
        YG2.saves.bonusDestroy += addBonus;
        YG2.SaveProgress();
        return YG2.saves.bonusDestroy;
    }
    
    public int AddBonusChange(int addBonus)
    {
        YG2.saves.bonusChange += addBonus;
        YG2.SaveProgress();
        return YG2.saves.bonusChange;
    }

}