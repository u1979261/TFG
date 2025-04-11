using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour
{
    [HideInInspector]public StorageSlot[] slots;
    public StorageSlot slotPrefab;
    public int StorageSize= 12;
    [Space]
    public bool opened;

    [Header("Drop Config")]
    public Transform dropPos;
    public Pickup dropBag;

    [Header("Furnace")]
    [HideInInspector] public bool isFurnace;

    private void OnDestroy()
    {
        Slot[] slots = GetComponentsInChildren<Slot>();
        for (int i = 0; i < slots.Length; i++)
        {
            DropItem(slots[i].data, slots[i].stackSize);
        }
    }
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

    //public void AddItem(ItemSO item, int amount)
    //{
    //    for (int i = 0; i < slots.Length; i++)
    //    {
    //        if (slots[i].data == null)
    //        {
    //            slots[i].data = item;
    //            slots[i].amount = amount;
    //            break;
    //        }
    //        else if (slots[i].data == item)
    //        {
    //            slots[i].amount += amount;
    //            break;
    //        }
    //    }
    //}

    public void AddItem(ItemSO data, int stackSize)
    {
        StorageSlot[] slots = GetComponentsInChildren<StorageSlot>();
        if (data.isStackable)
        {
            StorageSlot stackableSlot = null;

            // TRY FINDING STACKABLE SLOT
            for (int i = 0; i < slots.Length; i++)
            {
                if (!slots[i].isEmpty)
                {
                    if (slots[i].data == data && slots[i].amount < data.maxStack)
                    {
                        stackableSlot = slots[i];
                        break;
                    }

                }
            }

            if (stackableSlot != null)
            {

                // IF IT CANNOT FIT THE PICKED UP AMOUNT
                if (stackableSlot.amount + stackSize > data.maxStack)
                {
                    int amountLeft = (stackableSlot.amount + stackSize) - data.maxStack;

                    // ADD IT TO THE STACKABLE SLOT
                    stackableSlot.AddItemToSlot(data, data.maxStack);

                    // TRY FIND A NEW EMPTY STACK
                    for (int i = 0; i < slots.Length; i++)
                    {
                        if (slots[i].isEmpty)
                        {
                            slots[i].AddItemToSlot(data, amountLeft);

                            break;
                        }
                    }
                }
                // IF IT CAN FIT THE PICKED UP AMOUNT
                else
                {
                    stackableSlot.AddStackAmount(stackSize);
                }

            }
            else
            {
                StorageSlot emptySlot = null;


                // FIND EMPTY SLOT
                for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i].isEmpty)
                    {
                        emptySlot = slots[i];
                        break;
                    }
                }

                // IF WE HAVE AN EMPTY SLOT THAN ADD THE ITEM
                if (emptySlot != null)
                {
                    emptySlot.AddItemToSlot(data, stackSize);
                }
                else
                {
                    DropItem(data, stackSize);
                }
            }

        }
        else
        {
            StorageSlot emptySlot = null;


            // FIND EMPTY SLOT
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].isEmpty)
                {
                    emptySlot = slots[i];
                    break;
                }
            }

            // IF WE HAVE AN EMPTY SLOT THAN ADD THE ITEM
            if (emptySlot != null)
            {
                emptySlot.AddItemToSlot(data, stackSize);
            }
            else
            {
                DropItem(data, stackSize);
            }

        }
    }
    public void DropItem(ItemSO data, int stack)
    {
        Debug.Log(dropPos);
        Pickup drop = Instantiate(dropBag.gameObject, dropPos).GetComponent<Pickup>();
        drop.transform.SetParent(null);

        drop.data = data;
        drop.stackSize = stack;
    }

}


