using System;
using UnityEngine;

public class BackgroundThemeSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject bgMap;
    [SerializeField] private Transform grandParent;
    public Sprite[] themeSprites;
    public Sprite lockSprites;

    private SpriteRenderer[] _srs;

    private void Start()
    {
        _srs = grandParent.GetComponentsInChildren<SpriteRenderer>(true);
        ApplyTheme(SaveGame.Instance.SelectedThemeId);
    }

    public void ApplyTheme(int themeIndex)
    {
        try
        {
            Sprite newSprite = themeSprites[themeIndex];
            bgMap.SetActive(true);

            //SpriteRenderer[] srs = grandParent.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var sr in _srs)
                sr.sprite = newSprite;
        }
        catch (Exception e)
        {
            bgMap.SetActive(false);
            Debug.Log("Обоев с индексом " + themeIndex + " не найдено");
        }
    }
    
}
