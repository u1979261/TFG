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
        if(loaded)
            Load();
    }
    public void Load()
    {
        // CHECK THAT DIRECTORY & FILE HAVE BEEN CREATED
        if (!Directory.Exists(Application.dataPath + "/saves"))
        {
            return;
        }

        if (!File.Exists(Application.dataPath + "/saves/GameSave.save"))
        {
            return;
        }


        // LOAD THE FILE TO SAVE DATA
        SaveData.Load();

        // LOAD THE PLAYER STATS
        FindAnyObjectByType<PlayerStats>().health = SaveData.Singleton.currentHealth;
        FindAnyObjectByType<PlayerStats>().hunger = SaveData.Singleton.currentHunger;
        FindAnyObjectByType<PlayerStats>().thirst = SaveData.Singleton.currentThirst;

        //LOAD DAY NIGHT CYCLE
        FindAnyObjectByType<DayNightCicle>().timeOfDay = SaveData.Singleton.currentTime;

        //LOAD INVENTORY ITEMS
        InventoryManager inventory = FindAnyObjectByType<Player>().GetComponentInChildren<InventoryManager>();
        ItemDataBase dataBase = FindAnyObjectByType<ItemDataBase>();

        int[] IDs = new int[0];
        int[] stacks = new int[0];

        IDs = SaveData.Singleton.itemsIDs.ToArray();
        stacks = SaveData.Singleton.itemsStacks.ToArray();


        for (int i = 0; i < inventory.inventorySlots.Length; i++)
        {
            if(SaveData.Singleton.itemsIDs[i] == 0)
            {
                inventory.inventorySlots[i].data = null;
                inventory.inventorySlots[i].stackSize = 0;
                inventory.inventorySlots[i].UpdateSlot();
            }
            else
            {
                
                inventory.inventorySlots[i].data = dataBase.GetItemData(IDs[i]);
                inventory.inventorySlots[i].stackSize = stacks[i];
                inventory.inventorySlots[i].UpdateSlot();
            }
        }

        // DESTROY INSTANTIATED SAVABLE OBJECT
        SavableObject[] objectsToDestroy = FindObjectsByType<SavableObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);


        for (int i = 0; i < objectsToDestroy.Length; i++)
        {
            if (objectsToDestroy[i].instantiate)
                Destroy(objectsToDestroy[i].gameObject);
        }



        // RUN THROUGH SAVABLE OBJECTS LIST AND LOAD THE DATA TO THEM
        foreach(SAVE_SavableObject save in SaveData.Singleton.savableObjects)
        {
            SavableObject obj = null;

            
            // FIND OBJECT IN SAVABLE OBJECTS
            for (int i = 0; i < savableObjects.Count; i++)
            {
                if (save.Id == savableObjects[i].ID)
                {
                    obj = savableObjects[i];
                    break;
                }
            }


            // IF THE OBJECT IS NOT FOUND THAN JUST RETURN
            if (obj == null)
            {
                Debug.LogError(obj);
                Debug.LogError($"SAVE SYSTEM : The object to load was not found, make sure the object is in the Savable Objects list of SaveHandler");
                return;
            }



            // LOAD DATA INTO THE OBJECT
            if (obj.instantiate)
            {
                SavableObject instance = Instantiate(obj.gameObject).GetComponent<SavableObject>();

                instance.ID = save.Id;
                instance.transform.position = save.position;
                instance.transform.rotation = save.rotation;

                //UPDATE DROP BAG
                instance.itemID = save.itemID;
                instance.itemStack = save.itemStack;

                if(instance.itemID != 0)
                {
                    Pickup pickup = instance.GetComponent<Pickup>();
                    if(pickup != null)
                    {
                        pickup.data = FindAnyObjectByType<ItemDataBase>().GetItemData(instance.itemID);
                        pickup.stackSize = instance.itemStack;
                    }
                    else
                    {
                        Debug.LogError($"SAVE SYSTEM : The object {instance.name} does not have a Pickup component, make sure to add it to the prefab.");
                        return;
                    }
                }

                //STORAGE SYSTEM
                if(instance.GetComponent<Storage>()!= null)
                {
                    Storage storage = instance.GetComponent<Storage>();
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
                            storage.slots[i].data = FindAnyObjectByType<ItemDataBase>().GetItemData(save.StorageItemsData[i]);
                            storage.slots[i].amount = save.StorageItemsData[i];
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
        // INITIALIZE THE SAVABLE OBJECT FILES LIST IN SAVE DATA
        SaveData.Singleton.savableObjects = new List<SAVE_SavableObject>();

        SavableObject[] objectsToSave = FindObjectsByType<SavableObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);


        foreach (SavableObject obj in objectsToSave)
        {
            // SAVE THE SAVABLE OBJECTS DATA TO THE SAVABLE OBJECT SAVE SCRIPT
            SAVE_SavableObject save = new SAVE_SavableObject();

            //POSITION
            save.Id = obj.ID;
            save.position = obj.transform.position;
            save.rotation = obj.transform.rotation;

            //DROP BAG

            save.itemID = obj.itemID;
            save.itemStack = obj.itemStack;

            //STORAGE ITEMS
            if (obj.GetComponent<Storage>() != null)
            {
                List<int> datas = new List<int>();
                List<int> stacks = new List<int>();

                for(int i = 0; i < obj.StorageItemsID.Length; i++)
                {
                    if (obj.StorageItemsID[i] == 0)
                    {
                        datas.Add(0);
                        stacks.Add(0);
                    }
                    else
                    {
                        datas.Add(obj.StorageItemsID[i]);
                        stacks.Add(obj.StorageItemsStacks[i]);
                    }
                }
                save.StorageItemsData = datas.ToArray();
                save.StorageItemsStack = stacks.ToArray();
            }

            SaveData.Singleton.savableObjects.Add(save);

        }

        // SAVE THE PLAYER STATS
        SaveData.Singleton.currentHealth = FindAnyObjectByType<PlayerStats>().health;
        SaveData.Singleton.currentHunger = FindAnyObjectByType<PlayerStats>().hunger;
        SaveData.Singleton.currentThirst = FindAnyObjectByType<PlayerStats>().thirst;

        // SAVE DAY NIGHT CYCLE
        SaveData.Singleton.currentTime = FindAnyObjectByType<DayNightCicle>().timeOfDay;

        // SAVE INVENTORY ITEMS

        InventoryManager inventory = FindAnyObjectByType<Player>().GetComponentInChildren<InventoryManager>();
        SaveData.Singleton.itemsIDs = new List<int>();
        SaveData.Singleton.itemsStacks = new List<int>();
        SaveData.Singleton.itemsIDs.Clear();
        SaveData.Singleton.itemsStacks.Clear();

        for(int i = 0; i < inventory.inventorySlots.Length; i++)
        {
            if(inventory.inventorySlots[i].data == null)
            {
                SaveData.Singleton.itemsIDs.Add(0);
                SaveData.Singleton.itemsStacks.Add(0);
            }
            else
            {
                SaveData.Singleton.itemsIDs.Add(inventory.inventorySlots[i].data.ID);
                SaveData.Singleton.itemsStacks.Add(inventory.inventorySlots[i].stackSize);
            }
        }

        // SAVE IT TO SAVE DATA
        if (SaveData.Save())
        {
            Debug.Log("SAVE SYSTEM : Game was saved successfully.");
        }
        else
        {
            Debug.LogWarning("SAVE SYSTEM : An error occured while saving the game.");
        }    
        
    }

    public void DeleteSaveFile()
    {
        if (!Directory.Exists(Application.dataPath + "/saves"))
        {
            return;
        }

        if (!File.Exists(Application.dataPath + "/saves/GameSave.save"))
        {
            return;
        }

        File.Delete(Application.dataPath + "/saves/GameSave.save");


    }
}
