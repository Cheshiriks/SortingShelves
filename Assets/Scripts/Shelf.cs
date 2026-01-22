using System;
using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    public Slot[] slots; // ровно 3
    [SerializeField] private List<Boolean> items;
    
    public bool HasTripleMatch()
    {
        // все заняты?
        if (slots.Length < 3) return false;
        if (slots[0].IsEmpty || slots[1].IsEmpty || slots[2].IsEmpty) return false;

        var t0 = slots[0].Item.Type;
        return slots[1].Item.Type == t0 && slots[2].Item.Type == t0;
    }

    // --------
    // проверка заполненности полок
    private void Update()
    {
        List<Boolean> bools = new List<Boolean>(); 
        foreach (var slot in slots)
        {
            bools.Add(slot.IsEmpty);
        }

        items = bools;
    }
    
    public void ClearMatchedTriple()
    {
        // на всякий случай
        if (!HasTripleMatch()) return;

        // 1) сохраняем ссылки на предметы
        var a = slots[0].Item;
        var b = slots[1].Item;
        var c = slots[2].Item;

        // 2) чистим слоты (чтобы логика стала пустой сразу)
        foreach (var s in slots)
            s.ClearItem();

        // 3) удаляем предметы из сцены
        if (a) Destroy(a.gameObject);
        if (b) Destroy(b.gameObject);
        if (c) Destroy(c.gameObject);
    }
}
