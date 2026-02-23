using UnityEngine;

public class SoundOnOf : MonoBehaviour
{
    [SerializeField] private GameObject soundOn;
    [SerializeField] private GameObject soundOff;

    public void SoundOn()
    {
        if (!SaveGame.Instance.soundOn)
        {
            SaveGame.Instance.soundOn = true;
        
            soundOn.SetActive(true);
            soundOff.SetActive(false);
        }
    }
    
    public void SoundOff()
    {
        if (SaveGame.Instance.soundOn)
        {
            SaveGame.Instance.soundOn = false;
        
            soundOn.SetActive(false);
            soundOff.SetActive(true);
        }
    }
}
