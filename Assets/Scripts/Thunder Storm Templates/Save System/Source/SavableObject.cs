using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavableObject : MonoBehaviour
{
    public int ID;
    [Tooltip("If marked true it will create the object when loading. Use it if this object will be created during runtime.")]
    public bool instantiate;


    [HideInInspector] public int itemID;
    [HideInInspector] public int itemStack;

    [HideInInspector] public int [] StorageItemsID;
    [HideInInspector] public int [] StorageItemsStacks;

    private void Update()
    {
        //PICKUP
        if (GetComponent<Pickup>() != null)
        {
            if(GetComponent<Pickup>().data != null)
            {
                itemID = GetComponent<Pickup>().data.ID;
                itemStack = GetComponent<Pickup>().stackSize;
            }
            else
            {
                itemID = 0;
                itemStack = 0;
            }
        }
        else
        {
            itemID = 0;
            itemStack = 0;
        }

        //STORAGE
        if(GetComponent<Storage>() != null)
        {
            Storage storage = GetComponent<Storage>();
            List<int> items = new List<int>();
            List<int> stacks = new List<int>();
            for(int i = 0; i < storage.slots.Length; i++)
            {
                if (storage.slots[i].data == null)
                {
                    items.Add(0);
                    stacks.Add(0);
                }
                else
                {
                    items.Add(storage.slots[i].data.ID);
                    stacks.Add(storage.slots[i].amount);
                }
            }

            StorageItemsID = items.ToArray();
            StorageItemsStacks = stacks.ToArray();
        }
    }

}
