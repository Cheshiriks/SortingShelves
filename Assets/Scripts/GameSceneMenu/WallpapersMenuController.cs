using UnityEngine;

public class WallpapersMenuController : MonoBehaviour
{
    [SerializeField] private float aspectThreshold = 1f;
    
    [SerializeField] private GameObject horizonatal;
    [SerializeField] private GameObject scroll;
    
    private int _lastW, _lastH;
    
    void Update()
    {
        if (Screen.width != _lastW || Screen.height != _lastH)
        {
            _lastW = Screen.width;
            _lastH = Screen.height;
            
            float aspect = (float) Screen.width / Screen.height;
            bool portraitNow = aspect < aspectThreshold;

            if (portraitNow)
            {
                horizonatal.SetActive(false);
                scroll.SetActive(true);
            }
            else
            {
                scroll.SetActive(false);
                horizonatal.SetActive(true);
            }

        }
        
    }
}
