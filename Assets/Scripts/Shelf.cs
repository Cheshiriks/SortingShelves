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

    private void Update()
    {
        List<Boolean> bools = new List<Boolean>(); 
        foreach (var slot in slots)
        {
            bools.Add(slot.IsEmpty);
        }

        items = bools;
    }
}
