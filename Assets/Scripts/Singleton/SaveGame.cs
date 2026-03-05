using UnityEngine;

public class SaveGame : MonoBehaviour
{

    public bool soundOn = true;

    [SerializeField] private int gameLevel = 1;
    [SerializeField] private int maxLevel = 1;
    [SerializeField] private int coins = 0;
    [SerializeField] private int stars = 0;
    
    [SerializeField] private int selectedThemeId = -1;
    [SerializeField] private int buyButtonId = 0;
    
    // Для анимации 
    private int _plusMenuCoins = 0;
    private int _plusMenuStars = 0;
    
    public static SaveGame Instance;
    
    public int Coins => coins;
    public int Stars => stars;
    public int MaxLevel => maxLevel;
    public int SelectedThemeId
    {
        get { return selectedThemeId; }
        set
        {
            selectedThemeId = value;
            SaveData();
        }
    }
    
    public int BuyButtonId
    {
        get { return buyButtonId; }
        set
        {
            buyButtonId = value;
            SaveData();
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
    
    public void SaveData()
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
    }

    public int PlusCoin(int addCoins)
    {
        coins += addCoins;
        SaveData();
        return coins;
    }
    
    public int MinusCoin(int minusCoins)
    {
        if (coins >= minusCoins)
        {
            coins -= minusCoins;
        }
        SaveData();
        return coins;
    }
    
    public int PlusStars(int addCoins)
    {
        stars += addCoins;
        SaveData();
        return stars;
    }
    
    public int MinusStars(int minusCoins)
    {
        if (stars >= minusCoins)
        {
            stars -= minusCoins;
        }
        SaveData();
        return stars;
    }

    public void WinLevel()
    {
        PlusStars(3);
        _plusMenuStars = 3;
        
        PlusCoin(50);
        _plusMenuCoins = 50;
        
        maxLevel++;
        if (gameLevel >= 50)
        {
            gameLevel = 30;
        }
        else
        {
            gameLevel++;
        }
        SaveData();
    }

}