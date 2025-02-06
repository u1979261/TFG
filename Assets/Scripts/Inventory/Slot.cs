using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
public class Slot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private DragDropHandler dragDropHandler;
    private InventoryManager inventory;


    public ItemSO data;
    public int stackSize;

    [Space]
    public Image icon;
    public TextMeshProUGUI stackText;

    private bool _isEmpty;
    public bool isEmpty => _isEmpty;

    private void Start()
    {
        dragDropHandler = GetComponentInParent<DragDropHandler>();
        inventory = GetComponentInParent<InventoryManager>();
        UpdateSlot();
    }
    public void UpdateSlot()
    {
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
    public void Drop() { 
        GetComponentInParent<InventoryManager>().DropItem(this);
    }

    public void Clean()
    {
        data = null;
        stackSize = 0;
        UpdateSlot();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!dragDropHandler.isDragging)
        {
            if (eventData.button == PointerEventData.InputButton.Left && !isEmpty)
            {
                dragDropHandler.slotFrom = this;
                dragDropHandler.isDragging = true;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (dragDropHandler.isDragging)
        {
            //DROP
            if (dragDropHandler.slotTo == null)
            {
                dragDropHandler.slotFrom.Drop();
                dragDropHandler.isDragging = false;
            }

            else if (dragDropHandler.slotTo != null)
            {
                inventory.DragDrop(dragDropHandler.slotFrom, dragDropHandler.slotTo);
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
}
