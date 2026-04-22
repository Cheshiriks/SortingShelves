using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class HintPulseManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textDestroy;
    [SerializeField] private Image colorDestroy;
    
    private bool _isActive;
    private GameManager cachedGameManager;

    public bool IsActive => _isActive;

    private GameManager GetGameManager()
    {
        if (cachedGameManager == null || !cachedGameManager.gameObject.activeInHierarchy)
            cachedGameManager = FindFirstObjectByType<GameManager>();

        return cachedGameManager;
    }

    public void TogglePulse()
    {
        _isActive = !_isActive;

        if (_isActive)
            StartPulseForAllCurrentItems();
        else
            StopPulseForAllCurrentItems();
    }

    public void UseOnItem(DraggableItem item)
    {
        if (!_isActive || item == null) return;

        // уменьшаем количество доступных бонусов 
        SaveGame.Instance.AddBonusDestroy(-1);
        StartDestroy();
        
        // выключаем режим
        TogglePulse();

        var gm = GetGameManager();
        if (gm != null)
            gm.RemoveAllOfType(item.Type);
    }

    private void StartDestroy()
    {
        int bonusDestroyCount = SaveGame.Instance.BonusDestroy;
        
        if (bonusDestroyCount == 0)
        {
            textDestroy.text = "+";
            colorDestroy.color = Color.red;
        } else
        {
            textDestroy.text = bonusDestroyCount.ToString();
            colorDestroy.color = new Color32(15, 171, 0, 255);
        }
    }
    
    public void RefreshIfActive()
    {
        if (!_isActive) return;

        StopPulseForAllCurrentItems();
        StartPulseForAllCurrentItems();
    }

    private void StartPulseForAllCurrentItems()
    {
        var gameManager = GetGameManager();
        if (gameManager == null) return;

        foreach (var shelf in gameManager.shelves)
        {
            foreach (var slot in shelf.slots)
            {
                if (slot != null && !slot.IsEmpty && slot.Item != null)
                    slot.Item.StartPulse();
            }
        }
    }

    private void StopPulseForAllCurrentItems()
    {
        var gameManager = GetGameManager();
        if (gameManager == null) return;

        foreach (var shelf in gameManager.shelves)
        {
            foreach (var slot in shelf.slots)
            {
                if (slot != null && !slot.IsEmpty && slot.Item != null)
                    slot.Item.StopPulse();
            }
        }
    }
}
