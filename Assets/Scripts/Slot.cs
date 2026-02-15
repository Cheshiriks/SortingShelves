using UnityEngine;

public class Slot : MonoBehaviour
{
    public Shelf shelf;          // назначь в инспекторе или автоматом из родителя
    public DraggableItem Item { get; private set; }

    public bool IsEmpty => Item == null;

    public Vector3 SnapPosition => transform.position;

    private void Awake()
    {
        // если предмет уже лежит в слоте в сцене — зарегистрировать
        var existing = GetComponentInChildren<DraggableItem>();
        if (existing != null)
        {
            SetItem(existing);
            existing.transform.position = new Vector3(SnapPosition.x, SnapPosition.y-0.1f);
            existing.transform.SetParent(transform); // чтобы точно был ребёнком слота
        }
    }
    
    public void SetItem(DraggableItem item)
    {
        if (item == null)
        {
            ClearItem();
            return;
        }

        // Если этот предмет уже был в другом слоте — убрать его оттуда
        if (item.CurrentSlot != null && item.CurrentSlot != this)
        {
            item.CurrentSlot.ClearItem();
        }

        // Если в этом слоте уже что-то лежит — (по твоим правилам не должно быть, но на всякий случай)
        if (Item != null && Item != item)
        {
            Item.CurrentSlot = null;
        }

        Item = item;
        item.CurrentSlot = this;
    }

    public void ClearItem()
    {
        if (Item != null)
        {
            var oldItem = Item;
            Item = null;

            if (oldItem.CurrentSlot == this)
                oldItem.CurrentSlot = null;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!shelf) shelf = GetComponentInParent<Shelf>();
    }
#endif
}
