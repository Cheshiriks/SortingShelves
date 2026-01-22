using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }
    public Shelf[] shelves;

    private void Awake()
    {
        I = this;
    }

    public void CheckAllShelves()
    {
        bool anyMatch = false;
        
        foreach (var shelf in shelves)
        {
            if (shelf.HasTripleMatch())
            {
                Debug.Log($"MATCH on shelf: {shelf.name}");
                shelf.ClearMatchedTriple();
                anyMatch = true;
                // Тут: начислить очки, звук,анимации и т.п.
            }
        }
        
        // Если что-то удалили — проверяем победу
        if (anyMatch && AreAllShelvesEmpty())
        {
            Debug.Log("Уровень пройден!");
        }
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
