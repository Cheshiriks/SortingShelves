using System.Collections.Generic;
using UnityEngine;
using YG;

public class CollectionManager : MonoBehaviour
{
    public CollectionItemData GetRandomAvailableCollectionItem()
    {
        var available = new List<CollectionItemData>();

        foreach (var item in CollectionDatabase.Instance.Items)
        {
            if (GetCollectionCount(item.id) < 4)
                available.Add(item);
        }

        if (available.Count == 0)
            return null;

        int index = Random.Range(0, available.Count);
        return available[index];
    }
    
    public int GetCollectionCount(string id)
    {
        var entry = YG2.saves.collectionProgress.Find(x => x.id == id);
        return entry != null ? entry.count : 0;
    }

    public int AddCollectionCount(string id, int add = 1)
    {
        var entry = YG2.saves.collectionProgress.Find(x => x.id == id);

        if (entry == null)
        {
            entry = new CollectionProgressEntry { id = id, count = 0 };
            YG2.saves.collectionProgress.Add(entry);
        }

        entry.count = Mathf.Clamp(entry.count + add, 0, 4);
        
        YG2.SaveProgress();
        
        return entry.count;
    }

    public bool IsCollectionComplete(string id)
    {
        return GetCollectionCount(id) >= 4;
    }
    
    public bool HasUnfinishedCollectionItems()
    {
        foreach (var item in CollectionDatabase.Instance.Items)
        {
            if (GetCollectionCount(item.id) < 4)
                return true;
        }
        return false;
    }
}
