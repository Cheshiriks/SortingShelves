using UnityEngine;

[RequireComponent(typeof(ResponsiveLayoutWorld))]
public class CameraFitToLevel : MonoBehaviour
{
    public Camera cam;

    [Header("What to fit (usually ShelfCollection)")]
    public Transform contentRoot;

    [Header("Center offset (world units)")]
    public float centerYOffset = 0f;
    
    [Header("Margins in world units")]
    public float marginX = 0.6f;
    public float marginY = 0.8f;

    [Header("Limits")]
    public float minOrthoSize = 1.5f;  // чтобы не становилось слишком крупно
    public float maxOrthoSize = 20f;   // чтобы не улетало

    [Header("Smooth")]
    public bool smooth = true;
    public float smoothSpeed = 8f;
    
    [Header("Bounds filter")]
    public LayerMask boundsLayers;

    private int lastW, lastH;

    private void OnEnable()
    {
        if (!cam) cam = Camera.main;
        if (!contentRoot) contentRoot = transform.Find("ShelfCollection");

        lastW = Screen.width;
        lastH = Screen.height;

        FitNow(force:true);
    }

    private void LateUpdate()
    {
        // LateUpdate — чтобы сначала ResponsiveLayoutWorld переставил полки, а потом мы фитили камеру
        if (Screen.width != lastW || Screen.height != lastH)
        {
            lastW = Screen.width;
            lastH = Screen.height;
            FitNow(force:false);
        }
        else
        {
            // если хочешь, можно фитить и без смены экрана (например, если полки анимировано двигаются)
            FitNow(force:false);
        }
    }

    private void FitNow(bool force)
    {
        if (!cam || !cam.orthographic || !contentRoot) return;

        Bounds b = CalculateBounds(contentRoot);
        if (b.size == Vector3.zero) return;

        float aspect = (float)Screen.width / Screen.height;

        // half-sizes контента + отступы
        float halfContentW = b.extents.x + marginX;
        float halfContentH = b.extents.y + marginY;

        // Для ортокамеры:
        // видимая halfHeight = orthographicSize
        // видимая halfWidth  = orthographicSize * aspect
        //
        // Нужно: halfWidth >= halfContentW  и  halfHeight >= halfContentH
        // => orthographicSize >= halfContentH
        // => orthographicSize >= halfContentW / aspect
        float neededSize = Mathf.Max(halfContentH, halfContentW / Mathf.Max(0.01f, aspect));
        neededSize = Mathf.Clamp(neededSize, minOrthoSize, maxOrthoSize);

        if (smooth && !force)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, neededSize, Time.deltaTime * smoothSpeed);
        }
        else
        {
            cam.orthographicSize = neededSize;
        }

        // (опционально) центрируем камеру на контент
        Vector3 p = cam.transform.position;
        cam.transform.position = new Vector3(b.center.x, b.center.y + centerYOffset, p.z);
    }

    private Bounds CalculateBounds(Transform root)
    {
        var renderers = root.GetComponentsInChildren<SpriteRenderer>(includeInactive:false);

        bool found = false;
        Bounds b = new Bounds(root.position, Vector3.zero);

        foreach (var r in renderers)
        {
            // проверяем, входит ли слой объекта в mask
            if ((boundsLayers.value & (1 << r.gameObject.layer)) == 0)
                continue;

            if (!found)
            {
                b = r.bounds;
                found = true;
            }
            else
            {
                b.Encapsulate(r.bounds);
            }
        }

        return found ? b : new Bounds(root.position, Vector3.zero);
    }
}