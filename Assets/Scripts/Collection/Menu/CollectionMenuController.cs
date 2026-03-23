using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YG;
using TMPro;

public class CollectionMenuController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private CollectionDatabase collectionDatabase;

    [Header("UI")]
    [SerializeField] private RectTransform content;
    [SerializeField] private CollectionShelfRowView shelfPrefab;
    [SerializeField] private int itemsPerShelf = 5;
    [SerializeField] private bool showAtLeastOneShelf = true;
    [SerializeField] private TextMeshProUGUI progressText;

    private readonly List<CollectionShelfRowView> spawnedShelves = new();
    private Dictionary<string, Sprite> spriteById;

    private void Awake()
    {
        BuildSpriteMap();
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        BuildSpriteMap();

        List<CollectionProgressEntry> progressEntries = GetCollectionProgress();
        List<CollectionMenuItemViewData> visibleItems = BuildVisibleItems(progressEntries);

        RebuildShelves(visibleItems);
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);

        int complete = progressEntries.FindAll(o => o.count == 4).Count;
        int allObjects = collectionDatabase.Size();
        progressText.text = "СОБРАНО " + complete + "/" + allObjects;
    }

    private void BuildSpriteMap()
    {
        spriteById = new Dictionary<string, Sprite>();

        if (collectionDatabase == null || collectionDatabase.GetItems() == null)
            return;

        foreach (CollectionItemData item in collectionDatabase.GetItems())
        {
            if (item == null || string.IsNullOrEmpty(item.id) || item.sprite == null)
                continue;

            if (!spriteById.ContainsKey(item.id))
                spriteById.Add(item.id, item.sprite);
        }
    }

    private List<CollectionProgressEntry> GetCollectionProgress()
    {
        return YG2.saves.collectionProgress;
    }

    private List<CollectionMenuItemViewData> BuildVisibleItems(List<CollectionProgressEntry> progressEntries)
    {
        List<CollectionMenuItemViewData> result = new();

        if (progressEntries == null)
            return result;

        foreach (CollectionProgressEntry entry in progressEntries)
        {
            if (entry == null || string.IsNullOrEmpty(entry.id))
                continue;

            if (entry.count < 1)
                continue;

            if (!spriteById.TryGetValue(entry.id, out Sprite sprite))
                continue;

            result.Add(new CollectionMenuItemViewData
            {
                id = entry.id,
                count = Mathf.Clamp(entry.count, 1, 4),
                sprite = sprite
            });
        }

        return result;
    }

    private void RebuildShelves(List<CollectionMenuItemViewData> items)
    {
        ClearShelves();

        int shelfCount;
        if (items.Count == 0)
        {
            shelfCount = showAtLeastOneShelf ? 1 : 0;
        }
        else
        {
            shelfCount = Mathf.CeilToInt(items.Count / (float)itemsPerShelf);
        }

        for (int shelfIndex = 0; shelfIndex < shelfCount; shelfIndex++)
        {
            CollectionShelfRowView shelfView = Instantiate(shelfPrefab, content);
            spawnedShelves.Add(shelfView);

            int startIndex = shelfIndex * itemsPerShelf;
            int count = Mathf.Min(itemsPerShelf, items.Count - startIndex);

            List<CollectionMenuItemViewData> shelfItems = new();

            for (int i = 0; i < count; i++)
                shelfItems.Add(items[startIndex + i]);

            shelfView.Setup(shelfItems);
        }
    }

    private void ClearShelves()
    {
        for (int i = spawnedShelves.Count - 1; i >= 0; i--)
        {
            if (spawnedShelves[i] != null)
                Destroy(spawnedShelves[i].gameObject);
        }

        spawnedShelves.Clear();
    }
}
