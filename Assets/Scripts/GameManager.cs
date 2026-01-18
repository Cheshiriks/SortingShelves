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
        foreach (var shelf in shelves)
        {
            if (shelf.HasTripleMatch())
            {
                Debug.Log($"MATCH on shelf: {shelf.name}");
                // Тут: начислить очки, заблокировать полку, убрать предметы, анимации и т.п.
            }
        }
    }
}
