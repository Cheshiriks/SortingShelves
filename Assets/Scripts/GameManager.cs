using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }
    public Shelf[] shelves;
    public DragController dragController;
    private bool _isResolving;
    
    [Header("Present")]
    public PresentMenuTimer presentMenu;
    
    [Header("Win")]
    public GameObject winMenu;

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
            // подтянуть следующий слой на этой полке (если используешь ShelfStack)
            var stack = matched.GetComponentInParent<ShelfStack>();
            if (stack) stack.TryAdvanceIfEmpty();

            if (dragController) dragController.enabled = true;
            _isResolving = false;

            // если хочешь разрешить "комбо" (следующий слой сразу дал тройку)
            // CheckAllShelves();

            // победа (если нужно)
            if (AreAllShelvesEmpty())
            {
                winMenu.SetActive(true);
                Debug.Log("Уровень пройден!");
            }
            else
            {
                TryShowPresentMenu();
            }
        });
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
}
