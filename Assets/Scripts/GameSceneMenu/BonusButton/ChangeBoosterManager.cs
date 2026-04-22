using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChangeBoosterManager : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private float collapseTime = 0.12f;
    [SerializeField] private float expandTime = 0.12f;
    
    private GameManager _cachedGameManager;
    private bool _isBusy;

    private GameManager GetGameManager()
    {
        if (_cachedGameManager == null || !_cachedGameManager.gameObject.activeInHierarchy)
            _cachedGameManager = FindFirstObjectByType<GameManager>();

        return _cachedGameManager;
    }

    public void UseBlueBooster()
    {
        if (_isBusy) return;
        StartCoroutine(UseBlueBoosterRoutine());
    }

    private IEnumerator UseBlueBoosterRoutine()
    {
        _isBusy = true;

        var gameManager = GetGameManager();
        if (gameManager == null)
        {
            _isBusy = false;
            yield break;
        }

        var database = ItemPrefabDatabase.Instance;
        if (database == null)
        {
            _isBusy = false;
            yield break;
        }

        GameObject targetPrefab = database.GetRandomPrefab();
        if (targetPrefab == null)
        {
            _isBusy = false;
            yield break;
        }

        // 1) Собираем уникальные ItemType на первом слое
        List<ItemType> uniqueTypesOnFirstLayer = new();

        foreach (var shelf in gameManager.shelves)
        {
            foreach (var slot in shelf.slots)
            {
                if (slot == null || slot.IsEmpty || slot.Item == null)
                    continue;

                ItemType type = slot.Item.Type;
                if (!uniqueTypesOnFirstLayer.Contains(type))
                    uniqueTypesOnFirstLayer.Add(type);
            }
        }

        if (uniqueTypesOnFirstLayer.Count == 0)
        {
            _isBusy = false;
            yield break;
        }

        // 2) Перемешиваем и берём до 4 уникальных типов
        for (int i = 0; i < uniqueTypesOnFirstLayer.Count; i++)
        {
            int j = Random.Range(i, uniqueTypesOnFirstLayer.Count);
            (uniqueTypesOnFirstLayer[i], uniqueTypesOnFirstLayer[j]) =
                (uniqueTypesOnFirstLayer[j], uniqueTypesOnFirstLayer[i]);
        }

        int countToReplace = Mathf.Min(4, uniqueTypesOnFirstLayer.Count);
        List<ItemType> selectedTypes = uniqueTypesOnFirstLayer.Take(countToReplace).ToList();

        // 3) Собираем все видимые предметы, которые надо заменить
        List<ReplaceData> replaceList = CollectVisibleItemsToReplace(gameManager, selectedTypes);

        // 4) Анимируем схлопывание старых предметов
        yield return StartCoroutine(CollapseOldItems(replaceList));

        // 5) Меняем будущие слои
        foreach (var shelf in gameManager.shelves)
        {
            var stack = shelf.GetComponent<ShelfStack>();
            if (stack != null)
            {
                stack.ReplaceTypesInFutureLayers(selectedTypes, targetPrefab);
                stack.RefreshPreviewOnly();
            }
        }

        // 6) Заменяем видимые предметы на новые (scale=0)
        List<DraggableItem> newItems = ReplaceVisibleItemsWithPrefab(replaceList, targetPrefab);

        // 7) Анимируем появление новых
        yield return StartCoroutine(ExpandNewItems(newItems));

        var hint = FindFirstObjectByType<HintPulseManager>();
        if (hint != null)
            hint.RefreshIfActive();

        gameManager.CheckAllShelves();

        _isBusy = false;
    }

    private List<ReplaceData> CollectVisibleItemsToReplace(GameManager gameManager, List<ItemType> typesToReplace)
    {
        List<ReplaceData> result = new();

        foreach (var shelf in gameManager.shelves)
        {
            foreach (var slot in shelf.slots)
            {
                if (slot == null || slot.IsEmpty || slot.Item == null)
                    continue;

                DraggableItem oldItem = slot.Item;
                if (!typesToReplace.Contains(oldItem.Type))
                    continue;

                result.Add(new ReplaceData
                {
                    slot = slot,
                    oldItem = oldItem
                });
            }
        }

        return result;
    }

    private IEnumerator CollapseOldItems(List<ReplaceData> replaceList)
    {
        float t = 0f;
        List<Vector3> startScales = new();

        foreach (var data in replaceList)
        {
            if (data.oldItem != null)
            {
                data.oldItem.StopPulse();
                startScales.Add(data.oldItem.transform.localScale);
            }
            else
            {
                startScales.Add(Vector3.one);
            }
        }

        while (t < collapseTime)
        {
            t += Time.deltaTime;
            float k = collapseTime <= 0f ? 1f : Mathf.Clamp01(t / collapseTime);
            float eased = 1f - Mathf.Pow(1f - k, 3f);

            for (int i = 0; i < replaceList.Count; i++)
            {
                if (replaceList[i].oldItem != null)
                {
                    replaceList[i].oldItem.transform.localScale =
                        Vector3.Lerp(startScales[i], Vector3.zero, eased);
                }
            }

            yield return null;
        }

        for (int i = 0; i < replaceList.Count; i++)
        {
            if (replaceList[i].oldItem != null)
                replaceList[i].oldItem.transform.localScale = Vector3.zero;
        }
    }

    private List<DraggableItem> ReplaceVisibleItemsWithPrefab(List<ReplaceData> replaceList, GameObject targetPrefab)
    {
        List<DraggableItem> newItems = new();

        foreach (var data in replaceList)
        {
            if (data.slot == null) continue;

            if (data.oldItem != null)
            {
                data.slot.ClearItem();
                Destroy(data.oldItem.gameObject);
            }

            GameObject go = Instantiate(targetPrefab, data.slot.transform);
            go.transform.localPosition = new Vector3(0, -0.2f, 0);
            go.transform.localScale = Vector3.zero;

            DraggableItem newItem = go.GetComponent<DraggableItem>();
            if (newItem == null)
            {
                Debug.LogError($"Prefab {targetPrefab.name} has no DraggableItem");
                Destroy(go);
                continue;
            }

            data.slot.SetItem(newItem);
            newItems.Add(newItem);
        }

        return newItems;
    }

    private IEnumerator ExpandNewItems(List<DraggableItem> newItems)
    {
        float t = 0f;

        while (t < expandTime)
        {
            t += Time.deltaTime;
            float k = expandTime <= 0f ? 1f : Mathf.Clamp01(t / expandTime);
            float eased = 1f - Mathf.Pow(1f - k, 3f);

            foreach (var item in newItems)
            {
                if (item != null)
                    item.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, eased);
            }

            yield return null;
        }

        foreach (var item in newItems)
        {
            if (item != null)
                item.transform.localScale = Vector3.one;
        }
    }

    private class ReplaceData
    {
        public Slot slot;
        public DraggableItem oldItem;
    }
}
