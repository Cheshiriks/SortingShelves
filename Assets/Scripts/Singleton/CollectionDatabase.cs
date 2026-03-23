using System.Collections.Generic;
using UnityEngine;

public class CollectionDatabase : MonoBehaviour
{
    public static CollectionDatabase Instance;

    [SerializeField] private List<CollectionItemData> items = new();

    public IReadOnlyList<CollectionItemData> Items => items;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public CollectionItemData GetById(string id)
    {
        return items.Find(x => x.id == id);
    }

    public List<CollectionItemData> GetItems()
    {
        return items;
    }
    
    public int Size()
    {
        return items.Count;
    }
}
