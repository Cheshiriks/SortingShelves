using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }
    public Shelf[] shelves;
    public DragController dragController;
    private bool _isResolving;
    private bool _gameOver;
    
    [Header("Present")]
    public PresentMenuTimer presentMenu;
    
    [Header("Menu")]
    [SerializeField] private GameObject winMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private UIBlocker uiBlocker;

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
}
