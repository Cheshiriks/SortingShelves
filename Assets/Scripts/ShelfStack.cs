using System.Collections.Generic;
using UnityEngine;

public class ShelfStack : MonoBehaviour
{
    [Header("Slots on THIS shelf (3)")]
    public Slot[] slots;

    [Header("Layers (0 = current, 1 = next preview, дальше скрыты)")]
    public List<ShelfPrefabLayer> layers = new();

    [Header("Ghost preview")]
    [Range(0f, 1f)] public float ghostRGB = 0.25f;
    public int ghostSortingOffset = -10; // чтобы тень была позади

    private int currentIndex = -1;

    // призраки по слотам
    private GameObject[] ghostObjects;

    private void Awake()
    {
        // автоподхват слотов, если не задано
        if (slots == null || slots.Length == 0)
            slots = GetComponentsInChildren<Slot>(true);

        ghostObjects = new GameObject[3];
    }

    private void Start()
    {
        // Если ты уже руками положил предметы в слоты в сцене — лучше НЕ спавнить 1-й слой автоматически.
        // Но в твоей новой механике логичнее, чтобы слои управляли содержимым.
        AdvanceToNextLayer(); // загрузим первый слой
    }

    public bool IsEmpty()
    {
        for (int i = 0; i < 3; i++)
            if (!slots[i].IsEmpty) return false;
        return true;
    }

    /// Вызывать после каждого хода/проверки
    public void TryAdvanceIfEmpty()
    {
        if (IsEmpty())
            AdvanceToNextLayer();
    }

    private void AdvanceToNextLayer()
    {
        currentIndex++;

        // очистим ghost предыдущий
        ClearGhosts();

        // если слоёв больше нет — просто выходим
        if (currentIndex >= layers.Count)
            return;

        // 1) спавним текущий слой в слоты
        SpawnLayer(layers[currentIndex]);

        // 2) показываем тень следующего (если есть)
        int next = currentIndex + 1;
        if (next < layers.Count)
            ShowGhostLayer(layers[next]);
    }

    private void SpawnLayer(ShelfPrefabLayer layer)
    {
        for (int i = 0; i < 3; i++)
        {
            // гарантированно очистим слот (на всякий случай)
            if (!slots[i].IsEmpty)
            {
                Destroy(slots[i].Item.gameObject);
                slots[i].ClearItem();
            }

            var prefab = (layer.prefabs != null && i < layer.prefabs.Length) ? layer.prefabs[i] : null;
            if (prefab == null) continue;

            // создаём предмет как ребёнка слота
            var go = Instantiate(prefab, slots[i].transform);
            //go.transform.localPosition = Vector3.zero;

            // зарегистрируемся в Slot
            var item = go.GetComponent<DraggableItem>();
            if (!item)
            {
                Debug.LogError($"Prefab {prefab.name} has no DraggableItem", prefab);
                continue;
            }

            slots[i].SetItem(item);
        }
    }

    private void ShowGhostLayer(ShelfPrefabLayer layer)
    {
        for (int i = 0; i < 3; i++)
        {
            var prefab = (layer.prefabs != null && i < layer.prefabs.Length) ? layer.prefabs[i] : null;
            if (prefab == null) continue;

            // ghost создаём как ребёнка слота (за предметами)
            var ghost = Instantiate(prefab, slots[i].transform);
            ghost.transform.localPosition = Vector3.zero;

            // 1) выключаем Drag/Colliders
            var drag = ghost.GetComponent<DraggableItem>();
            if (drag) Destroy(drag); // или drag.enabled=false, но Destroy проще
            foreach (var c in ghost.GetComponentsInChildren<Collider2D>(true))
                c.enabled = false;

            // 2) делаем прозрачным
            foreach (var sr in ghost.GetComponentsInChildren<SpriteRenderer>(true))
            {
                Color c = sr.color;

                c.r = ghostRGB;
                c.g = ghostRGB;
                c.b = ghostRGB;
                c.a = 1f; // полностью непрозрачный

                sr.color = c;

                sr.sortingOrder += ghostSortingOffset;
            }

            ghostObjects[i] = ghost;
        }
    }

    private void ClearGhosts()
    {
        for (int i = 0; i < ghostObjects.Length; i++)
        {
            if (ghostObjects[i])
                Destroy(ghostObjects[i]);
            ghostObjects[i] = null;
        }
    }
}
