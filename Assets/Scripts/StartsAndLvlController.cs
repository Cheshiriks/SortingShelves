using TMPro;
using UnityEngine;

public class StartsAndLvlController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI starsText;
    [SerializeField] private TextMeshProUGUI lvlText;
    [SerializeField] private TextMeshProUGUI coinsText;
    
    void Start()
    {
        if (starsText) starsText.text = SaveGame.Instance.Stars.ToString();
        if (lvlText) lvlText.text = "УР." + SaveGame.Instance.MaxLevel;
        if (coinsText) coinsText.text = SaveGame.Instance.Coins.ToString();
    }
    
}
