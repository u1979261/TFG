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

    private void Start()
    {
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

}


