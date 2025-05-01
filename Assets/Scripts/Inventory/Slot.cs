using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private DragDropHandler dragDropHandler;
    private InventoryManager inventory;
    public Weapon weaponEquippedOn;

    public ItemSO data;
    public int stackSize;

    [Space]
    public Image icon;
    public TextMeshProUGUI stackText;

    private bool _isEmpty;
    public bool isEmpty => _isEmpty;

    private void Start()
    {
        dragDropHandler = GetComponentInParent<Player>().GetComponentInChildren<DragDropHandler>();
        inventory = GetComponentInParent<Player>().GetComponentInChildren<InventoryManager>();
        UpdateSlot();
    }

    public void UpdateSlot()
    {
        if (data != null && data.itemType != ItemSO.ItemType.Weapon && stackSize <= 0)
        {
            data = null;
        }

        if (data == null)
        {
            _isEmpty = true;
            icon.gameObject.SetActive(false);
            stackText.gameObject.SetActive(false);
        }
        else
        {
            _isEmpty = false;
            icon.sprite = data.itemIcon;
            stackText.text = $"x{stackSize}";

            icon.gameObject.SetActive(true);
            stackText.gameObject.SetActive(true);
        }
    }

    public void AddItemToSlot(ItemSO itemData, int itemStackSize)
    {
        data = itemData;
        stackSize = itemStackSize;
    }

    public void AddStackAmount(int itemStackSize)
    {
        stackSize += itemStackSize;
    }

    public void Drop()
    {
        ItemSO weapon = this.data;
        GetComponentInParent<InventoryManager>().DropItem(this);
        if (weaponEquippedOn != null && weapon == weaponEquippedOn.weaponData)
        {
            weaponEquippedOn.UnEquip();
        }
    }

    public void Clean()
    {
        data = null;
        stackSize = 0;
        UpdateSlot();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!dragDropHandler.isDragging && !isEmpty)
        {
            if (eventData.button == PointerEventData.InputButton.Left || eventData.button == PointerEventData.InputButton.Right || eventData.button == PointerEventData.InputButton.Middle)
            {
                dragDropHandler.slotFrom = this;
                dragDropHandler.isDragging = true;
                dragDropHandler.isRightClick = eventData.button == PointerEventData.InputButton.Right;
                dragDropHandler.isMiddleClick = eventData.button == PointerEventData.InputButton.Middle;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (dragDropHandler.isDragging)
        {
            bool isRightClick = eventData.button == PointerEventData.InputButton.Right;
            Debug.Log("Right Click: " + isRightClick);
            bool isMiddleClick = eventData.button == PointerEventData.InputButton.Middle;
            Debug.Log("Middle Click: " + isMiddleClick);

            if (dragDropHandler.slotTo == null)
            {
                dragDropHandler.slotFrom.Drop();
                dragDropHandler.isDragging = false;
            }
            else if (dragDropHandler.slotTo != null)
            {
                inventory.DragDrop(dragDropHandler.slotFrom, dragDropHandler.slotTo, isRightClick, isMiddleClick);
                dragDropHandler.isDragging = false;
            }
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (dragDropHandler.isDragging)
        {
            dragDropHandler.slotTo = this;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (dragDropHandler.isDragging)
        {
            dragDropHandler.slotTo = null;
        }
    }

    public void Use()
    {
        if (data == null) return;

        // Si no es un objeto de construcci�n, desactiva la construcci�n activa
        if (inventory.building.slotInUse != null && data.itemType != ItemSO.ItemType.Building)
        {
            inventory.building.slotInUse = null;

            if (inventory.building.buildReference != null)
            {
                Destroy(inventory.building.buildReference.gameObject);
            }
        }

        if (data.itemType == ItemSO.ItemType.Weapon || data.itemType == ItemSO.ItemType.MeleeWeapon)
        {
            bool shouldJustUnequip = false;

            for (int i = 0; i < inventory.weapons.Length; i++)
            {
                if (inventory.weapons[i].gameObject.activeSelf)
                {
                    if (inventory.weapons[i].slotEquippedOn == this)
                    {
                        shouldJustUnequip = true;
                    }
                    inventory.weapons[i].UnEquip();
                }
            }

            if (shouldJustUnequip) return;

            for (int i = 0; i < inventory.weapons.Length; i++)
            {
                if (inventory.weapons[i].weaponData == data)
                {
                    inventory.weapons[i].Equip(this);
                }
            }
        }
        else if (data.itemType == ItemSO.ItemType.Consumable)
        {
            Consume();
        }
        else if (data.itemType == ItemSO.ItemType.Building)
        {
            Build();
        }
    }


    public void Build()
    {
        for (int i = 0; i < inventory.weapons.Length; i++)
        {
            if (inventory.weapons[i].gameObject.activeSelf)
            {
                inventory.weapons[i].UnEquip();
            }
        }

        if (inventory.building.slotInUse == null)
        {
            inventory.building.slotInUse = this;
        }
        else
        {
            if (inventory.building.slotInUse == this)
            {
                inventory.building.slotInUse = null;
            }
            else
            {
                inventory.building.slotInUse = this;
                Destroy(inventory.building.buildReference.gameObject);
            }
        }
    }

    public void Consume()
    {
        PlayerStats stats = GetComponentInParent<PlayerStats>();
        stats.health += data.healthStats;
        stats.hunger += data.hungerStats;
        stats.thirst += data.thirstStats;

        stackSize--;
        UpdateSlot();
    }
}
