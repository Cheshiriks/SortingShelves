using UnityEngine;

public class AdManager : MonoBehaviour
{
    private bool _isShowAd = false;
    
    public static AdManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool GetIsShowAd()
    {
        return _isShowAd;
    }
    
    public void SetIsShowAd(bool flag)
    {
        _isShowAd = flag;
    }
}
