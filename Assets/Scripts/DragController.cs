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
    private HintPulseManager _hintPulseManager;

    private void Awake()
    {
        cam = Camera.main;
        _hintPulseManager = FindFirstObjectByType<HintPulseManager>();
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

        DraggableItem candidate = hit.collider.GetComponent<DraggableItem>();
        if (!candidate) return;
        if (candidate.CurrentSlot == null) return;

        if (IsItemLocked(candidate))
            return;
        
        // Если активен режим пульсации — используем бустер вместо перетаскивания
        if (_hintPulseManager != null && _hintPulseManager.IsActive)
        {
            _hintPulseManager.UseOnItem(candidate);
            return;
        }

        // проверяем, что полка не заблокирована
        var shelfLock = candidate.CurrentSlot.shelf.GetComponent<ShelfLock>();
        if (shelfLock != null && shelfLock.IsLocked)
            return;

        // Только теперь действительно "подбираем" предмет
        dragged = candidate;
        dragged.OnPickup();

        startSlot = dragged.CurrentSlot;
        startSlot.ClearItem();

        grabbedVisualSetup();

        Vector3 mouseWorld = GetMouseWorld();
        grabOffset = dragged.transform.position - mouseWorld;
    }
    
    private bool IsItemLocked(DraggableItem item)
    {
        if (item == null || item.CurrentSlot == null) return false;

        var shelfLock = item.CurrentSlot.shelf.GetComponent<ShelfLock>();
        return shelfLock != null && shelfLock.IsLocked;
    }

    private void Drag()
    {
        if (!dragged) return;
        dragged.transform.position = GetMouseWorld() + grabOffset + dragged.VisualOffset;
    }

    private void Drop()
    {
        if (!dragged) return;

        Slot target = RaycastSlotUnderMouse();

        // проверяем, что полка не заблокирована
        if (target != null)
        {
            var targetLock = target.shelf.GetComponent<ShelfLock>();
            if (targetLock != null && targetLock.IsLocked)
            {
                target = null; // считаем, что положить нельзя
            }
        }
        
        // 1) Отпустили в пустоту
        if (target == null)
        {
            dragged.MoveToSlot(startSlot, () =>
            {
                GameManager.I.CheckAllShelves();
            });
            FinishDrop();
            return;
        }

        // 2) Целевой слот пустой — кладем прямо туда
        if (target.IsEmpty)
        {
            dragged.MoveToSlot(target, () =>
            {
                GameManager.I.CheckAllShelves();
            });
            
            FinishDrop();
            return;
        }

        // 3) Целевой слот занят — ищем первый пустой на ЭТОЙ полке
        Shelf shelf = target.shelf;
        
        // проверяем, что полка не заблокирована
        var shelfLock = shelf.GetComponent<ShelfLock>();
        if (shelfLock != null && shelfLock.IsLocked)
        {
            dragged.MoveToSlot(startSlot, () =>
            {
                GameManager.I.CheckAllShelves();
            });
            FinishDrop();
            return;
        }
        
        Slot emptyOnShelf = FindFirstEmptySlot(shelf);

        if (emptyOnShelf != null)
            dragged.MoveToSlot(emptyOnShelf, () =>
            {
                GameManager.I.CheckAllShelves();
            });
        else
            dragged.MoveToSlot(startSlot, () =>
            {
                GameManager.I.CheckAllShelves();
            });

        FinishDrop();
    }
    
    
    private Slot FindFirstEmptySlot(Shelf shelf)
    {
        if (shelf == null || shelf.slots == null) return null;

        foreach (var s in shelf.slots)
            if (s != null && s.IsEmpty)
                return s;

        return null;
    }

    private void FinishDrop()
    {
        RestoreVisualSetup();
        dragged = null;
        startSlot = null;
        //GameManager.I.CheckAllShelves();
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
            draggedSR.sortingOrder = 99;
        }
    }

    private void RestoreVisualSetup()
    {
        if (draggedSR)
            draggedSR.sortingOrder = originalSortingOrder;
        draggedSR = null;
    }
}
