using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class InventoryManager : MonoBehaviour
{
    public Weapon[] weapons;
    public BuildingHandler building;

    public bool opened;
    private InputSystem_Actions _playerInput;

    [Header("Invetory Settings")]
    public int inventorySize = 24;
    public int hotBarSize = 6;

    [Header("Refs")]
    public GameObject dropModel;
    public Transform dropPos;
    public GameObject slotTemplate;
    public Transform contentHolder;
    public Transform hotBarContentHolder;

    [Header("HOTBAR")]
    public GameObject inventoryPanel;
    public GameObject CraftingPanel;



    [HideInInspector] public Slot[] inventorySlots;
    private Slot[] hotBarSlots;

    private void Start()
    {
        _playerInput = new InputSystem_Actions();
        _playerInput.Player.Enable();
        GenerateSlots();
        GenerateSlotsHotBar();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            hotBarSlots[0].Use();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            hotBarSlots[1].Use();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            hotBarSlots[2].Use();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            hotBarSlots[3].Use();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            hotBarSlots[4].Use();
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            hotBarSlots[5].Use();
        }

        if (_playerInput.Player.Inventory.WasPressedThisFrame())
        {
            opened = !opened;
        }
        if (opened)
        {
            inventoryPanel.transform.localPosition = new Vector3(0, 4, 0);
        }
        else
        {
            inventoryPanel.transform.localPosition = new Vector3(-100000, 4, 0);
            if (GetComponentInParent<WindowHandler>().storage.opened)
            {
                GetComponentInParent<WindowHandler>().storage.Close();
            }
        }
        if (GetComponentInParent<WindowHandler>().storage.opened)
        {
            GetComponentInParent<WindowHandler>().crafting.opened = false;
        }
        else
        {
            GetComponentInParent<WindowHandler>().crafting.opened = true;
        }

    }

    private void OnDisable() 
    {
        _playerInput.Player.Disable();
    }
    
    private void GenerateSlots()
    {
        List<Slot> inventorySlots_ = new List<Slot>();

        // GENERATE SLOTS
        for (int i = 0; i < inventorySize; i++)
        { 
            Slot slot = Instantiate(slotTemplate,contentHolder).GetComponent<Slot>();

            inventorySlots_.Add(slot);
        }
        inventorySlots = inventorySlots_.ToArray();
    }
    private void GenerateSlotsHotBar()
    {
        List<Slot> inventorySlots_ = new List<Slot>();
        List<Slot> hotBarList = new List<Slot>();

        //UPDATTE HOTBAR SLOTS
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots_.Add(inventorySlots[i]);
        }

        // GENERATE SLOTS
        for (int i = 0; i < hotBarSize; i++)
        {
            Slot slot = Instantiate(slotTemplate, hotBarContentHolder).GetComponent<Slot>();

            inventorySlots_.Add(slot);
            hotBarList.Add(slot);
        }
        inventorySlots = inventorySlots_.ToArray();
        hotBarSlots = hotBarList.ToArray();
    }
    public void AddItem(Pickup pickUp)
    {
        if (pickUp.data.isStackable)
        {
            Slot stackableSlot = null;

            // TRY FINDING STACKABLE SLOT
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (!inventorySlots[i].isEmpty)
                {
                    if (inventorySlots[i].data == pickUp.data && inventorySlots[i].stackSize < pickUp.data.maxStack)
                    {
                        stackableSlot = inventorySlots[i];
                        break;
                    }

                }
            }

            if (stackableSlot != null)
            {

                // IF IT CANNOT FIT THE PICKED UP AMOUNT
                if (stackableSlot.stackSize + pickUp.stackSize > pickUp.data.maxStack)
                {
                    int amountLeft = (stackableSlot.stackSize + pickUp.stackSize) - pickUp.data.maxStack;



                    // ADD IT TO THE STACKABLE SLOT
                    stackableSlot.AddItemToSlot(pickUp.data, pickUp.data.maxStack);

                    // TRY FIND A NEW EMPTY STACK
                    for (int i = 0; i < inventorySlots.Length; i++)
                    {
                        if (inventorySlots[i].isEmpty)
                        {
                            inventorySlots[i].AddItemToSlot(pickUp.data, amountLeft);
                            inventorySlots[i].UpdateSlot();

                            break;
                        }
                    }



                    Destroy(pickUp.gameObject);
                }
                // IF IT CAN FIT THE PICKED UP AMOUNT
                else
                {
                    stackableSlot.AddStackAmount(pickUp.stackSize);

                    Destroy(pickUp.gameObject);
                }

                stackableSlot.UpdateSlot();
            }
            else
            {
                Slot emptySlot = null;


                // FIND EMPTY SLOT
                for (int i = 0; i < inventorySlots.Length; i++)
                {
                    if (inventorySlots[i].isEmpty)
                    {
                        emptySlot = inventorySlots[i];
                        break;
                    }
                }

                // IF WE HAVE AN EMPTY SLOT THAN ADD THE ITEM
                if (emptySlot != null)
                {
                    emptySlot.AddItemToSlot(pickUp.data, pickUp.stackSize);
                    emptySlot.UpdateSlot();

                    Destroy(pickUp.gameObject);
                }
                else
                {
                    pickUp.transform.position = dropPos.position;
                }
            }

        }
        else
        {
            Slot emptySlot = null;


            // FIND EMPTY SLOT
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i].isEmpty)
                {
                    emptySlot = inventorySlots[i];
                    break;
                }
            }

            // IF WE HAVE AN EMPTY SLOT THAN ADD THE ITEM
            if (emptySlot != null)
            {
                emptySlot.AddItemToSlot(pickUp.data, pickUp.stackSize);
                emptySlot.UpdateSlot();

                Destroy(pickUp.gameObject);
            }
            else
            {
                pickUp.transform.position = dropPos.position;
            }

        }
    }
    public void DropItem(Slot slot)
    {
        GameObject modelToDrop = slot.data.specificDropModel != null ? slot.data.specificDropModel : dropModel;
        Pickup pickup = Instantiate(modelToDrop, dropPos).GetComponent<Pickup>();
        pickup.transform.position = dropPos.position;
        pickup.transform.SetParent(null);

        pickup.data = slot.data;
        pickup.stackSize = slot.stackSize;

        slot.Clean();
    }


    public void DragDrop(Slot from, Slot to, bool isRightClick, bool isMiddleClick)
    {
        if (from.weaponEquippedOn != null) from.weaponEquippedOn.UnEquip();
        if (to.weaponEquippedOn != null) to.weaponEquippedOn.UnEquip();

        if (from == building.slotInUse) building.slotInUse = null;
        if (to == building.slotInUse) building.slotInUse = null;

        bool isWeapon = from.data.itemType == ItemSO.ItemType.Weapon || from.data.itemType == ItemSO.ItemType.MeleeWeapon;

        // Desactivar clic derecho y medio con armas
        if (isWeapon && (isRightClick || isMiddleClick))
            return;

        // Intercambio completo si son distintos o no stackeables o armas
        if (from.data != to.data || from.data == null || !from.data.isStackable || isWeapon)
        {
            ItemSO data = to.data;
            int stackSize = to.stackSize;

            to.data = from.data;
            to.stackSize = isMiddleClick ? from.stackSize / 2 : (isRightClick ? 1 : from.stackSize);

            if (isRightClick || isMiddleClick)
            {
                int amountRemoved = to.stackSize;
                from.stackSize -= amountRemoved;
            }
            else
            {
                from.data = data;
                from.stackSize = stackSize;
            }

            if (from.stackSize <= 0)
                from.data = null;
        }
        else if (from.data.isStackable)
        {
            int amountToMove = isRightClick ? 1 :
                               isMiddleClick ? from.stackSize / 2 :
                               from.stackSize;

            int availableSpace = from.data.maxStack - to.stackSize;
            amountToMove = Mathf.Min(amountToMove, availableSpace);

            to.stackSize += amountToMove;
            from.stackSize -= amountToMove;

            if (from.stackSize <= 0)
                from.data = null;
        }

        from.UpdateSlot();
        to.UpdateSlot();
    }



    public void AddItem(ItemSO data, int stackSize)
    {
        if (data.isStackable)
        {
            Slot stackableSlot = null;

            // TRY FINDING STACKABLE SLOT
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (!inventorySlots[i].isEmpty)
                {
                    if (inventorySlots[i].data == data && inventorySlots[i].stackSize < data.maxStack)
                    {
                        stackableSlot = inventorySlots[i];
                        break;
                    }

                }
            }

            if (stackableSlot != null)
            {

                // IF IT CANNOT FIT THE PICKED UP AMOUNT
                if (stackableSlot.stackSize + stackSize > data.maxStack)
                {
                    int amountLeft = (stackableSlot.stackSize + stackSize) - data.maxStack;



                    // ADD IT TO THE STACKABLE SLOT
                    stackableSlot.AddItemToSlot(data, data.maxStack);

                    // TRY FIND A NEW EMPTY STACK
                    for (int i = 0; i < inventorySlots.Length; i++)
                    {
                        if (inventorySlots[i].isEmpty)
                        {
                            inventorySlots[i].AddItemToSlot(data, amountLeft);
                            inventorySlots[i].UpdateSlot();

                            break;
                        }
                    }
                }
                // IF IT CAN FIT THE PICKED UP AMOUNT
                else
                {
                    stackableSlot.AddStackAmount(stackSize);
                }

                stackableSlot.UpdateSlot();
            }
            else
            {
                Slot emptySlot = null;


                // FIND EMPTY SLOT
                for (int i = 0; i < inventorySlots.Length; i++)
                {
                    if (inventorySlots[i].isEmpty)
                    {
                        emptySlot = inventorySlots[i];
                        break;
                    }
                }

                // IF WE HAVE AN EMPTY SLOT THAN ADD THE ITEM
                if (emptySlot != null)
                {
                    emptySlot.AddItemToSlot(data,stackSize);
                    emptySlot.UpdateSlot();
                }
                else
                {
                    DropItem(data, stackSize);
                }
            }

        }
        else
        {
            Slot emptySlot = null;


            // FIND EMPTY SLOT
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i].isEmpty)
                {
                    emptySlot = inventorySlots[i];
                    break;
                }
            }

            // IF WE HAVE AN EMPTY SLOT THAN ADD THE ITEM
            if (emptySlot != null)
            {
                emptySlot.AddItemToSlot(data, stackSize);
                emptySlot.UpdateSlot();
            }
            else
            {
                DropItem(data, stackSize);
            }

        }
    }

    public void DropItem(ItemSO data, int stackSize)
    {
        Pickup pickup = Instantiate(dropModel, dropPos).GetComponent<Pickup>();
        pickup.transform.position = dropPos.position;
        pickup.transform.SetParent(null);

        pickup.data = data;
        pickup.stackSize = stackSize;
    }
}
