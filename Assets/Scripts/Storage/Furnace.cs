using UnityEngine;
using System.Collections.Generic;

public class Furnace : MonoBehaviour
{
    public Storage storage;

    [Space]
    [Header("Slots")]
    [HideInInspector] public StorageSlot fuelSlot;

    [Space]
    public bool isOn;
    public enum AllowedItemType
    {
        Resources,
        Consumable
    }
    public AllowedItemType allowedItemType = AllowedItemType.Resources;
    public GameObject fire;

    [Header("Fuel Timing")]
    private float fuelTimer;
    private float currentFuelTime;

    private const int maxSmeltingSlots = 2;

    [Header("Fuel Byproduct")]
    public ItemSO coalItem;
    public int coalAmountPerFuel = 1;

    private class SmeltData
    {
        public StorageSlot slot;
        public float currentTime;
        public float targetTime;
    }

    private List<SmeltData> smeltQueue = new List<SmeltData>();

    private void Start()
    {
        if (GetComponent<Storage>() != null)
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
        if (isOn)
        {
            // Buscar combustible si no hay
            if (fuelSlot == null)
            {
                for (int i = 0; i < storage.slots.Length; i++)
                {
                    if (storage.slots[i].data != null && storage.slots[i].data.isFuel)
                    {
                        fuelSlot = storage.slots[i];
                        currentFuelTime = 0f;
                        fuelTimer = fuelSlot.data.processTime;
                        break;
                    }
                }
            }

            // Buscar ítems para fundir si no hay activos
            if (smeltQueue.Count == 0)
            {
                smeltQueue.Clear();
                int foundSmeltables = 0;

                for (int i = 0; i < storage.slots.Length && foundSmeltables < maxSmeltingSlots; i++)
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
                            smeltQueue.Add(new SmeltData
                            {
                                slot = storage.slots[i],
                                currentTime = 0f,
                                targetTime = item.processTime
                            });
                            foundSmeltables++;
                        }
                    }
                }
            }

            // Apagar si no hay combustible
            if (fuelSlot == null || fuelSlot.data == null)
            {
                TurnOff();
                return;
            }

            // CONSUMIR COMBUSTIBLE
            if (currentFuelTime < fuelTimer)
            {
                currentFuelTime += Time.deltaTime;
            }
            else
            {
                currentFuelTime = 0f;
                fuelSlot.amount--;

                // Generar carbón como subproducto
                if (coalItem != null)
                {
                    storage.AddItem(coalItem, coalAmountPerFuel);
                }

                if (fuelSlot.amount <= 0)
                {
                    fuelSlot = null;
                }
            }

            // FUNDICIÓN MULTIPLE
            for (int i = smeltQueue.Count - 1; i >= 0; i--)
            {
                var smeltData = smeltQueue[i];

                if (smeltData.slot.amount <= 0 || smeltData.slot.data == null)
                {
                    smeltQueue.RemoveAt(i);
                    continue;
                }

                smeltData.currentTime += Time.deltaTime;

                if (smeltData.currentTime >= smeltData.targetTime)
                {
                    // Verificar si hay espacio para el resultado
                    if (CanStoreItem(smeltData.slot.data.outcome, smeltData.slot.data.outcomeAmount))
                    {
                        smeltData.currentTime = 0f;
                        storage.AddItem(smeltData.slot.data.outcome, smeltData.slot.data.outcomeAmount);
                        smeltData.slot.amount--;

                        if (smeltData.slot.amount <= 0)
                        {
                            smeltQueue.RemoveAt(i);
                        }
                    }
                    else
                    {
                        // Si no hay espacio, apagar todo
                        TurnOff();
                        break;
                    }
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
        smeltQueue.Clear();
    }

    private bool CanStoreItem(ItemSO item, int amount)
    {
        for (int i = 0; i < storage.slots.Length; i++)
        {
            var slot = storage.slots[i];

            if (slot.data == null)
            {
                return true; // slot vacío
            }
            else if (slot.data == item && slot.amount + amount <= slot.data.maxStack)
            {
                return true; // mismo ítem y hay espacio
            }
        }

        return false; // no hay ningún lugar para guardarlo
    }
}
