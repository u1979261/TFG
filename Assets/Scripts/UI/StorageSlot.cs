using UnityEngine;

public class StorageSlot : MonoBehaviour
{
    [HideInInspector]public bool isEmpty;

    public ItemSO data;
    public int amount;

    private void Update()
    {
        isEmpty = data == null;
    }

    public void AddItemToSlot(ItemSO itemData, int itemStackSize)
    {
        data = itemData;
        amount = itemStackSize;
    }

    public void AddStackAmount(int itemStackSize)
    {
        amount += itemStackSize;
    }
}
