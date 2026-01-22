using UnityEngine;

public class ResponsiveLayoutWorld : MonoBehaviour
{
    [Header("Switch when aspect goes below this (1 = square)")]
    public float aspectThreshold = 1f;

    [Header("Camera")]
    public Camera cam;
    public float refWidth = 1920f;
    public float refHeight = 1080f;
    public float refOrthoSizeLandscape = 5f; // поставь текущее Orthographic Size в landscape

    [Header("Shelves to move (real shelves)")]
    public Transform[] shelves;

    [Header("Anchors")]
    public Transform[] landscapeAnchors;
    public Transform[] portraitAnchors;

    [Header("Smooth")]
    public bool smooth = true;
    public float moveSpeed = 12f;

    private bool isPortrait;
    private int lastW, lastH;

    private void Awake()
    {
        if (!cam) cam = Camera.main;
        lastW = Screen.width;
        lastH = Screen.height;

        ApplyAll(force: true);
    }

    private void Update()
    {
        if (Screen.width != lastW || Screen.height != lastH)
        {
            lastW = Screen.width;
            lastH = Screen.height;
            ApplyAll(force: false);
        }

        if (smooth)
            SmoothMoveShelves();
    }

    private void ApplyAll(bool force)
    {
        float aspect = (float)Screen.width / Screen.height;
        bool portraitNow = aspect < aspectThreshold;

        if (!force && portraitNow == isPortrait) return;
        isPortrait = portraitNow;

        ApplyCamera(aspect);
        ApplyShelvesImmediate(); // если smooth==true, это задаст “цели”
    }

    private void ApplyCamera(float aspect)
    {
        if (!cam || !cam.orthographic) return;

        float refAspect = refWidth / refHeight;

        if (!isPortrait)
        {
            // LANDSCAPE / SQUARE: fit by height
            cam.orthographicSize = refOrthoSizeLandscape;
        }
        else
        {
            // PORTRAIT: fit by width (сохраняем видимую ширину как в reference)
            cam.orthographicSize = (refOrthoSizeLandscape * refAspect) / aspect;
        }
    }

    private void ApplyShelvesImmediate()
    {
        var anchors = isPortrait ? portraitAnchors : landscapeAnchors;

        for (int i = 0; i < shelves.Length; i++)
        {
            if (!shelves[i] || i >= anchors.Length || !anchors[i]) continue;

            if (!smooth)
                shelves[i].position = anchors[i].position;
            // если smooth=true, то фактическое движение сделает SmoothMoveShelves()
        }
    }

    private void SmoothMoveShelves()
    {
        var anchors = isPortrait ? portraitAnchors : landscapeAnchors;

        for (int i = 0; i < shelves.Length; i++)
        {
            if (!shelves[i] || i >= anchors.Length || !anchors[i]) continue;

            shelves[i].position = Vector3.Lerp(
                shelves[i].position,
                anchors[i].position,
                Time.deltaTime * moveSpeed
            );
        }
    }
}
