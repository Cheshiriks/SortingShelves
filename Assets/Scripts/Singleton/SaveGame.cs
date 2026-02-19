using UnityEngine;

public class SaveGame : MonoBehaviour
{

    public static bool SoundOn = true;

    [SerializeField] private int gameLevel = 1;
    [SerializeField] private int maxLevel = 1;
    [SerializeField] private int coins = 0;
    [SerializeField] private int stars = 0;
    
    public static SaveGame Instance;
    
    public int Coins => coins;

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
    }

    public int PlusCoin(int addCoins)
    {
        coins += addCoins;
        return coins;
    }
    
    public int MinusCoin(int minusCoins)
    {
        if (coins >= minusCoins)
        {
            coins -= minusCoins;
        }
        return coins;
    }

}