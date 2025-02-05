using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class Slot : MonoBehaviour
{
    public ItemSO data;
    public int stackSize;

    [Space]
    public Image icon;
    public TextMeshProUGUI stackText;

    private bool _isEmpty;
    public bool isEmpty => _isEmpty;

    private void Start()
    {
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
}
