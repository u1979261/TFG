using JetBrains.Annotations;
using UnityEngine;

public class ItemDataBase : MonoBehaviour
{
    public ItemSO [] items;
    public ItemSO GetItemData(int id)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].ID == id)
            {
                return items[i];
            }
        }
        Debug.LogError("Item with ID " + id + " not found in the database.");
        return null;
    }
}
