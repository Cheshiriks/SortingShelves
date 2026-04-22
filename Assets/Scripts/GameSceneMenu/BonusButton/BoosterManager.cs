using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class BoosterManager : MonoBehaviour
{
    [SerializeField] private HintPulseManager destroyManager;
    [SerializeField] private ChangeBoosterManager changeManager;
    
    //[SerializeField] private GameObject buttonDestroy;
    //[SerializeField] private GameObject buttonChange;
    
    [SerializeField] private TextMeshProUGUI textDestroy;
    [SerializeField] private TextMeshProUGUI textChange;
    
    [SerializeField] private Image colorDestroy;
    [SerializeField] private Image colorChange;
    
    [SerializeField] private GameObject menuAddDestroy;
    [SerializeField] private GameObject menuAddChange;
    
    [SerializeField] private int coinsDestroy = 1200;
    [SerializeField] private int coinsChange = 1600;
    
    [SerializeField] private UIBlocker uiBlocker;
    
    [SerializeField] private TextMeshProUGUI coinsText;
    
    void Start()
    {
        StartDestroy();
        StartChange();
    }

    public void UseDestroy()
    {
        if (SaveGame.Instance.BonusDestroy > 0)
        {
            destroyManager.TogglePulse();
        }
        else
        {
            // вызываем меню покупки
            menuAddDestroy.SetActive(true);
            uiBlocker.SetBlocked(true);
            
            //SaveGame.Instance.AddBonusDestroy(1);
            //StartDestroy();
        }
    }
    
    public void UseChange()
    {
        if (SaveGame.Instance.BonusChange > 0)
        {
            changeManager.UseBlueBooster();
            SaveGame.Instance.AddBonusChange(-1);
            StartChange();
        }
        else
        {
            // вызываем меню покупки
            menuAddChange.SetActive(true);
            uiBlocker.SetBlocked(true);
                
            //SaveGame.Instance.AddBonusChange(1);
            //StartChange();
        }
    }

    private void StartDestroy()
    {
        int bonusDestroyCount = SaveGame.Instance.BonusDestroy;
        
        if (bonusDestroyCount == 0)
        {
            textDestroy.text = "+";
            colorDestroy.color = Color.red;
        } else
        {
            textDestroy.text = bonusDestroyCount.ToString();
            colorDestroy.color = new Color32(15, 171, 0, 255);
        }
    }
    
    private void StartChange()
    {
        int bonusChangeCount = SaveGame.Instance.BonusChange;
        
        if (bonusChangeCount == 0)
        {
            textChange.text = "+";
            colorChange.color = Color.red;
        } else
        {
            textChange.text = bonusChangeCount.ToString();
            colorChange.color = new Color32(15, 171, 0, 255);
        }
    }

    public void CloseAddDestroyMenu()
    {
        menuAddDestroy.SetActive(false);
        uiBlocker.SetBlocked(false);
    }
    
    public void CloseAddChangeMenu()
    {
        menuAddChange.SetActive(false);
        uiBlocker.SetBlocked(false);
    }

    public void AddDestroyBonusForCoins()
    {
        if (SaveGame.Instance.Coins >= coinsDestroy)
        {
            SaveGame.Instance.MinusCoin(coinsDestroy);
            SaveGame.Instance.AddBonusDestroy(1);
            
            StartDestroy();
            coinsText.text = SaveGame.Instance.Coins.ToString();
            CloseAddDestroyMenu();
        }
    }
    
    public void AddChangeBonusForCoins()
    {
        if (SaveGame.Instance.Coins >= coinsChange)
        {
            SaveGame.Instance.MinusCoin(coinsChange);
            SaveGame.Instance.AddBonusChange(1);
            
            StartChange();
            coinsText.text = SaveGame.Instance.Coins.ToString();
            CloseAddChangeMenu();
        }
    }

    public void AddDestroyBonusForAd()
    {
        string id = "bonus_destroy";
        YG2.RewardedAdvShow(id, GetDestroyRewards);
    }

    private void GetDestroyRewards()
    {
        SaveGame.Instance.AddBonusDestroy(2);
        
        StartDestroy();
        CloseAddDestroyMenu();
    }
    
    public void AddChangeBonusForAd()
    {
        string id = "bonus_change";
        YG2.RewardedAdvShow(id, GetChangeRewards);
    }

    private void GetChangeRewards()
    {
        SaveGame.Instance.AddBonusChange(2);
        
        StartChange();
        CloseAddChangeMenu();
    }

}
