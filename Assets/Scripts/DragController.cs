using UnityEngine;

public class DragController : MonoBehaviour
{
    [SerializeField] private LayerMask itemMask;
    [SerializeField] private LayerMask slotMask;

    private Camera cam;
    private DraggableItem dragged;
    private Slot startSlot;
    private Vector3 grabOffset;
    private int originalSortingOrder;
    private SpriteRenderer draggedSR;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            TryPick();

        if (Input.GetMouseButton(0))
            Drag();

        if (Input.GetMouseButtonUp(0))
            Drop();
    }

    private void TryPick()
    {
        Vector2 pos = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0f, itemMask);
        if (!hit.collider) return;

        dragged = hit.collider.GetComponent<DraggableItem>();
        if (!dragged) return;
        if (dragged.CurrentSlot == null) return;

        startSlot = dragged.CurrentSlot;
        startSlot.ClearItem();

        grabbedVisualSetup();

        Vector3 mouseWorld = GetMouseWorld();
        grabOffset = dragged.transform.position - mouseWorld;
    }

    private void Drag()
    {
        if (!dragged) return;
        dragged.transform.position = GetMouseWorld() + grabOffset;
    }

    private void Drop()
    {
        if (!dragged) return;

        Slot target = RaycastSlotUnderMouse();

        if (target != null && target.IsEmpty)
        {
            dragged.MoveToSlot(target);
        }
        else
        {
            // если отпустили в пустоте или в занятый слот — вернуть обратно
            dragged.MoveToSlot(startSlot);
        }

        dragged = null;
        startSlot = null;

        GameManager.I.CheckAllShelves();
    }

    private Slot RaycastSlotUnderMouse()
    {
        Vector2 pos = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0f, slotMask);
        if (!hit.collider) return null;
        return hit.collider.GetComponent<Slot>();
    }

    private Vector3 GetMouseWorld()
    {
        Vector3 m = Input.mousePosition;
        m.z = Mathf.Abs(cam.transform.position.z);
        return cam.ScreenToWorldPoint(m);
    }

    private void grabbedVisualSetup()
    {
        draggedSR = dragged.GetComponent<SpriteRenderer>();
        if (draggedSR)
        {
            originalSortingOrder = draggedSR.sortingOrder;
            draggedSR.sortingOrder = 999;
        }
    }

    private void restoreVisualSetup()
    {
        if (draggedSR)
            draggedSR.sortingOrder = originalSortingOrder;
        draggedSR = null;
    }
}
