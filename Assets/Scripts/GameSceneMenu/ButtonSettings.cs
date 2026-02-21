using UnityEngine;

public class ButtonSettings : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject areYouSureMenu;
    [SerializeField] private UIBlocker uiBlocker;
    
    public void OpenSettings()
    {
        areYouSureMenu.SetActive(false);
        settingsMenu.SetActive(true);
        uiBlocker.SetBlocked(true);
    }
    
    public void CloseSettings()
    {
        uiBlocker.SetBlocked(false);
        settingsMenu.SetActive(false);
    }
    
    public void OpenAreYouSureMenu()
    {
        settingsMenu.SetActive(false);
        areYouSureMenu.SetActive(true);
        uiBlocker.SetBlocked(true);
    }
}
