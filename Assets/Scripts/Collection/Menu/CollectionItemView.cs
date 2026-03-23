using UnityEngine;
using UnityEngine.UI;

public class CollectionItemView : MonoBehaviour
{
    [SerializeField] private Image shadowImage;
    [SerializeField] private Image fillImage;

    public void Setup(CollectionMenuItemViewData data)
    {
        if (shadowImage != null)
        {
            shadowImage.sprite = data.sprite;
            shadowImage.type = Image.Type.Simple;
            shadowImage.color = new Color(1f, 1f, 1f, 0.25f);
        }

        if (fillImage != null)
        {
            fillImage.sprite = data.sprite;
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Radial360;
            fillImage.fillOrigin = (int)Image.Origin360.Bottom;
            fillImage.fillAmount = Mathf.Clamp01(data.count / 4f);
            fillImage.color = Color.white;
        }
    }
}
