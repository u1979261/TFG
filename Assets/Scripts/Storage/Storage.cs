using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour
{
    [HideInInspector]public StorageSlot[] slots;
    public StorageSlot slotPrefab;
    public int StorageSize= 12;
    [Space]
    public bool opened;


    [Header("Furnace")]
    [HideInInspector] public bool isFurnace;


    private void Start()
    {
        isFurnace = GetComponent<Furnace>() != null;
        List<StorageSlot> slotList = new List<StorageSlot>();
        for (int i = 0; i < StorageSize; i++)
        {
            StorageSlot slot = Instantiate(slotPrefab, transform).GetComponent<StorageSlot>();
            slotList.Add(slot);
        }
        slots = slotList.ToArray();
    }
    public void Open(StorageUI UI)
    {
        UI.Open(this);
        opened = true;
    }

    public void Close(Slot[] UISlots)
    {
        for (int i = 0; i < UISlots.Length; i++)
        {
            if (UISlots[i].data == null)
            {
                slots[i].data = null;
            }
            else
            {
                slots[i].data = UISlots[i].data;
            }
            slots[i].amount = UISlots[i].stackSize;
        }
        opened = false;
    }

    public void AddItem(ItemSO item, int amount)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].data == null)
            {
                slots[i].data = item;
                slots[i].amount = amount;
                break;
            }
            else if (slots[i].data == item)
            {
                slots[i].amount += amount;
                break;
            }
        }
    }

}


