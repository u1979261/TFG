using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveHandler : MonoBehaviour
{
    public static bool loaded = false;

    [Header("Savable Objects")]
    [Tooltip("You must drag here every savable object that you wish to load")]
    public List<SavableObject> savableObjects;

    private void Start()
    {
        Invoke("Load", 0.5f);
    }

    public void Load()
    {
        if (!Directory.Exists(Application.dataPath + "/saves") ||
            !File.Exists(Application.dataPath + "/saves/GameSave.save"))
        {
            return;
        }

        SaveData.Load();

        // Load player stats
        var playerStats = FindAnyObjectByType<PlayerStats>();
        playerStats.health = SaveData.Singleton.currentHealth;
        playerStats.hunger = SaveData.Singleton.currentHunger;
        playerStats.thirst = SaveData.Singleton.currentThirst;

        // Load day-night cycle
        FindAnyObjectByType<DayNightCicle>().timeOfDay = SaveData.Singleton.currentTime;

        // Load inventory
        InventoryManager inventory = FindAnyObjectByType<Player>().GetComponentInChildren<InventoryManager>();
        ItemDataBase dataBase = FindAnyObjectByType<ItemDataBase>();

        int[] IDs = SaveData.Singleton.itemsIDs.ToArray();
        int[] stacks = SaveData.Singleton.itemsStacks.ToArray();

        for (int i = 0; i < inventory.inventorySlots.Length; i++)
        {
            if (IDs[i] == 0)
            {
                inventory.inventorySlots[i].data = null;
                inventory.inventorySlots[i].stackSize = 0;
            }
            else
            {
                inventory.inventorySlots[i].data = dataBase.GetItemData(IDs[i]);
                inventory.inventorySlots[i].stackSize = stacks[i];
            }
            inventory.inventorySlots[i].UpdateSlot();
        }

        // Destroy instanced savable objects
        SavableObject[] objectsToDestroy = FindObjectsByType<SavableObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var obj in objectsToDestroy)
        {
            if (obj.instantiate)
                Destroy(obj.gameObject);
        }

        // Load savable objects
        foreach (SAVE_SavableObject save in SaveData.Singleton.savableObjects)
        {
            SavableObject obj = null;

            foreach (var candidate in savableObjects)
            {
                if (candidate.ID == save.Id)
                {
                    obj = candidate;
                    break;
                }
            }

            if (obj == null)
            {
                Debug.LogError($"SAVE SYSTEM : The object to load (ID: {save.Id}) was not found. Make sure it is in the Savable Objects list.");
                continue;
            }

            if (obj.instantiate)
            {
                SavableObject instance = Instantiate(obj.gameObject).GetComponent<SavableObject>();

                instance.ID = save.Id;
                instance.transform.position = save.position;
                instance.transform.rotation = save.rotation;

                // Drop bag
                instance.itemID = save.itemID;
                instance.itemStack = save.itemStack;

                if (instance.itemID != 0)
                {
                    Pickup pickup = instance.GetComponent<Pickup>();
                    if (pickup != null)
                    {
                        pickup.data = dataBase.GetItemData(instance.itemID);
                        pickup.stackSize = instance.itemStack;
                    }
                    else
                    {
                        Debug.LogError($"SAVE SYSTEM : Object {instance.name} missing Pickup component.");
                        continue;
                    }
                }

                // Storage system
                Storage storage = instance.GetComponent<Storage>();
                if (storage != null)
                {
                    storage.GenerateSlots();
                    for (int i = 0; i < storage.slots.Length; i++)
                    {
                        if (save.StorageItemsData[i] == 0)
                        {
                            storage.slots[i].data = null;
                            storage.slots[i].amount = 0;
                        }
                        else
                        {
                            storage.slots[i].data = dataBase.GetItemData(save.StorageItemsData[i]);
                            storage.slots[i].amount = save.StorageItemsStack[i]; 
                        }
                    }
                }
            }
            else
            {
                obj.ID = save.Id;
                obj.transform.position = save.position;
                obj.transform.rotation = save.rotation;
            }
        }
    }

    public void Save()
    {
        SaveData.Singleton.savableObjects = new List<SAVE_SavableObject>();

        SavableObject[] objectsToSave = FindObjectsByType<SavableObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (SavableObject obj in objectsToSave)
        {
            SAVE_SavableObject save = new SAVE_SavableObject
            {
                Id = obj.ID,
                position = obj.transform.position,
                rotation = obj.transform.rotation,
                itemID = obj.itemID,
                itemStack = obj.itemStack
            };

            if (obj.GetComponent<Storage>() != null)
            {
                List<int> datas = new List<int>();
                List<int> stacks = new List<int>();

                for (int i = 0; i < obj.StorageItemsID.Length; i++)
                {
                    datas.Add(obj.StorageItemsID[i]);
                    stacks.Add(obj.StorageItemsStacks[i]);
                }

                save.StorageItemsData = datas.ToArray();
                save.StorageItemsStack = stacks.ToArray();
            }

            SaveData.Singleton.savableObjects.Add(save);
        }

        // Save player stats
        var playerStats = FindAnyObjectByType<PlayerStats>();
        SaveData.Singleton.currentHealth = playerStats.health;
        SaveData.Singleton.currentHunger = playerStats.hunger;
        SaveData.Singleton.currentThirst = playerStats.thirst;

        // Save time
        SaveData.Singleton.currentTime = FindAnyObjectByType<DayNightCicle>().timeOfDay;

        // Save inventory
        InventoryManager inventory = FindAnyObjectByType<Player>().GetComponentInChildren<InventoryManager>();
        SaveData.Singleton.itemsIDs = new List<int>();
        SaveData.Singleton.itemsStacks = new List<int>();

        foreach (var slot in inventory.inventorySlots)
        {
            if (slot.data == null)
            {
                SaveData.Singleton.itemsIDs.Add(0);
                SaveData.Singleton.itemsStacks.Add(0);
            }
            else
            {
                SaveData.Singleton.itemsIDs.Add(slot.data.ID);
                SaveData.Singleton.itemsStacks.Add(slot.stackSize);
            }
        }

        if (SaveData.Save())
        {
            Debug.Log("SAVE SYSTEM : Game was saved successfully.");
        }
        else
        {
            Debug.LogWarning("SAVE SYSTEM : An error occurred while saving the game.");
        }
    }

    public void DeleteSaveFile()
    {
        string savePath = Application.dataPath + "/saves/GameSave.save";
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
    }
}
