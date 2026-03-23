using System.Collections.Generic;
using UnityEngine;

public class CollectionShelfRowView : MonoBehaviour
{
    [SerializeField] private CollectionItemView itemPrefab;
    [SerializeField] private List<RectTransform> slots = new List<RectTransform>(5);

    private readonly List<CollectionItemView> spawnedItems = new();

    public void Setup(List<CollectionMenuItemViewData> items)
    {
        ClearItems();

        int count = Mathf.Min(items.Count, slots.Count);

        for (int i = 0; i < count; i++)
        {
            CollectionItemView itemView = Instantiate(itemPrefab, slots[i]);
            spawnedItems.Add(itemView);

            RectTransform rt = itemView.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            rt.localScale = Vector3.one;

            itemView.Setup(items[i]);
        }
    }

    private void ClearItems()
    {
        for (int i = spawnedItems.Count - 1; i >= 0; i--)
        {
            if (spawnedItems[i] != null)
                Destroy(spawnedItems[i].gameObject);
        }

        spawnedItems.Clear();
    }
}
