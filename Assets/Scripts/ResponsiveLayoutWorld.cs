using System.Linq;
using UnityEngine;

public class ResponsiveLayoutWorld : MonoBehaviour
{
    [Header("Switch when aspect goes below this (1 = square)")]
    public float aspectThreshold = 1f;

    [Header("Camera")]
    public Camera cam;

    [Header("Shelves to move (real shelves)")]
    private Transform[] shelves;

    [Header("Anchors")]
    private Transform[] landscapeAnchors;
    private Transform[] portraitAnchors;

    [Header("Auto-find inside THIS level root")]
    public Transform shelfCollection;          // lvl_X/ShelfCollection
    public Transform landscapeAnchorsRoot;     // lvl_X/LayoutAnchors/LandscapeAnchors
    public Transform portraitAnchorsRoot;      // lvl_X/LayoutAnchors/PortraitAnchors
    
    [Header("Smooth")]
    public bool smoothMove = true;
    public float moveSpeed = 12f;

    private bool isPortrait;
    private int lastW, lastH;

    private void Reset()
    {
        AutoFind();
    }

    private void OnEnable()
    {
        if (!cam) cam = Camera.main;
        AutoFind();

        Collect();

        lastW = Screen.width;
        lastH = Screen.height;

        ApplyAll(force: true);
    }

    private void Update()
    {
        // если размеры не менялись — не дёргаем логику
        if (Screen.width != lastW || Screen.height != lastH)
        {
            lastW = Screen.width;
            lastH = Screen.height;
            ApplyAll(force: false);
        }

        if (smoothMove)
            SmoothMoveShelves();
    }

    private void AutoFind()
    {
        if (!shelfCollection)
            shelfCollection = transform.Find("ShelfCollection");

        if (!landscapeAnchorsRoot || !portraitAnchorsRoot)
        {
            var layoutAnchors = transform.Find("LayoutAnchors");
            if (layoutAnchors)
            {
                if (!landscapeAnchorsRoot) landscapeAnchorsRoot = layoutAnchors.Find("LandscapeAnchors");
                if (!portraitAnchorsRoot)  portraitAnchorsRoot  = layoutAnchors.Find("PortraitAnchors");
            }
        }
    }

    private void Collect()
    {
        // Полки: берём прямых детей ShelfCollection
        shelves = shelfCollection
            ? shelfCollection.Cast<Transform>().ToArray()
            : new Transform[0];

        // Якоря: берём детей LandscapeAnchors/PortraitAnchors и сортируем по имени Shelf_1..Shelf_3
        landscapeAnchors = landscapeAnchorsRoot
            ? landscapeAnchorsRoot.Cast<Transform>().OrderBy(t => t.name).ToArray()
            : new Transform[0];

        portraitAnchors = portraitAnchorsRoot
            ? portraitAnchorsRoot.Cast<Transform>().OrderBy(t => t.name).ToArray()
            : new Transform[0];

        // Небольшая защита, чтобы сразу было понятно, если что-то не найдено
        if (shelves.Length == 0) Debug.LogWarning($"[{name}] No shelves found. Check ShelfCollection.", this);
        if (landscapeAnchors.Length == 0) Debug.LogWarning($"[{name}] No landscape anchors found.", this);
        if (portraitAnchors.Length == 0) Debug.LogWarning($"[{name}] No portrait anchors found.", this);
    }

    private void ApplyAll(bool force)
    {
        float aspect = (float)Screen.width / Screen.height;
        bool portraitNow = aspect < aspectThreshold;

        if (!force && portraitNow == isPortrait) return;
        isPortrait = portraitNow;

        // если не smooth — ставим сразу
        if (!smoothMove)
            ApplyShelvesImmediate();
    }

    private void ApplyShelvesImmediate()
    {
        var anchors = isPortrait ? portraitAnchors : landscapeAnchors;

        int n = Mathf.Min(shelves.Length, anchors.Length);
        for (int i = 0; i < n; i++)
        {
            if (shelves[i] && anchors[i])
                shelves[i].position = anchors[i].position;
        }
    }

    private void SmoothMoveShelves()
    {
        var anchors = isPortrait ? portraitAnchors : landscapeAnchors;

        int n = Mathf.Min(shelves.Length, anchors.Length);
        for (int i = 0; i < n; i++)
        {
            if (!shelves[i] || !anchors[i]) continue;

            shelves[i].position = Vector3.Lerp(
                shelves[i].position,
                anchors[i].position,
                Time.deltaTime * moveSpeed
            );
        }
    }
}
