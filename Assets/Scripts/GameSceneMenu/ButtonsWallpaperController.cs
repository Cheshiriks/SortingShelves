using System;
using UnityEngine;

public class ButtonsWallpaperController : MonoBehaviour
{
    [SerializeField] private GameObject[] _buttons;

    public void ActiveButton(int id)
    {
        try
        {
            GameObject button = _buttons[id];
            button.SetActive(true);
        }
        catch (Exception e)
        {
            Debug.Log("Кнопки с индексом " + id + " не найдено");
        }
    }
}
