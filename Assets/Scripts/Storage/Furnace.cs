using JetBrains.Annotations;
using Mono.Cecil;
using UnityEngine;

public class Furnace : MonoBehaviour
{

    public Storage storage;
    [Space]
    [Space]
    [HideInInspector] public StorageSlot fuelSlot;
    [HideInInspector] public StorageSlot smeltingSlot;

    [Space]
    public bool isOn;
    public enum AllowedItemType
    {
        Resources,
        Consumable
    }
    public AllowedItemType allowedItemType = AllowedItemType.Resources;
    public GameObject fire;

    private float fuelTimer;
    private float currentFuelTime;

    private float smeltTimer;
    private float currentSmeltTime;



    private void Start()
    {
        if(GetComponent<Storage>() != null)
        {
            storage = GetComponent<Storage>();
        }
        else
        {
            Debug.LogError("FURNACE: Furnace does not have a storage script attached to it.");
        }
    }

    private void Update()
    {
        //FIND SLOTS

        if (isOn)
        {
            if (fuelSlot == null)
            {
                for (int i = 0; i < storage.slots.Length; i++)
                {
                    if (storage.slots[i].data != null)
                    {
                        if (storage.slots[i].data.isFuel)
                        {
                            fuelSlot = storage.slots[i];

                            currentFuelTime = 0f;
                            fuelTimer = fuelSlot.data.processTime;

                            break;
                        }
                    }
                }
            }

            if (smeltingSlot == null)
            {
                for (int i = 0; i < storage.slots.Length; i++)
                {
                    var item = storage.slots[i].data;

                    if (item != null && item.outcome != null)
                    {
                        bool isAllowed = false;

                        switch (allowedItemType)
                        {
                            case AllowedItemType.Resources:
                                if (item.itemType == ItemSO.ItemType.Generic)
                                    isAllowed = true;
                                break;
                            case AllowedItemType.Consumable:
                                if (item.itemType == ItemSO.ItemType.Consumable)
                                    isAllowed = true;
                                break;
                        }

                        if (isAllowed)
                        {
                            smeltingSlot = storage.slots[i];
                            currentSmeltTime = 0f;
                            smeltTimer = item.processTime;
                            break;
                        }
                    }
                }
            }

            if (fuelSlot == null)
            {
                TurnOff();
            }
            else
            {
                if (fuelSlot.data == null)
                {
                    TurnOff();
                }
            }
        }



        //SMELTING

        if (isOn)
        {
            //FUEL
            if (currentFuelTime < fuelTimer)
            {
                currentFuelTime += Time.deltaTime;
            }
            else
            {
                currentFuelTime = 0;
                fuelSlot.amount--;
            }

            //SMELTING
            if(currentSmeltTime < smeltTimer)
            {
                currentSmeltTime += Time.deltaTime;
            }
            else
            {
                currentSmeltTime = 0f;
                if (smeltingSlot != null)
                {
                    if (smeltingSlot.data != null)
                    {
                        storage.AddItem(smeltingSlot.data.outcome, smeltingSlot.data.outcomeAmount);
                    }
                    smeltingSlot.amount--;
                }
                
            }
        }
    }

    public void TurnOn()
    {
        isOn = true;
        fire.SetActive(true);
    }

    public void TurnOff()
    {
        isOn = false;
        fire.SetActive(false);

        fuelSlot = null;
        smeltingSlot = null;

    }
}
