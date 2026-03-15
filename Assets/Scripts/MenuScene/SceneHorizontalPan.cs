using UnityEngine;
using UnityEngine.EventSystems;

public class SceneHorizontalPan : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [SerializeField] private RectTransform sceneImage; // SceneImg
    [SerializeField] private Canvas canvas;
    [SerializeField] private float targetAspect = 16f / 9f;

    private RectTransform viewport;

    private float minX;
    private float maxX;
    private bool canDrag;

    private void Awake()
    {
        viewport = GetComponent<RectTransform>();

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        Canvas.ForceUpdateCanvases();
        UpdateLayout();
    }

    private void Start()
    {
        Canvas.ForceUpdateCanvases();
        UpdateLayout();
    }

    private void OnRectTransformDimensionsChange()
    {
        if (!isActiveAndEnabled) return;
        UpdateLayout();
    }

    private void UpdateLayout()
    {
        if (viewport == null || sceneImage == null || canvas == null) return;

        float viewportWidth = viewport.rect.width;
        float viewportHeight = viewport.rect.height;

        float imageHeight = viewportHeight;
        float imageWidth = imageHeight * targetAspect;

        sceneImage.anchorMin = new Vector2(0.5f, 0.5f);
        sceneImage.anchorMax = new Vector2(0.5f, 0.5f);
        sceneImage.pivot = new Vector2(0.5f, 0.5f);

        sceneImage.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, imageWidth);
        sceneImage.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, imageHeight);

        float extraWidth = Mathf.Max(0f, imageWidth - viewportWidth);

        canDrag = extraWidth > 0.01f;

        minX = -extraWidth * 0.5f;
        maxX =  extraWidth * 0.5f;

        Vector2 pos = sceneImage.anchoredPosition;
        pos.x = canDrag ? Mathf.Clamp(pos.x, minX, maxX) : 0f;
        pos.y = 0f;
        sceneImage.anchoredPosition = pos;

        Debug.Log($"viewport: {viewportWidth}x{viewportHeight}, imageWidth: {imageWidth}, canDrag: {canDrag}");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag) return;

        Vector2 pos = sceneImage.anchoredPosition;
        pos.x += eventData.delta.x / canvas.scaleFactor;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = 0f;

        sceneImage.anchoredPosition = pos;
    }
}
