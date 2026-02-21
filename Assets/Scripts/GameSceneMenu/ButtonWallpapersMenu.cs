using UnityEngine;

public class ButtonWallpapersMenu : MonoBehaviour
{
    [SerializeField] private GameObject wallpapersMenu;
    [SerializeField] private GameObject buttonSetting;
    [SerializeField] private GameObject buttonCustom;
    
    [Header("Missions collection")]
    [SerializeField] private Transform missionsScale;

    public void OpenWallpapersMenu()
    {
        buttonSetting.SetActive(false);
        buttonCustom.SetActive(true);
        
        missionsScale.localScale = Vector3.zero;
        
        wallpapersMenu.SetActive(true);
    }
    
    public void CloseWallpapersMenu()
    {
        buttonSetting.SetActive(true);
        buttonCustom.SetActive(false);
        
        missionsScale.localScale = Vector3.one;
        
        wallpapersMenu.SetActive(false);
    }

}
