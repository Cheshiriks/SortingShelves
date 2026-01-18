using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DraggableItem : MonoBehaviour
{
    public ItemType Type;

    [HideInInspector] public Slot CurrentSlot;

    private Camera cam;
    private bool dragging;
    private Vector3 grabOffset;
    private int originalSortingOrder;

    private SpriteRenderer sr;
    private Slot startSlot;

    private void Awake()
    {
        cam = Camera.main;
        sr = GetComponent<SpriteRenderer>();
        if (sr) originalSortingOrder = sr.sortingOrder;
    }

    private void OnMouseDown()
    {
        Debug.Log("CLICK: " + name);
        
        if (CurrentSlot == null) return;

        dragging = true;
        startSlot = CurrentSlot;

        // освобождаем слот на время перетаскивания (чтобы можно было кинуть обратно/в другой)
        startSlot.ClearItem();

        Vector3 mouseWorld = GetMouseWorld();
        grabOffset = transform.position - mouseWorld;

        if (sr) sr.sortingOrder = 999; // чтобы был поверх
    }

    private void OnMouseDrag()
    {
        if (!dragging) return;
        transform.position = GetMouseWorld() + grabOffset;
    }

    private void OnMouseUp()
    {
        if (!dragging) return;
        dragging = false;

        if (sr) sr.sortingOrder = originalSortingOrder;

        Slot target = RaycastSlotUnderMouse();

        // Правила:
        // 1) нельзя бросить в пустоту -> вернуть обратно
        // 2) можно бросить только в пустой слот (или сделать swap по желанию)

        if (target != null && target.IsEmpty)
        {
            MoveToSlot(target);
        }
        else
        {
            // вернуть обратно
            MoveToSlot(startSlot);
        }

        GameManager.I.CheckAllShelves();
    }

    public void MoveToSlot(Slot slot)
    {
        if (slot == null)
        {
            Debug.LogWarning($"MoveToSlot called with null for {name}");
            return;
        }

        slot.SetItem(this);
        transform.position = slot.SnapPosition;
        transform.SetParent(slot.transform);
    }

    private Vector3 GetMouseWorld()
    {
        Vector3 m = Input.mousePosition;
        m.z = Mathf.Abs(cam.transform.position.z);
        return cam.ScreenToWorldPoint(m);
    }

    private Slot RaycastSlotUnderMouse()
    {
        Vector2 pos = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);

        if (hit.collider == null) return null;
        return hit.collider.GetComponent<Slot>();
    }
    
}
