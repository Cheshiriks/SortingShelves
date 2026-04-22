using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }
    public Shelf[] shelves;
    public DragController dragController;
    private bool _isResolving;
    private bool _gameOver;
    private bool _isRemovingByBooster;
    
    [Header("Present")]
    public PresentMenuTimer presentMenu;
    
    [Header("Menu")]
    [SerializeField] private GameObject winMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private UIBlocker uiBlocker;
    
    [Header("Audio")]
    [SerializeField] private AudioClip collapseClip;

    private void Awake()
    {
        I = this;
        if (!presentMenu) presentMenu = FindFirstObjectByType<PresentMenuTimer>();
    }
    
    public void TryShowPresentMenu()
    {
        if (presentMenu) presentMenu.TryShow();
    }

    public void CheckAllShelves()
    {
        if (_isResolving) return;

        // найти первую полку с тройкой
        Shelf matched = null;
        foreach (var s in shelves)
        {
            if (s.HasTripleMatch())
            {
                matched = s;
                break;
            }
            var stack = s.GetComponentInParent<ShelfStack>();
            if (stack) stack.TryAdvanceIfEmpty();
        }

        if (matched == null) return;

        _isResolving = true;
        if (dragController) dragController.enabled = false;

        matched.ClearMatchedTripleAnimated(() =>
        {
            // уменьшаем look всех полок на 1
            OnSuccessfulClear();
            
            // подтянуть следующий слой на этой полке (если используешь ShelfStack)
            var stack = matched.GetComponentInParent<ShelfStack>();
            if (stack) stack.TryAdvanceIfEmpty();

            if (dragController) dragController.enabled = true;
            _isResolving = false;

            // если хочешь разрешить "комбо" (следующий слой сразу дал тройку)
            CheckAllShelves();

            // победа (если нужно)
            if (AreAllShelvesEmpty())
            {
                SaveGame.Instance.WinLevel();
                winMenu.SetActive(true);
                Debug.Log("Уровень пройден!");
            }
            else
            {
                TryShowPresentMenu();
            }
        });
    }
    
    public void CheckLoseCondition()
    {
        if (_gameOver) return;
        
        foreach (var shelf in shelves)
        {
            foreach (var slot in shelf.slots)
            {
                if (slot.IsEmpty)
                    return; // есть куда ходить -> не проиграл
            }
        }

        _gameOver = true;
        Debug.Log("Ты проиграл");
        
        uiBlocker.SetBlocked(true);
        StartCoroutine(LoseDelayRoutine());
    }

    private IEnumerator LoseDelayRoutine()
    {
        yield return new WaitForSeconds(1f);
        gameOverMenu.SetActive(true);
    }

    public bool AreAllShelvesEmpty()
    {
        foreach (var shelf in shelves)
        {
            foreach (var slot in shelf.slots)
            {
                if (!slot.IsEmpty)
                    return false;
            }
        }
        return true;
    }
    
    private void OnSuccessfulClear()
    {
        DecreaseAllShelfLocks(1);
    }
    
    public void DecreaseAllShelfLocks(int amount = 1)
    {
        foreach (var shelf in shelves)
        {
            var shelfLock = shelf.GetComponent<ShelfLock>();
            if (shelfLock != null && shelfLock.IsLocked)
                shelfLock.DecreaseLock(amount);
        }
    }
    
        public void RemoveAllOfType(ItemType type)
    {
        if (_isRemovingByBooster) return;
        StartCoroutine(RemoveAllOfTypeRoutine(type));
    }

    private IEnumerator RemoveAllOfTypeRoutine(ItemType type)
    {
        _isRemovingByBooster = true;

        if (dragController) dragController.enabled = false;

        // 1) Собираем все видимые предметы этого типа
        List<(Slot slot, DraggableItem item)> visibleItems = new();

        foreach (var shelf in shelves)
        {
            foreach (var slot in shelf.slots)
            {
                if (slot != null && !slot.IsEmpty && slot.Item != null && slot.Item.Type == type)
                {
                    visibleItems.Add((slot, slot.Item));
                }
            }
        }

        // 1.5 Проигрываем звук
        if (collapseClip != null)
            AudioManager.Instance.PlaySFX(collapseClip);
        
        // 2) Удаляем этот тип из будущих слоёв
        foreach (var shelf in shelves)
        {
            var stack = shelf.GetComponent<ShelfStack>();
            if (stack != null)
            {
                stack.RemoveTypeFromFutureLayers(type);
                stack.RefreshPreviewOnly();
            }
        }

        // 3) Анимируем видимые предметы
        yield return StartCoroutine(AnimateCollapseItems(visibleItems));

        // 4.5) Уменьшаем замки, как после обычного схлопывания
        OnSuccessfulClear();
        
        // 4) Чистим слоты и удаляем объекты
        foreach (var pair in visibleItems)
        {
            if (pair.slot != null && pair.slot.Item == pair.item)
                pair.slot.ClearItem();

            if (pair.item != null)
                Destroy(pair.item.gameObject);
        }

        // 5) Если полка опустела — подтягиваем следующий слой
        foreach (var shelf in shelves)
        {
            var stack = shelf.GetComponentInParent<ShelfStack>();
            if (stack != null)
                stack.TryAdvanceIfEmpty();
        }

        if (dragController) dragController.enabled = true;

        _isRemovingByBooster = false;
        
        if (AreAllShelvesEmpty())
        {
            SaveGame.Instance.WinLevel();
            winMenu.SetActive(true);
            Debug.Log("Уровень пройден!");
        }

        CheckAllShelves();
    }

    private IEnumerator AnimateCollapseItems(List<(Slot slot, DraggableItem item)> items)
    {
        float scaleUp = 1.3f;
        float upTime = 0.12f;
        float downTime = 0.12f;

        List<Vector3> startScales = new();
        foreach (var pair in items)
        {
            if (pair.item != null)
            {
                pair.item.StopPulse();
                startScales.Add(pair.item.transform.localScale);
            }
            else
            {
                startScales.Add(Vector3.one);
            }
        }

        // Scale up
        float t = 0f;
        while (t < upTime)
        {
            t += Time.deltaTime;
            float k = (upTime <= 0f) ? 1f : Mathf.Clamp01(t / upTime);

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].item != null)
                    items[i].item.transform.localScale = Vector3.Lerp(startScales[i], startScales[i] * scaleUp, k);
            }

            yield return null;
        }

        // Collapse
        t = 0f;
        while (t < downTime)
        {
            t += Time.deltaTime;
            float k = (downTime <= 0f) ? 1f : Mathf.Clamp01(t / downTime);

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].item != null)
                    items[i].item.transform.localScale = Vector3.Lerp(startScales[i] * scaleUp, Vector3.zero, k);
            }

            yield return null;
        }
    }
}
