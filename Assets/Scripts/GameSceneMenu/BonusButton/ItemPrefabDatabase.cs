using System.Collections.Generic;
using UnityEngine;

public class ItemPrefabDatabase : MonoBehaviour
{
    public static ItemPrefabDatabase Instance;

    [SerializeField] private List<GameObject> allItemPrefabs = new();

    public IReadOnlyList<GameObject> AllItemPrefabs => allItemPrefabs;

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

    public GameObject GetRandomPrefab()
    {
        if (allItemPrefabs == null || allItemPrefabs.Count == 0)
            return null;

        return allItemPrefabs[Random.Range(0, allItemPrefabs.Count)];
    }
}
