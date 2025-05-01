using System.Collections.Generic;
using UnityEngine;

public class StorageUI : MonoBehaviour
{
    [HideInInspector] public Storage storageOpened;
    public Slot slotPrefab;
    public Transform content;
    [Space]
    public bool opened;
    public Vector3 openPosition;

    [Header("Furnace")]

    public GameObject furnaceUI;
    public GameObject turnOnButton;
    public GameObject turnOffButton;
    private void Update()
    {
        if (opened)
        {
            transform.localPosition = openPosition;
        }
        else
            transform.position = new Vector3(0, -1000, 0);

        if (storageOpened != null)
        {
            if (storageOpened.isFurnace)
            {
                if (storageOpened.GetComponent<Furnace>().isOn)
                {
                    for (int i = 0; i < storageOpened.slots.Length; i++)
                    {
                        Slot[] slots = GetComponentsInChildren<Slot>();
                        if (storageOpened.slots[i].data == null)
                        {
                            slots[i].data = null;
                        }
                        else
                        {
                            slots[i].data = storageOpened.slots[i].data;
                        }

                        slots[i].stackSize = storageOpened.slots[i].amount;
                        slots[i].UpdateSlot();
                    }
                }

                else
                {
                    for (int i = 0; i < storageOpened.slots.Length; i++)
                    {
                        Slot[] slots = GetComponentsInChildren<Slot>();
                        if (slots[i].data == null)
                        {
                            storageOpened.slots[i].data = null;
                        }
                        else
                        {
                            storageOpened.slots[i].data = slots[i].data;
                        }

                        storageOpened.slots[i].amount = slots[i].stackSize;
                    }

                }
                    furnaceUI.SetActive(true);
                if (storageOpened.GetComponent<Furnace>().isOn)
                {
                    turnOnButton.SetActive(false);
                    turnOffButton.SetActive(true);
                }
                else
                {
                    turnOnButton.SetActive(true);
                    turnOffButton.SetActive(false);
                }
            }
            else
            {
                furnaceUI.SetActive(false);
            }
        }
    }
    public void Open(Storage storage)
    {
        storageOpened = storage;
        for (int i = 0; i < storage.slots.Length; i++)
        {
            Slot slot = Instantiate(slotPrefab, content).GetComponent<Slot>();
            if(storage.slots[i].data == null)
            {
                slot.data = null;
            }
            else
            {
                slot.data = storage.slots[i].data;
            }

            slot.stackSize = storage.slots[i].amount;
            slot.UpdateSlot();
        }
        opened = true;
    }

    public void Close()
    {
        if(storageOpened == null)
        {
            return;
        }
        storageOpened.Close(GetComponentsInChildren<Slot>());
        Slot[] slotsToDestroy = GetComponentsInChildren<Slot>();
        for (int i = 0; i < slotsToDestroy.Length; i++)
        {
            Destroy(slotsToDestroy[i].gameObject);
        }
        storageOpened = null;
        opened = false;
    }

    public void TurnOn()
    {
        if (storageOpened == null)
        {
            return;
        }

        storageOpened.GetComponent<Furnace>().TurnOn();
    }

    public void TurnOff()
    {
        if (storageOpened == null)
        {
            return;
        }

        storageOpened.GetComponent<Furnace>().TurnOff();
    }
}
