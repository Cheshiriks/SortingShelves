using UnityEngine;
using UnityEngine.UI;

public class SelectTheme : MonoBehaviour
{
    
    [SerializeField] private int id;
    [SerializeField] private BackgroundThemeSwitcher themeSwitcher;
    [SerializeField] private Image blackImg;
    
    private Image _wallpaperImg;
    private bool _isSelected = false;
    private bool _isCheck = true;

    public int Id => id;
    
    public void ChangeTheme()
    {
        if (id < SaveGame.Instance.BuyButtonId)
        {
            themeSwitcher.ApplyTheme(id);
            SaveGame.Instance.SelectedThemeId = id;
        }
    }
    
    private void Start()
    {
        _wallpaperImg = GetComponent<Image>();
        if (SaveGame.Instance.BuyButtonId < id)
        {
            _wallpaperImg.sprite = themeSwitcher.lockSprites;
        }
    }

    private void Update()
    {
        if (SaveGame.Instance.SelectedThemeId == id && !_isSelected)
        {
            blackImg.color = Color.green;
            _isSelected = true;
            _isCheck = true;
        }

        if (SaveGame.Instance.SelectedThemeId != id && _isSelected)
        {
            blackImg.color = Color.black;
            _isSelected = false;
            _isCheck = true;
        }

        if (SaveGame.Instance.BuyButtonId == id && _isCheck)
        {
            blackImg.color = Color.red;
            ChangeImg();
            _isCheck = false;
        }
    }
    
    public void ChangeImg()
    {
        Sprite newSprite = themeSwitcher.themeSprites[id];
        _wallpaperImg.sprite = newSprite;
    }
    
    public void ChangeColor(Color32 color)
    {
        blackImg.color = color;
    }
}
